using DeciderDemo.Entities.Conference.Commands;

namespace DeciderDemo.Entities.Conference.Events;

public record WorkshopSeatReleased(Guid ConferenceId, string WorkshopId, string UserName)
{
    public static WorkshopSeatReserved From(Guid conferenceId, ReleaseWorkshopSeat command)
        => new(conferenceId, command.WorkshopId, command.UserName);
}
