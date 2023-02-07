using DeciderDemo.Entities;
using DeciderDemo.Entities.Conference;
using DeciderDemo.Entities.Conference.Events;
using DeciderDemo.Entities.Conference.Values;
using DeciderDemo.Entities.Participant;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DeciderDemo.Pages;

public class Workshops : PageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid ConferenceId { get; set; }

    public Workshop[] WorkshopList { get; set; } = Array.Empty<Workshop>();

    public ParticipantState[] UserNames { get; set; } = Array.Empty<ParticipantState>();

    public IActionResult OnGet([FromServices] Loader<Guid, ConferenceState> loader, [FromServices] GetAllEntities<ParticipantState> getParticipants)
    {
        try
        {
            WorkshopList = loader(ConferenceId).Workshops;
            UserNames = getParticipants().ToArray();
        }
        catch
        {
            return NotFound();
        }

        return Page();
    }

    public IActionResult OnPostReserveWorkshopSeat(string workshopId, string participantId,
        [FromServices] Loader<ParticipantIdentity, ParticipantState> loader,
        [FromServices] ConferenceCommandHandler commandHandler)
    {
        try
        {
            loader(participantId);
        }
        catch
        {
            return BadRequest(@$"Couldn't load participant with id ""{participantId}""");
        }

        var (_, events) = commandHandler.HandleCommand(ConferenceId, new ReserveWorkshopSeat(workshopId, participantId));

        if (events.SingleOrDefault() is WorkshopSeatNotReserved e)
        {
            TempData["Message"] = e.Reason;
        }

        return RedirectToPage();
    }
}