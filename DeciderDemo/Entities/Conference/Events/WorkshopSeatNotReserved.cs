namespace DeciderDemo.Entities.Conference.Events;
public record WorkshopSeatNotReserved(Guid ConferenceId, string Id, string UserName, string Reason);