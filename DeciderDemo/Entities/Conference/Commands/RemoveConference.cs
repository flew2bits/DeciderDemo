namespace DeciderDemo.Entities.Conference.Commands;

public record RemoveConference(string Id, string User) : IWorkshopCommand
{
    public static RemoveConference From(string id, string user) => new(id, user);
}