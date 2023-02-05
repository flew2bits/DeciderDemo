namespace DeciderDemo.Entities.Participant.Commands;

public record RemoveParticipant(string Reason) : IParticipantCommand;
