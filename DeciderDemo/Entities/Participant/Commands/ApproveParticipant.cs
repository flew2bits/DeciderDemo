namespace DeciderDemo.Entities.Participant.Commands;

public record ApproveParticipant : IParticipantCommand
{
    private ApproveParticipant()
    {
    }

    public static ApproveParticipant Instance => new();
}