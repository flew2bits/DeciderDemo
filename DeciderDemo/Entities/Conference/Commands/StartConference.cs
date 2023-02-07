namespace DeciderDemo.Entities.Conference.Commands;

public record StartConference
    (string ConferenceName, DateOnly StartDate, DateOnly EndDate) : IConferenceCommand
{
    public static StartConference From(string conferenceName, DateOnly startDate, DateOnly endDate) =>
        new(conferenceName, startDate, endDate);
};
