namespace DeciderDemo.Entities.Participant.Events;

public record ParticipantApproved(string ParticipantId, DateTime TimeStamp) : IParticipantEvent;