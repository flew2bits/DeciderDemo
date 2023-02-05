namespace DeciderDemo.Entities.Participant.Events;

public record ParticipantRemoved(string ParticipantId, string Reason, DateTime TimeStamp) : IParticipantEvent;