namespace DeciderDemo.Entities.Conference;

public static class ConferenceStateExtensions
{
    public static bool LocationAvailableBetweenTimes(this ConferenceState state, string location, DateOnly date,
        TimeOnly start, TimeOnly end) =>
        state.Workshops.Where(w => w.Date == date && w.Location == location)
            .All(w => !(w.End >= start && w.Start <= end));
}