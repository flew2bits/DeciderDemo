namespace DeciderDemo.Entities.Conference.Commands;

public record RemoveWorkshopFromConference(string Id, string User) : IWorkshopCommand
{
    public static RemoveWorkshopFromConference From(string id, string user) => new(id, user);
}