using System.Reflection;
using System.Text.Json;

namespace DeciderDemo.Entities;

public class FileSystemEntityDatabaseOptions<TState, TIdentity, TEvent> where TState: class where TEvent: class
{
    public string Prefix { get; set; } = $"{typeof(TState).Name}_";
    public string BasePath { get; set; } = "ConferenceDB";

    public string ArchivePath { get; set; } = Path.Combine("ConferenceDB", "Archive");

    public Evolver<TState, TIdentity, TEvent> Evolver { get; set; } = null!;
    public IdentityConverter<TIdentity>? IdentityConverter { get; set; } = null!;
}

public record IdentityConverter<TIdentity>(Func<TIdentity, string> AsString, Func<string, TIdentity> FromString);

public abstract class FileSystemEntityDatabase {

    protected static readonly JsonSerializerOptions
        SerializerOptions = new() { WriteIndented = true }; 
}

public class FileSystemEntityDatabase<TState, TIdentity, TEvent>: FileSystemEntityDatabase
    where TState: class where TEvent: class

{
    private readonly IdentityConverter<TIdentity> _identityConverter;
    private readonly Evolver<TState, TIdentity, TEvent> _evolver;
    
    public FileSystemEntityDatabase(FileSystemEntityDatabaseOptions<TState, TIdentity, TEvent> options)
    {
        _identityConverter = options.IdentityConverter
                             ?? TryBuildConverter()
                             ?? throw new ArgumentException("IdentityConverter is not defined or could not be determined");
        _evolver = options.Evolver ?? throw new ArgumentException("Evolver is not defined");
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    private static IdentityConverter<TIdentity>? TryBuildConverter()
    {
        var type = typeof(TIdentity);

        // if (type == typeof(string))
        // {
        //     return new IdentityConverter<TIdentity>(s => s as string, s => s);
        // }
        
        var parseMethod = type.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m => m.Name == "Parse");
        if (parseMethod is null) return null;
        
        var returnType = parseMethod.ReturnType;
        if (returnType != type) return null;
        
        var parameters = parseMethod.GetParameters();
        if (parameters.Length != 1) return null;
        if (parameters[0].ParameterType != typeof(string)) return null;

        TIdentity Parse(string s) => 
            parseMethod.Invoke(null, new[] { s }) is TIdentity i ? i : throw new InvalidOperationException("Could not parse");

        return new IdentityConverter<TIdentity>(i => i?.ToString() ?? throw new InvalidOperationException("Could not convert to string"), Parse);
    }

    
    private static readonly Dictionary<string, Type> TypeMap = new ();
    private readonly FileSystemEntityDatabaseOptions<TState, TIdentity, TEvent> _options;

    public TState[] GetAll()
    {
        var files = Directory.EnumerateFiles(_options.BasePath, $"{_options.Prefix}*.json");
        var ids = files.Select(f => f[(_options.BasePath.Length + 1 + _options.Prefix.Length)..^5]).Select(_identityConverter.FromString);
        return ids.Select(LoadFromEvents).ToArray();

    }

    public TState Find(TIdentity id)
    {
        return LoadFromEvents(id);
    }

    private TState LoadFromEvents(TIdentity id)
    {
        var path = Path.Combine(_options.BasePath, $"{_options.Prefix}{_identityConverter.AsString(id)}.jsonstream");
        if (!File.Exists(path)) throw new InvalidOperationException("Could not find entity");
        var events = File.ReadAllLines(path);
        var state = _evolver.InitialState(id);
        foreach (var line in events)
        {
            var parts = line.Split(':', 2);
            if (!TypeMap.TryGetValue(parts[0], out var type))
            {
                type = Type.GetType(parts[0]);
                if (type is not null) TypeMap.TryAdd(parts[0], type);
            }

            if (type is null) throw new InvalidOperationException("Could not find type");

            if (JsonSerializer.Deserialize(parts[1], type) is not TEvent @event) 
                throw new InvalidOperationException("Could not parse type");
            state = _evolver.Evolve(state, @event);
        }

        return state;
    }

    public void Save(TIdentity id, TState state, IEnumerable<TEvent> events)
    {
        var path = Path.Combine(_options.BasePath, $"{_options.Prefix}{id}.json");
        var streamPath = Path.Combine(_options.BasePath, $"{_options.Prefix}{id}.jsonstream");
        File.WriteAllText(path, JsonSerializer.Serialize(state, SerializerOptions));
        File.AppendAllLines(streamPath, events.Select(e => $"{e.GetType().FullName}:"+JsonSerializer.Serialize((object)e)));
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