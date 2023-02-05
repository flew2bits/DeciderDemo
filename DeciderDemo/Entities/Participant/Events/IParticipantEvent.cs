namespace DeciderDemo.Entities.Participant.Events;

public interface IParticipantEvent
{
    string ParticipantId { get; }
    public DateTime TimeStamp { get; }
}