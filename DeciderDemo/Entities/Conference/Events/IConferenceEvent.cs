namespace DeciderDemo.Entities.Conference.Events;

public interface IConferenceEvent
{
    string User { get; }
    DateTime TimeStamp { get; }
}