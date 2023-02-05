namespace DeciderDemo.Entities;

public static class EntityHelpers
{
    public static Saver<TIdentity, TState, TEvent>
        SaveThenPublish<TIdentity, TState, TEvent>(
            Saver<TIdentity, TState, TEvent> save,
                Action<IEnumerable<TEvent>> publish
        )  where TState: class where TEvent: class => (identity, state, events) =>
    {
        var e = events as TEvent[] ?? events.ToArray();
        save(identity, state, e);
        publish(e);
    };
}