using DeciderDemo.Entities.Conference.Commands;

namespace DeciderDemo.Entities.Conference.Events;

public record WorkshopSeatRemoved(Guid ConferenceId, string WorkshopId, string UserName)
{
    public static WorkshopSeatRemoved From(Guid conferenceId, RemoveWorkshopSeat command) =>
        new(conferenceId, command.WorkshopId, command.UserName);
}