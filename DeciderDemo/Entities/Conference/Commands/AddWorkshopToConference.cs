namespace DeciderDemo.Entities.Conference.Commands;

public record AddWorkshopToConference(string Id, string Name, DateOnly Date, TimeOnly Start, TimeOnly End,
    string Location,
    string Facilitator, int Capacity) : IWorkshopCommand
{
    public static AddWorkshopToConference From(string id, string name, DateOnly date, TimeOnly start, TimeOnly end,
        string location, string facilitator, int capacity) =>
        new(id, name, date, start, end, location, facilitator, capacity);
}