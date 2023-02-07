namespace DeciderDemo.Entities.Conference.Values;

public static class WorkshopExtensions
{
    public static bool CanReserveSeatForWorkshopParticipant(this ICollection<Workshop> workshops, string workshopId, string participantId, out string failures)
    {
        failures = "";

        var workshop = workshops.SingleOrDefault(w => w.Id == workshopId);
        
        
        if (workshop is null)
        {
            failures = "Selected workshop does not exist";
            return false;
        }

        if (workshop.Capacity - workshop.Reservations.Length <= 0)
        {
            failures = "No seats available in the selected workshop";
            return false;
        }

        if (workshop.Reservations.Any(r => r.UserName == participantId))
        {
            failures = "Participant is already signed up for the selected workshop";
            return false;
        }
        
        // check if participant is signed up for another workshop at this time
        if (workshops.Any(w => w.Reservations.Any(r =>
                r.UserName == participantId &&
                w.Date == workshop.Date &&
                w.Start < workshop.End &&
                workshop.Start < w.End)))
        {
            failures = "Participant is signed up for another workshop at that time";
            return false;
        }
        
        return true;
    }
}