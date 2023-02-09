using DeciderDemo.Entities.Conference.Commands;

namespace DeciderDemo.Entities.Conference.Events;

public record WorkshopSeatNotRemoved(Guid ConferenceId, string WorkshopId, string UserName, string Reason)
{
    public static WorkshopSeatNotRemoved From(Guid conferenceId, RemoveWorkshopSeat command, string reason) =>
        new(conferenceId, command.WorkshopId, command.UserName, reason);
}