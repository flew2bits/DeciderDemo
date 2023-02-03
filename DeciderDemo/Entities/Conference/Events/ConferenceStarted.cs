namespace DeciderDemo.Entities.Conference.Events;

public record ConferenceStarted(string ConferenceName, DateOnly StartDate, DateOnly EndDate, string User,
    DateTime TimeStamp): IConferenceEvent
{
    public static ConferenceStarted From(string conferenceName, DateOnly startDate, DateOnly endDate, string user) =>
        new ConferenceStarted(conferenceName, startDate, endDate, user, DateTime.UtcNow);
}
