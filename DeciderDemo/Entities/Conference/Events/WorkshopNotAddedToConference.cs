using DeciderDemo.Entities.Conference.Commands;

namespace DeciderDemo.Entities.Conference.Events;

public record WorkshopNotAddedToConference(Guid ConferenceId, string Id, string Name, string Reason, string User,
    DateTime TimeStamp) : IConferenceEvent
{
    public static WorkshopNotAddedToConference
        From(Guid conferenceId, AddWorkshopToConference command, string reason) =>
        new(conferenceId, command.Id, command.Name, reason, command.User, DateTime.UtcNow);
}