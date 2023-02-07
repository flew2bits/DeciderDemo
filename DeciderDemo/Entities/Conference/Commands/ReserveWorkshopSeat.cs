using DeciderDemo.Entities.Conference.Commands;

public record ReserveWorkshopSeat(string Id, string UserName) : IWorkshopCommand;