using DeciderDemo.Entities.Conference.Commands;
using DeciderDemo.Entities.Conference.Events;
using JetBrains.Annotations;

namespace DeciderDemo.Entities.Conference;

[UsedImplicitly]
public record ConferenceCommandHandler: EntityCommandHandler<ConferenceState, Guid, IConferenceCommand, IConferenceEvent>
{
    public ConferenceCommandHandler(FileSystemConferenceDatabase database): 
        base(database.FindConference, database.SaveConference, ConferenceDecider.Decider)
    {
    }
}