namespace DeciderDemo.Entities.Conference.Commands;

public record ReleaseWorkshopSeat(string WorkshopId, string UserName) : IWorkshopCommand;