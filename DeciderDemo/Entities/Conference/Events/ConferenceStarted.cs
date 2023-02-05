namespace DeciderDemo.Entities.Conference.Events;

public record ConferenceStarted(Guid ConferenceId, string ConferenceName, DateOnly StartDate, DateOnly EndDate, string User,
    DateTime TimeStamp): IConferenceEvent
{
    public static ConferenceStarted From(Guid conferenceId, string conferenceName, DateOnly startDate, DateOnly endDate, string user) =>
        new ConferenceStarted(conferenceId, conferenceName, startDate, endDate, user, DateTime.UtcNow);
}
