using DeciderDemo.Entities.Participant.Commands;
using DeciderDemo.Entities.Participant.Events;

namespace DeciderDemo.Entities.Participant;

public static class ParticipantDecider
{
    private static IParticipantEvent[] Decide(ParticipantState state, IParticipantCommand command) =>
        command switch
        {
            SignupParticipant sp => new IParticipantEvent[]
                { new ParticipantSignedUp(state.Identity.UserName, sp.FirstName, sp.LastName, DateTime.UtcNow) },
            _ => Array.Empty<IParticipantEvent>()
        };

    private static ParticipantState Evolve(ParticipantState state, IParticipantEvent @event) =>
        @event switch
        {
            ParticipantSignedUp psu => state with { FirstName = psu.FirstName, LastName = psu.LastName },
            _ => state
        };

    private static ParticipantState InitialState(ParticipantIdentity id) => new(id, string.Empty, string.Empty);

    public static readonly Decider<ParticipantState, ParticipantIdentity, IParticipantCommand, IParticipantEvent> Decider =
        new(Decide, Evolve, InitialState, _ => false, c => c is SignupParticipant);
}