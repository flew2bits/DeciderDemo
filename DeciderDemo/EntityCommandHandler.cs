namespace DeciderDemo;

public abstract record EntityCommandHandler<TState, TIdentity, TCommand>(
    Decider<TState, TIdentity, TCommand> Decider,
    Loader<TIdentity, TState> LoadEntity,
    IEnumerable<Saver<TIdentity, TState>> EntitySavers,
    Archiver<TIdentity>? ArchiveIdentity = null
)
    where TState : class
    where TCommand : class
{
    private bool TryLoad(TIdentity identity, out TState? state)
    {
        state = null;
        
        try
        {
            state = LoadEntity(identity);
        }
        catch
        {
            return false;
        }

        return true;
    }
    
    public (TState, IEnumerable<object>) HandleCommand(TIdentity identity, TCommand command)
    {
        var state = (TryLoad(identity, out var s), Decider.IsCreator(command)) switch
        {
            (false, true) => Decider.InitialState(identity),
            (true, false) => s!,
            (true, true) => throw new InvalidOperationException("An entity with the given id already exists"),
            (false, false) => throw new InvalidOperationException("Could not find the entity with the given id")
        };
        
        var (newState, events) = Decider.Handle(state, command);
        // Archive if newState is terminal?

        foreach (var saver in EntitySavers)
        {
            if (!saver(identity, newState, events)) break;
        }

        if (Decider.IsFinal(newState))
        {
               ArchiveIdentity?.Invoke(identity);
        }
        return (newState, events);
    }
}

public delegate TState Loader<in TIdentifier, out TState>(TIdentifier id) where TState : class;

public delegate bool Saver<in TIdentifier, in TState>(TIdentifier id, TState state,
    IEnumerable<object> events)
    where TState : class;

public delegate void Archiver<in TIdentifier>(TIdentifier id);