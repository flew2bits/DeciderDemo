using DeciderDemo.Entities.Conference.Commands;
using DeciderDemo.Entities.Conference.Events;

namespace DeciderDemo.Entities.Conference;

public class ConferenceCommandHandler
{
    private readonly FileSystemConferenceDatabase _database;

    public ConferenceCommandHandler(FileSystemConferenceDatabase database)
    {
        _database = database;
        _handler = new EntityCommandHandler<ConferenceState, Guid, IConferenceCommand, IConferenceEvent>(
            _database.FindConference, _database.SaveConference, ConferenceDecider.Decider);
    }

    private readonly
        EntityCommandHandler<ConferenceState, Guid, IConferenceCommand, IConferenceEvent>
        _handler; 

    public (ConferenceState, IEnumerable<IConferenceEvent>) HandleCommand(Guid id, IConferenceCommand command) =>
        _handler.HandleCommand(id, command);
}