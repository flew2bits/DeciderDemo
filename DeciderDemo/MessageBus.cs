using System.Text.Json;
using DateOnlyTimeOnly.AspNet.Converters;
using DeciderDemo.Entities;
using DeciderDemo.Entities.Conference;
using DeciderDemo.Entities.Conference.Commands;
using DeciderDemo.Entities.Participant.Commands;
using DeciderDemo.Entities.Participant.Events;
using DeciderDemo.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace DeciderDemo;

public class MessageBus
{
    private readonly GetAllEntities<ConferenceState> _getAllConferences;
    private readonly ConferenceCommandHandler _conferenceCommandHandler;
    private readonly IHttpContextAccessor _contextAccessor;

    public static readonly JsonSerializerOptions SerializerOptions = new()
        { Converters = { new DateOnlyJsonConverter(), new TimeOnlyJsonConverter() } };

    public MessageBus(GetAllEntities<ConferenceState> getAllConferences, 
        ConferenceCommandHandler conferenceCommandHandler,
        IHttpContextAccessor contextAccessor)
    {
        _getAllConferences = getAllConferences;
        _conferenceCommandHandler = conferenceCommandHandler;
        _contextAccessor = contextAccessor;
    }

    public async Task Publish(object @event)
    {
        Console.WriteLine($"Event of type {@event.GetType().Name} was published");
        Console.WriteLine($"\t{JsonSerializer.Serialize(@event, SerializerOptions)}");

        var httpContext = _contextAccessor.HttpContext;
        httpContext?.AddStoredEvent(@event);
        
        if (@event is ParticipantRemoved remove)
        {
            var conferences = await _getAllConferences();
            var workshops = conferences.SelectMany(c =>
                c.Workshops.Where(w => w.Reservations.Any(r => r.UserName == remove.ParticipantId))
                    .Select(w => new { c.ConferenceId, WorkshopId = w.Id }));
            foreach (var workshop in workshops)
            {
                await _conferenceCommandHandler.HandleCommand(workshop.ConferenceId,
                    new RemoveWorkshopSeat(workshop.WorkshopId, remove.ParticipantId));
            }
        }
    }

    public async Task PublishAll(IEnumerable<object> events)
    {
        foreach (var @event in events) await Publish(@event);
    }
}