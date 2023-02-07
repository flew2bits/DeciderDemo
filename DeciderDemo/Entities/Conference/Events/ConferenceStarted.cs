namespace DeciderDemo.Entities.Conference.Events;

public record ConferenceStarted(Guid ConferenceId, string ConferenceName, DateOnly StartDate, DateOnly EndDate)
{
    public static ConferenceStarted From(Guid conferenceId, string conferenceName, DateOnly startDate, DateOnly endDate) =>
        new ConferenceStarted(conferenceId, conferenceName, startDate, endDate);
}
