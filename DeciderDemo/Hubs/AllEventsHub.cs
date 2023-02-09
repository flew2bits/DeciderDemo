using Microsoft.AspNetCore.SignalR;

namespace DeciderDemo.Hubs;

public interface IAllEventsHub
{
    Task SendEvent(object @event);
}

public class AllEventsHub : Hub<IAllEventsHub>
{
    
}