using DeciderDemo.Entities.Conference;

namespace DeciderDemo.Entities;

public static class DictionaryConferenceDatabase
{
    private static readonly Dictionary<Guid, ConferenceState> Database = new();

    public static ConferenceState[] GetAllConferences() => Database.Values.ToArray();

    public static ConferenceState FindConference(Guid id) => Database.TryGetValue(id, out var state)
        ? state
        : throw new InvalidOperationException("conference not found");

    public static void SaveConference(Guid id, ConferenceState state) => Database.AddOrUpdate(id, state);
}

public static class DictionaryExtensions {
    public static bool AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
        var result = dictionary.TryAdd(key, value);
        if (!result) dictionary[key] = value;
        return result;
    }
}