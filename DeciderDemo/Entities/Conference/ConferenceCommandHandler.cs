using DeciderDemo.Entities.Conference.Commands;
using DeciderDemo.Entities.Conference.Events;
using JetBrains.Annotations;

namespace DeciderDemo.Entities.Conference;

[UsedImplicitly]
public record
    ConferenceCommandHandler : EntityCommandHandler<ConferenceState, Guid, IConferenceCommand, IConferenceEvent>
{
    public ConferenceCommandHandler(FileSystemEntityDatabase<ConferenceState, Guid, IConferenceEvent> database,
        MessageBus messageBus) :
        base(database.Find,
            EntityHelpers.SaveThenPublish<Guid, ConferenceState, IConferenceEvent>(database.Save, messageBus.PublishAll), 
            ConferenceDecider.Decider)
    {
    }
}