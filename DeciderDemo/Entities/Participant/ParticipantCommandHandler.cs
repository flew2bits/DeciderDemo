using DeciderDemo.Entities.Participant.Commands;
using DeciderDemo.Entities.Participant.Events;

namespace DeciderDemo.Entities.Participant;

public record
    ParticipantCommandHandler(Loader<ParticipantIdentity, ParticipantState> LoadEntity,
        IEnumerable<Saver<ParticipantIdentity, ParticipantState, IParticipantEvent>> EntitySavers,
        Archiver<ParticipantIdentity> Archiver) : EntityCommandHandler<
        ParticipantState, ParticipantIdentity, IParticipantCommand,
        IParticipantEvent>(ParticipantDecider.Decider,
        LoadEntity, EntitySavers, Archiver);