namespace DeciderDemo.Entities.Conference.Values;

public record Workshop(string Id, string Name, DateOnly Date, TimeOnly Start, TimeOnly End, string Location, string Facilitator, int Capacity);