namespace DeciderDemo.Entities.Participant;

public enum ParticipantStatus
{
    New,
    Approved,
    Unapproved,
    Terminated
}

public record ParticipantState(ParticipantIdentity Identity, string FirstName, string LastName, ParticipantStatus Status);

public static class ParticipantStateExtensions
{
    public static bool IsApproved(this ParticipantState state) => state.Status is ParticipantStatus.Approved;
}