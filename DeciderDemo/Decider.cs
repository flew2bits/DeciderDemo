namespace DeciderDemo;

public record Evolver<TState, TIdentity, TEvent>(
    Func<TState, TEvent, TState> Evolve,
    Func<TIdentity, TState> InitialState
) where TState: class where TEvent: class;

public record Decider<TState, TIdentity, TCommand, TEvent>(
    Func<TState, TCommand, IEnumerable<TEvent>> Decide,
    Func<TState, TEvent, TState> Evolve, 
    Func<TIdentity, TState> InitialState,
    Predicate<TState> IsFinal, 
    Predicate<TCommand> IsCreator)
    : Evolver<TState, TIdentity, TEvent>(Evolve,InitialState)
    where TState : class
    where TCommand : class
    where TEvent : class
{
    public (TState, TEvent[]) Handle(TState state, TCommand command)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(command);

        var events = Decide(state, command).ToArray();
        var newState = events.Aggregate(state, Evolve);
        return (newState, events);
    }
}