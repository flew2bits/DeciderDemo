using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Options;
using EventMetadata = System.Collections.Generic.Dictionary<string, object>;
namespace DeciderDemo.Entities;

public class FileSystemEntityDatabaseOptions<TIdentity, TState> where TState : class
{
    public string Prefix { get; set; } = $"{typeof(TState).Name}_";
    public string BasePath { get; set; } = "ConferenceDB";
    public string ArchivePath { get; set; } = Path.Combine("ConferenceDB", "Archive");

    public Evolver<TIdentity, TState>? Evolver { get; set; }
}

public abstract class FileSystemEntityDatabase
{
    protected static readonly JsonSerializerOptions
        SerializerOptions = new() { WriteIndented = true };

    protected static readonly ConcurrentDictionary<string, Type> TypeMap = new();
}

public class FileSystemEntityDatabase<TIdentity, TState> : FileSystemEntityDatabase
    where TState : class where TIdentity : IParsable<TIdentity>

{
    private readonly Evolver<TIdentity, TState>? _evolver;

    public FileSystemEntityDatabase(IOptions<FileSystemEntityDatabaseOptions<TIdentity, TState>> options,
        IEnumerable<IEventMetadataProvider> metadataProviders)
    {
        _evolver = options.Value.Evolver;
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        _metadataProviders = metadataProviders.ToArray();
    }

    private readonly FileSystemEntityDatabaseOptions<TIdentity, TState> _options;
    private readonly IEventMetadataProvider[] _metadataProviders;

    public async Task<ICollection<TState>> GetAll()
    {
        var files = Directory.EnumerateFiles(_options.BasePath, $"{_options.Prefix}*.json");
        var ids = files.Select(f => f[(_options.BasePath.Length + 1 + _options.Prefix.Length)..^5])
            .Select(i => TIdentity.Parse(i, null));
        return await ids.ToAsyncEnumerable().SelectAwait(async x => await Find(x)).ToArrayAsync();
    }

    private string StreamPath(TIdentity id) => Path.Combine(_options.BasePath, $"{_options.Prefix}{id}.jsonstream");
    private string StatePath(TIdentity id) => Path.Combine(_options.BasePath, $"{_options.Prefix}{id}.json");
    
    public async Task<TState> Find(TIdentity id) =>
        await (_evolver != null ? HydrateState(_evolver!, id) : LoadFromState(id));

    private async Task<TState> LoadFromState(TIdentity id) =>
        JsonSerializer.Deserialize<TState>(await File.ReadAllTextAsync(StatePath(id))) 
        ?? throw new InvalidOperationException("Could not deserialize state");

    private async Task<TState> HydrateState(Evolver<TIdentity, TState> evolver, TIdentity id)
        => await LoadEventStream(id).AggregateAsync(evolver.InitialState(id), evolver.Evolve);

    private async IAsyncEnumerable<object> LoadEventStream(TIdentity id)
    {
        var path = StreamPath(id);
        if (!File.Exists(path)) throw new InvalidOperationException("Could not find entity");
        var events = await File.ReadAllLinesAsync(path);
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

            yield return @event;
        }
    }
    
    public async Task<bool> Save(TIdentity id, TState state, IEnumerable<object> events)
    {
        try
        {
            var path = StatePath(id);
            var streamPath = StreamPath(id);

            var metadata = _metadataProviders
                .SelectMany(p => p.GetValues().Select(v => (Key: $"{p.Category}_{v.Key}", v.Value)))
                .ToArray();

            await File.WriteAllTextAsync(path, JsonSerializer.Serialize(state, SerializerOptions));
            await File.AppendAllLinesAsync(streamPath,
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
        var path = StatePath(id);
        var streamPath = StreamPath(id);
        var archivePath = Path.Combine(_options.ArchivePath, $"{archiveId}_{_options.Prefix}{id}.json");
        var archiveStreamPath = Path.Combine(_options.ArchivePath, $"{archiveId}_{_options.Prefix}{id}.jsonstream");

        File.Move(path, archivePath);
        File.Move(streamPath, archiveStreamPath);
    }
}