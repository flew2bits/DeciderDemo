namespace DeciderDemo.Entities.Conference;

public static class ConferenceStateExtensions
{
    public static bool LocationAvailableBetweenTimes(this ConferenceState state, string location, DateOnly date,
        TimeOnly start, TimeOnly end) =>
        state.Workshops.Where(w => w.Date == date && w.Location == location)
            .All(w => !(w.End > start && w.Start < end));

    public static bool FacilitatorAvailableBetweenTimes(this ConferenceState state, string facilitator, DateOnly date,
        TimeOnly start, TimeOnly end) =>
        state.Workshops.Where(w => w.Date == date && w.Facilitator == facilitator)
            .All(w => !(w.End > start && w.Start < end));
    
    public static bool CanAddWorkshopToConference(this ConferenceState state, string id, DateOnly date, TimeOnly start,
        TimeOnly end, string location, string facilitator, out string failureReason)
    {
        failureReason = string.Empty;

        var failures = new List<string>();
        
        if (date < state.StartDate || date > state.EndDate) failures.Add("Date of workshop is not during conference");
        if (state.Workshops.Any(w => w.Id == id)) failures.Add("Workshop with id already exists");
        if (!state.LocationAvailableBetweenTimes(location, date, start, end)) 
            failures.Add("Location is not available at selected date and time");
        if (!state.FacilitatorAvailableBetweenTimes(facilitator, date, start, end))
            failures.Add("Facilitator is not available at selected date and time");

        failureReason = string.Join(", ", failures);
        
        return !failures.Any();
    }
}