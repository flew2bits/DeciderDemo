namespace DeciderDemo;

public record EntityCommandHandler<TState, TIdentity, TCommand, TEvent>(
    Loader<TState, TIdentity> LoadEntity,
    Saver<TState, TIdentity, TEvent> SaveEntity,
    Decider<TState, TIdentity, TCommand, TEvent> Decider
)
    where TState : class
    where TCommand : class
    where TEvent : class
{
    public (TState, IEnumerable<TEvent>) HandleCommand(TIdentity identity, TCommand command)
    {
        var state = Decider.IsCreator(command) ? Decider.InitialState(identity) : LoadEntity(identity);
        var (newState, events) = Decider.Handle(state, command);
        SaveEntity(identity, newState, events);
        return (newState, events);
    }
}

public delegate TState Loader<out TState, in TIdentifier>(TIdentifier id) where TState : class;

public delegate void Saver<in TState, in TIdentifier, in TEvent>(TIdentifier id, TState state,
    IEnumerable<TEvent> events)
    where TState : class;
