using DeciderDemo.Entities.Conference.Commands;

namespace DeciderDemo.Entities.Conference.Events;

public record WorkshopNotAddedToConference(Guid ConferenceId, string Id, string Name, string Reason)
{
    public static WorkshopNotAddedToConference
        From(Guid conferenceId, AddWorkshopToConference command, string reason) =>
        new(conferenceId, command.Id, command.Name, reason);
}