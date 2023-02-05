namespace DeciderDemo.Entities.Conference.Events;

public record WorkshopRemovedFromConference(Guid ConferenceId, string Id, string User, DateTime TimeStamp) : IConferenceEvent
{
    public static WorkshopRemovedFromConference From(Guid conferenceId, string id, string user) => 
        new(conferenceId, id, user, DateTime.UtcNow);
}