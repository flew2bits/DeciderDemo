using DeciderDemo.Entities.Participant.Commands;
using DeciderDemo.Entities.Participant.Events;

namespace DeciderDemo.Entities.Participant;

public record
    ParticipantCommandHandler : EntityCommandHandler<ParticipantState, ParticipantIdentity, IParticipantCommand,
        IParticipantEvent>
{
    public ParticipantCommandHandler(
        FileSystemEntityDatabase<ParticipantState, ParticipantIdentity, IParticipantEvent> database,
        MessageBus messageBus) :
        base(database.Find,
            // I'm not sure why the types need to be specified
            EntityHelpers.SaveThenPublish<ParticipantIdentity, ParticipantState, IParticipantEvent>
                (database.Save, messageBus.PublishAll),
            ParticipantDecider.Decider)
    {
    }
}