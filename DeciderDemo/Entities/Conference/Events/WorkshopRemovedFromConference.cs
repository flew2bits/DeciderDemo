namespace DeciderDemo.Entities.Conference.Events;

public record WorkshopRemovedFromConference(Guid ConferenceId, string Id)
{
    public static WorkshopRemovedFromConference From(Guid conferenceId, string id) => 
        new(conferenceId, id);
}