using DeciderDemo.Entities.Conference.Commands;

namespace DeciderDemo.Entities.Conference.Events;

public record ConferenceStarted(Guid ConferenceId, string ConferenceName, DateOnly StartDate, DateOnly EndDate)
{
    public static ConferenceStarted From(Guid conferenceId, StartConference command) =>
        new ConferenceStarted(conferenceId, command.ConferenceName, command.StartDate, command.EndDate);
}
