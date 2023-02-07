namespace DeciderDemo.Entities.Conference.Events;

public record WorkshopSeatReserved(Guid ConferenceId, string Id, string UserName);