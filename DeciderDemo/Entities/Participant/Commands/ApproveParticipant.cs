namespace DeciderDemo.Entities.Participant.Commands;

public record ApproveParticipant
{
    private ApproveParticipant()
    {
    }

    public static ApproveParticipant Instance => new();
}