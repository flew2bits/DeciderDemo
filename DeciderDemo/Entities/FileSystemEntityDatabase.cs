using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Options;
using EventMetadata = System.Collections.Generic.Dictionary<string, object>;
namespace DeciderDemo.Entities;

public class FileSystemEntityDatabaseOptions<TState, TIdentity> where TState : class
{
    public string Prefix { get; set; } = $"{typeof(TState).Name}_";
    public string BasePath { get; set; } = "ConferenceDB";
    public string ArchivePath { get; set; } = Path.Combine("ConferenceDB", "Archive");

    public Evolver<TState, TIdentity> Evolver { get; set; } = null!;
}

public abstract class FileSystemEntityDatabase
{
    protected static readonly JsonSerializerOptions
        SerializerOptions = new() { WriteIndented = true };

    protected static readonly ConcurrentDictionary<string, Type> TypeMap = new();
}

public class FileSystemEntityDatabase<TState, TIdentity> : FileSystemEntityDatabase
    where TState : class where TIdentity : IParsable<TIdentity>

{
    private readonly Evolver<TState, TIdentity> _evolver;

    public FileSystemEntityDatabase(IOptions<FileSystemEntityDatabaseOptions<TState, TIdentity>> options,
        IEnumerable<IEventMetadataProvider> metadataProviders)
    {
        _evolver = options.Value.Evolver ?? throw new ArgumentException("Evolver is not defined");
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        _metadataProviders = metadataProviders.ToArray();
    }

    private readonly FileSystemEntityDatabaseOptions<TState, TIdentity> _options;
    private readonly IEventMetadataProvider[] _metadataProviders;

    public TState[] GetAll()
    {
        var files = Directory.EnumerateFiles(_options.BasePath, $"{_options.Prefix}*.json");
        var ids = files.Select(f => f[(_options.BasePath.Length + 1 + _options.Prefix.Length)..^5])
            .Select(i => TIdentity.Parse(i, null));
        return ids.Select(LoadFromEvents).ToArray();
    }

    public TState Find(TIdentity id)
    {
        return LoadFromEvents(id);
    }

    private TState LoadFromEvents(TIdentity id)
    {

        
        var path = Path.Combine(_options.BasePath, $"{_options.Prefix}{id}.jsonstream");
        if (!File.Exists(path)) throw new InvalidOperationException("Could not find entity");
        var events = File.ReadAllLines(path);
        var state = _evolver.InitialState(id);
        foreach (var linePair in events.Chunk(2))
        {
            var eventMetadata = JsonSerializer.Deserialize<EventMetadata>(linePair.First());
            if (eventMetadata is null) throw new InvalidOperationException("Could not parse event");

            if (!eventMetadata.TryGetValue("$type", out var typeName))
                throw new InvalidOperationException("Could not get event type");

            var typeNameValue = typeName.ToString() ?? throw new InvalidOperationException("Type name is not a string");

            if (!TypeMap.TryGetValue(typeNameValue, out var type))
            {
                type = Type.GetType(typeNameValue);
                if (type is not null) TypeMap.TryAdd(typeNameValue, type);
            }

            if (type is null) throw new InvalidOperationException("Could not find type");

            if (JsonSerializer.Deserialize(linePair.Last(), type) is not { } @event)
                throw new InvalidOperationException("Could not parse type");
            state = _evolver.Evolve(state, @event);
        }

        return state;
    }

    public bool Save(TIdentity id, TState state, IEnumerable<object> events)
    {
        try
        {
            var path = Path.Combine(_options.BasePath, $"{_options.Prefix}{id}.json");
            var streamPath = Path.Combine(_options.BasePath, $"{_options.Prefix}{id}.jsonstream");

            var metadata = _metadataProviders
                .SelectMany(p => p.GetValues().Select(v => (Key: $"{p.Category}_{v.Key}", v.Value)))
                .ToArray();

            File.WriteAllText(path, JsonSerializer.Serialize(state, SerializerOptions));
            File.AppendAllLines(streamPath,
                events.Select(e =>
                    $"{JsonSerializer.Serialize(metadata.Append(("$type", e.GetType().FullName!)).ToDictionary(k => k.Key, v => v.Value))}\n{JsonSerializer.Serialize((object)e)}"));
        }
        catch
        {
            return false;
        }

        return true;
    }

    public void Archive(TIdentity id)
    {
        var archiveId = Guid.NewGuid();
        var path = Path.Combine(_options.BasePath, $"{_options.Prefix}{id}.json");
        var streamPath = Path.Combine(_options.BasePath, $"{_options.Prefix}{id}.jsonstream");
        var archivePath = Path.Combine(_options.ArchivePath, $"{archiveId}_{_options.Prefix}{id}.json");
        var archiveStreamPath = Path.Combine(_options.ArchivePath, $"{archiveId}_{_options.Prefix}{id}.jsonstream");

        File.Move(path, archivePath);
        File.Move(streamPath, archiveStreamPath);
    }
}