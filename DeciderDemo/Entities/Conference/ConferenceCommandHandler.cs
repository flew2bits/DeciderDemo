using DeciderDemo.Entities.Conference.Commands;
using DeciderDemo.Entities.Conference.Events;
using JetBrains.Annotations;

namespace DeciderDemo.Entities.Conference;

[UsedImplicitly]
public record ConferenceCommandHandler: EntityCommandHandler<ConferenceState, Guid, IConferenceCommand, IConferenceEvent>
{
    public ConferenceCommandHandler(FileSystemEntityDatabase<ConferenceState, Guid, IConferenceEvent> database): 
        base(database.Find, database.Save, ConferenceDecider.Decider)
    {
    }
}