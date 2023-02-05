namespace DeciderDemo;

public abstract record EntityCommandHandler<TState, TIdentity, TCommand, TEvent>(
    Loader<TState, TIdentity> LoadEntity,
    Saver<TState, TIdentity, TEvent> SaveEntity,
    Decider<TState, TIdentity, TCommand, TEvent> Decider
)
    where TState : class
    where TCommand : class
    where TEvent : class
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
    
    public (TState, IEnumerable<TEvent>) HandleCommand(TIdentity identity, TCommand command)
    {
        var state = (TryLoad(identity, out var s), Decider.IsCreator(command)) switch
        {
            (false, true) => Decider.InitialState(identity),
            (true, false) => s!,
            (true, true) => throw new InvalidOperationException("An entity with the given id already exists"),
            (false, false) => throw new InvalidOperationException("Could not find the entity with the given id")
        };
        
        var (newState, events) = Decider.Handle(state, command);
        SaveEntity(identity, newState, events);
        return (newState, events);
    }
}

public delegate TState Loader<out TState, in TIdentifier>(TIdentifier id) where TState : class;

public delegate void Saver<in TState, in TIdentifier, in TEvent>(TIdentifier id, TState state,
    IEnumerable<TEvent> events)
    where TState : class;
