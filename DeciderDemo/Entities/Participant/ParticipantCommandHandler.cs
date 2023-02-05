using DeciderDemo.Entities.Participant.Commands;
using DeciderDemo.Entities.Participant.Events;

namespace DeciderDemo.Entities.Participant;

public record
    ParticipantCommandHandler : EntityCommandHandler<ParticipantState, ParticipantIdentity, IParticipantCommand, IParticipantEvent>
{
    public ParticipantCommandHandler(FileSystemEntityDatabase<ParticipantState, ParticipantIdentity, IParticipantEvent> database, MessageBus messageBus) :
        base(database.Find, (id, state, events) =>
        {
            var e = events as IParticipantEvent[] ?? events.ToArray();
            database.Save(id, state, e);
            messageBus.PublishAll(e);
        }, ParticipantDecider.Decider)
    {
    }
}