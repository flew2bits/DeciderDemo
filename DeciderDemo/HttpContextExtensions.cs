namespace DeciderDemo;

public static class HttpContextExtensions
{
    public static IEnumerable<object> StoredEvents(this HttpContext context)
        => (context.Items["events"] as List<object>)?.ToArray() ?? Array.Empty<object>();

    public static void AddStoredEvent(this HttpContext context, object @event)
    {
        if (!context.Items.ContainsKey("events")) context.Items.Add("events", new List<object>());

        (context.Items["events"] as List<object>)?.Add(@event);
    }
}