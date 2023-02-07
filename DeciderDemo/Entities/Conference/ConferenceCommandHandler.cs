using DeciderDemo.Entities.Conference.Commands;
using DeciderDemo.Entities.Conference.Events;
using JetBrains.Annotations;

namespace DeciderDemo.Entities.Conference;

[UsedImplicitly]
public record
    ConferenceCommandHandler(Loader<Guid, ConferenceState> LoadEntity,
        IEnumerable<Saver<Guid, ConferenceState>> EntitySavers) :
        EntityCommandHandler<ConferenceState, Guid, IConferenceCommand>(ConferenceDecider.Decider,
            LoadEntity, EntitySavers);