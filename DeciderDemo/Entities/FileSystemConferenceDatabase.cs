using System.Dynamic;
using System.Text.Json;
using DeciderDemo.Entities.Conference;
using DeciderDemo.Entities.Conference.Events;
using Microsoft.Extensions.Options;

namespace DeciderDemo.Entities;

public class FileSystemConferenceDatabaseOptions
{
    public string Prefix { get; set; } = "Conference_";
    public string BasePath { get; set; } = ".";
}

public class FileSystemConferenceDatabase
{
    public FileSystemConferenceDatabase(IOptions<FileSystemConferenceDatabaseOptions> options)
    {
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
    }
    

    private static readonly JsonSerializerOptions
        SerializerOptions = new JsonSerializerOptions { WriteIndented = true }; 
    
    private static readonly Dictionary<string, Type> TypeMap = new ();
    private readonly FileSystemConferenceDatabaseOptions _options;

    public ConferenceState[] GetAllConferences()
    {
        var files = Directory.EnumerateFiles(_options.BasePath, $"{_options.Prefix}*.json");
        var ids = files.Select(f => f[(_options.BasePath.Length + 1 + _options.Prefix.Length)..^5]).Select(Guid.Parse);
        return ids.Select(LoadFromEvents).ToArray();
        return files.Select(f => JsonSerializer.Deserialize<ConferenceState>(File.ReadAllText(f)))
            .Where(c => c is not null)
            .Cast<ConferenceState>()
            .ToArray();
    }

    public ConferenceState FindConference(Guid id)
    {
        return LoadFromEvents(id);
        var path = Path.Combine(_options.BasePath, $"{_options.Prefix}{id}.json");
        if (!File.Exists(path)) throw new InvalidOperationException("Conference was not found");
        return JsonSerializer.Deserialize<ConferenceState>(File.ReadAllText(path)) ??
               throw new InvalidOperationException("Could not load conference");
    }

    private ConferenceState LoadFromEvents(Guid id)
    {
        var path = Path.Combine(_options.BasePath, $"{_options.Prefix}{id}.jsonstream");
        var events = File.ReadAllLines(path);
        var state = ConferenceDecider.Decider.InitialState(id);
        foreach (var line in events)
        {
            var parts = line.Split(':', 2);
            if (!TypeMap.TryGetValue(parts[0], out var type))
            {
                type = Type.GetType(parts[0]);
                if (type is not null) TypeMap.TryAdd(parts[0], type);
            }

            if (type is null) throw new InvalidOperationException("Could not find type");

            if (JsonSerializer.Deserialize(parts[1], type) is not IConferenceEvent @event) 
                throw new InvalidOperationException("Could not parse type");
            state = ConferenceDecider.Decider.Evolve(state, @event);
        }

        return state;
    }

    public void SaveConference(Guid id, ConferenceState state, IEnumerable<IConferenceEvent> events)
    {
        var path = Path.Combine(_options.BasePath, $"{_options.Prefix}{id}.json");
        var streamPath = Path.Combine(_options.BasePath, $"{_options.Prefix}{id}.jsonstream");
        File.WriteAllText(path, JsonSerializer.Serialize(state, SerializerOptions));
        File.AppendAllLines(streamPath, events.Select(e => $"{e.GetType().FullName}:"+JsonSerializer.Serialize((object)e)));
    }
}