using DeciderDemo.Entities.Conference.Commands;

namespace DeciderDemo.Entities.Conference.Events;

public record WorkshopSeatNotReleased(Guid ConferenceId, string WorkshopId, string UserName, string Reason)
{
    public static WorkshopSeatNotReserved From(Guid conferenceId, ReleaseWorkshopSeat command, string reason) =>
        new(conferenceId, command.WorkshopId, command.UserName, reason);
}