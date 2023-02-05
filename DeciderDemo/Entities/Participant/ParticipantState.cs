namespace DeciderDemo.Entities.Participant;

public record ParticipantState(ParticipantIdentity Identity, string FirstName, string LastName, bool IsApproved);