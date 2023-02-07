using DeciderDemo.Entities.Participant.Commands;
using DeciderDemo.Entities.Participant.Events;

namespace DeciderDemo.Entities.Participant;

public static class ParticipantDecider
{
    private static object[] From(params object[] events) => events;
    
    private static object[] Decide(ParticipantState state, object command) =>
        command switch
        {
            SignupParticipant sp => From(new ParticipantSignedUp(state.Identity.UserName, sp.FirstName, sp.LastName)),
            ApproveParticipant => state.IsApproved()
                ? Array.Empty<object>()
                : From(new ParticipantApproved(state.Identity.UserName)),
            RemoveParticipant rp => From(new ParticipantRemoved(state.Identity.UserName, rp.Reason)),
            _ => Array.Empty<object>()
        };

    private static ParticipantState Evolve(ParticipantState state, object @event) =>
        @event switch
        {
            ParticipantSignedUp psu => state with { FirstName = psu.FirstName, LastName = psu.LastName },
            ParticipantApproved => state with { Status = ParticipantStatus.Approved },
            ParticipantRemoved _ => state with { Status = ParticipantStatus.Terminated },
            _ => state
        };

    private static ParticipantState InitialState(ParticipantIdentity id) => new(id, string.Empty, string.Empty, ParticipantStatus.New);

    public static readonly Decider<ParticipantIdentity, ParticipantState>
        Decider =
            new(Decide, Evolve, InitialState, s => s.Status is ParticipantStatus.Terminated, c => c is SignupParticipant);
}