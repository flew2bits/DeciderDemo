using DeciderDemo.Entities.Conference.Commands;
using DeciderDemo.Entities.Conference.Events;
using JetBrains.Annotations;

namespace DeciderDemo.Entities.Conference;

[UsedImplicitly]
public record
    ConferenceCommandHandler(Loader<Guid, ConferenceState> LoadEntity,
        IEnumerable<Saver<Guid, ConferenceState, IConferenceEvent>> EntitySavers) :
        EntityCommandHandler<ConferenceState, Guid, IConferenceCommand, IConferenceEvent>(ConferenceDecider.Decider,
            LoadEntity, EntitySavers);