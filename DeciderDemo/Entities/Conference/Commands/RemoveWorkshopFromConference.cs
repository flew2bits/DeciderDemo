namespace DeciderDemo.Entities.Conference.Commands;

public record RemoveWorkshopFromConference(string Id) : IWorkshopCommand
{
    public static RemoveWorkshopFromConference From(string id) => new(id);
}