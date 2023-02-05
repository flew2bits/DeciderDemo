﻿using DeciderDemo.Entities.Participant.Commands;
using DeciderDemo.Entities.Participant.Events;

namespace DeciderDemo.Entities.Participant;

public static class ParticipantDecider
{
    private static IParticipantEvent[] Decide(ParticipantState state, IParticipantCommand command) =>
        command switch
        {
            SignupParticipant sp => new IParticipantEvent[]
                { new ParticipantSignedUp(state.Identity.UserName, sp.FirstName, sp.LastName, DateTime.UtcNow) },
            ApproveParticipant => state.IsApproved()
                ? Array.Empty<IParticipantEvent>()
                : new IParticipantEvent[] { new ParticipantApproved(state.Identity.UserName, DateTime.UtcNow) },
            RemoveParticipant rp => new IParticipantEvent[]
                { new ParticipantRemoved(state.Identity.UserName, rp.Reason, DateTime.UtcNow) },
            _ => Array.Empty<IParticipantEvent>()
        };

    private static ParticipantState Evolve(ParticipantState state, IParticipantEvent @event) =>
        @event switch
        {
            ParticipantSignedUp psu => state with { FirstName = psu.FirstName, LastName = psu.LastName },
            ParticipantApproved => state with { Status = ParticipantStatus.Approved },
            ParticipantRemoved _ => state with { Status = ParticipantStatus.Terminated },
            _ => state
        };

    private static ParticipantState InitialState(ParticipantIdentity id) => new(id, string.Empty, string.Empty, ParticipantStatus.New);

    public static readonly Decider<ParticipantState, ParticipantIdentity, IParticipantCommand, IParticipantEvent>
        Decider =
            new(Decide, Evolve, InitialState, s => s.Status is ParticipantStatus.Terminated, c => c is SignupParticipant);
}