namespace DeciderDemo;

public class MessageBus
{
    public static void Publish(object @event)
    {
        Console.WriteLine($"Event of type {@event.GetType().Name} was published");
        Console.WriteLine($"\t{System.Text.Json.JsonSerializer.Serialize(@event)}");
    }

    public void PublishAll(IEnumerable<object> events)
    {
        foreach (var @event in events) Publish(@event);
    }
}