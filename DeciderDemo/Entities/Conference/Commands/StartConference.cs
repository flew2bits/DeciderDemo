namespace DeciderDemo.Entities.Conference.Commands;

public record StartConference
    (string ConferenceName, DateOnly StartDate, DateOnly EndDate, string User) : IConferenceCommand
{
    public static StartConference From(string conferenceName, DateOnly startDate, DateOnly endDate, string user) =>
        new(conferenceName, startDate, endDate, user);
};
