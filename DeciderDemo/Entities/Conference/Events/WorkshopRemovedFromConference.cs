using DeciderDemo.Entities.Conference.Commands;

namespace DeciderDemo.Entities.Conference.Events;

public record WorkshopRemovedFromConference(Guid ConferenceId, string Id)
{
    public static WorkshopRemovedFromConference From(Guid conferenceId, RemoveWorkshopFromConference command) => 
        new(conferenceId, command.Id);
}