namespace DeciderDemo.Entities.Participant.Events;

public record ParticipantSignedUp(string ParticipantId, string FirstName, string LastName, DateTime TimeStamp): IParticipantEvent;