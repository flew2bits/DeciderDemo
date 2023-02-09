namespace DeciderDemo.Entities.Conference.Events;

public record WorkshopSeatReserved(Guid ConferenceId, string Id, string UserName)
{
    public static WorkshopSeatReserved From(Guid conferenceId, ReserveWorkshopSeat command) =>
        new(conferenceId, command.Id, command.UserName);
}
