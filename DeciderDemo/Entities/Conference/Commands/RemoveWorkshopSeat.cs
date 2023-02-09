namespace DeciderDemo.Entities.Conference.Commands;

public record RemoveWorkshopSeat(string WorkshopId, string UserName) : IWorkshopCommand;