namespace DeciderDemo;

public record EntityCommandHandler<TState, TIdentity, TCommand, TEvent>(Func<TIdentity, TState> Loader,
    Action<TIdentity, TState, IEnumerable<TEvent>> Saver, Decider<TState, TIdentity, TCommand, TEvent> Decider) 
    where TState: class 
    where TCommand: class
    where TEvent: class
{
    public (TState, IEnumerable<TEvent>) HandleCommand(TIdentity identity, TCommand command)
    {
        var state = Decider.IsCreator(command) ? Decider.InitialState(identity) : Loader(identity);
        var (newState, events) = Decider.Handle(state, command);
        Saver(identity, newState, events);
        return (newState, events);
    }
}
