namespace DeciderDemo.Entities;

public class TimeStampEventMetadataProvider: IEventMetadataProvider
{
    private const string Timestamp = nameof(Timestamp);

    public string Category => "Time";
    
    public IEnumerable<KeyValuePair<string, object>> GetValues()
    {
        yield return new KeyValuePair<string, object>(Timestamp, DateTime.UtcNow);
    }
}