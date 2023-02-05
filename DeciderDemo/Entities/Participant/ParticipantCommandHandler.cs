using DeciderDemo.Entities.Participant.Commands;
using DeciderDemo.Entities.Participant.Events;

namespace DeciderDemo.Entities.Participant;

public record
    ParticipantCommandHandler : EntityCommandHandler<ParticipantState, ParticipantIdentity, IParticipantCommand, IParticipantEvent>
{
    public ParticipantCommandHandler(FileSystemEntityDatabase<ParticipantState, ParticipantIdentity, IParticipantEvent> database) :
        base(database.Find, database.Save, ParticipantDecider.Decider)
    {
    }
}