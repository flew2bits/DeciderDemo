namespace DeciderDemo;

public record Evolver<TState, TIdentity>(
    Func<TState, object, TState> Evolve,
    Func<TIdentity, TState> InitialState
) where TState : class;

public record Decider<TState, TIdentity, TCommand>(
        Func<TState, TCommand, IEnumerable<object>> Decide,
        Func<TState, object, TState> Evolve,
        Func<TIdentity, TState> InitialState,
        Predicate<TState> IsFinal,
        Predicate<TCommand> IsCreator)
    : Evolver<TState, TIdentity>(Evolve, InitialState)
    where TState : class
    where TCommand : class
{
    public (TState, object[]) Handle(TState state, TCommand command)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(command);

        var events = Decide(state, command).ToArray();
        var newState = events.Aggregate(state, Evolve);
        return (newState, events);
    }
}