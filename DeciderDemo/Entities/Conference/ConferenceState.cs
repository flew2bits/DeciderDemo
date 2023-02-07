using DeciderDemo.Entities.Conference.Values;

namespace DeciderDemo.Entities.Conference;

public record ConferenceState(Guid ConferenceId, string ConferenceName, DateOnly StartDate, DateOnly EndDate, 
    Workshop[] Workshops);
