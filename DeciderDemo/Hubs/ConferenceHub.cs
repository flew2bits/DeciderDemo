using DeciderDemo.Entities.Conference;
using DeciderDemo.Entities.Conference.Commands;
using Microsoft.AspNetCore.SignalR;

namespace DeciderDemo.Hubs;

public class ConferenceHub: Hub
{
    private readonly ConferenceCommandHandler _commandHandler;

    public ConferenceHub(ConferenceCommandHandler commandHandler)
    {
        _commandHandler = commandHandler;
    }

    public Task AddConference(string conferenceName, DateOnly? startDate, DateOnly? endDate)
    {
        if (!startDate.HasValue || !endDate.HasValue || string.IsNullOrEmpty(conferenceName)) return Task.CompletedTask;
        
        return _commandHandler.HandleCommand(Guid.NewGuid(), StartConference.From(conferenceName, startDate.Value, endDate.Value));
    }
}