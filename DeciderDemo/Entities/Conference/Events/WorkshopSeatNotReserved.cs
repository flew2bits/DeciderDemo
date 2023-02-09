namespace DeciderDemo.Entities.Conference.Events;

public record WorkshopSeatNotReserved(Guid ConferenceId, string Id, string UserName, string Reason)
{
    public static WorkshopSeatNotReserved From(Guid conferenceId, ReserveWorkshopSeat command, string reason) =>
        new(conferenceId, command.Id, command.UserName, reason);
}