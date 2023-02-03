using DeciderDemo.Entities;
using DeciderDemo.Entities.Conference;
using DeciderDemo.Entities.Conference.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DeciderDemo.Pages;

public class Index : PageModel
{
    public ConferenceState[] Conferences = Array.Empty<ConferenceState>();
    
    public void OnGet()
    {
        Conferences = ConferenceDatabase.GetAllConferences();
    }

    public IActionResult OnPostAddConference(string? conferenceName, DateTime? conferenceStart, DateTime? conferenceEnd)
    {
        if (!string.IsNullOrEmpty(conferenceName) && conferenceStart.HasValue && conferenceEnd.HasValue &&
            conferenceStart <= conferenceEnd)
        {
            ConferenceCommandHandler.HandleCommand(Guid.NewGuid(),
                StartConference.From(conferenceName, DateOnly.FromDateTime(conferenceStart.Value),
                    DateOnly.FromDateTime(conferenceEnd.Value), "tander3"));
        }
        
        return RedirectToPage();
    }

    public IActionResult OnPostAddWorkshopToConference(Guid? workshopConferenceId, string? workshopName,
        string? workshopId,
        DateTime? workshopDate, DateTime? workshopStart, DateTime? workshopEnd, string? workshopLocation,
        string? workshopFacilitator,
        int? workshopCapacity)
    {
        if (!workshopConferenceId.HasValue || string.IsNullOrEmpty(workshopName) || string.IsNullOrEmpty(workshopId) ||
            !workshopDate.HasValue || !workshopStart.HasValue || !workshopEnd.HasValue ||
            string.IsNullOrEmpty(workshopLocation) ||
            string.IsNullOrEmpty(workshopFacilitator) || !workshopCapacity.HasValue) return RedirectToPage();

        if (workshopCapacity.Value <= 0) return RedirectToPage();
        if (workshopStart.Value.TimeOfDay > workshopEnd.Value.TimeOfDay) return RedirectToPage();

        ConferenceCommandHandler.HandleCommand(workshopConferenceId.Value, AddWorkshopToConference.From(workshopId,
            workshopName,
            DateOnly.FromDateTime(workshopDate.Value), TimeOnly.FromDateTime(workshopStart.Value),
            TimeOnly.FromDateTime(workshopEnd.Value),
            workshopLocation, workshopFacilitator, workshopCapacity.Value, "tander3"));

        return RedirectToPage();
    }
}