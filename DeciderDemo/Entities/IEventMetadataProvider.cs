namespace DeciderDemo.Entities;

public interface IEventMetadataProvider
{
    string Category { get; }
    IEnumerable<KeyValuePair<string, object>> GetValues();
}