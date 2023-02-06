using DeciderDemo.Entities.Conference.Commands;
using DeciderDemo.Entities.Conference.Events;
using JetBrains.Annotations;

namespace DeciderDemo.Entities.Conference;

[UsedImplicitly]
public record
    ConferenceCommandHandler : EntityCommandHandler<ConferenceState, Guid, IConferenceCommand, IConferenceEvent>
{
    public ConferenceCommandHandler(FileSystemEntityDatabase<ConferenceState, Guid, IConferenceEvent> database) :
        base( ConferenceDecider.Decider, database.Find,
            EntityHelpers.SaveThenPublish<Guid, ConferenceState, IConferenceEvent>(database.Save, MessageBus.PublishAll))
    {
    }
}