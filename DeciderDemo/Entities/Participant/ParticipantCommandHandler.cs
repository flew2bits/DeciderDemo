using DeciderDemo.Entities.Participant.Commands;
using DeciderDemo.Entities.Participant.Events;

namespace DeciderDemo.Entities.Participant;

public record
    ParticipantCommandHandler(Loader<ParticipantIdentity, ParticipantState> LoadEntity,
        IEnumerable<Saver<ParticipantIdentity, ParticipantState>> EntitySavers,
        Archiver<ParticipantIdentity> Archiver) : EntityCommandHandler<
        ParticipantState, ParticipantIdentity>(ParticipantDecider.Decider,
        LoadEntity, EntitySavers, Archiver);