using DeciderDemo.Entities.Conference.Commands;
using DeciderDemo.Entities.Conference.Events;

namespace DeciderDemo.Entities.Conference;

public static class ConferenceCommandHandler
{
    private static readonly
        EntityCommandHandler<ConferenceState, Guid, IConferenceCommand, IConferenceEvent>
        Handler = new(
            FileSystemConferenceDatabase.FindConference, 
            FileSystemConferenceDatabase.SaveConference,
            ConferenceDecider.Decider);

    public static (ConferenceState, IEnumerable<IConferenceEvent>) HandleCommand(Guid id, IConferenceCommand command) =>
        Handler.HandleCommand(id, command);
}