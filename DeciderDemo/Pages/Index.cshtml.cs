using DeciderDemo.Entities;
using DeciderDemo.Entities.Conference;
using DeciderDemo.Entities.Participant;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DeciderDemo.Pages;

public partial class Index : PageModel
{
    public ConferenceState[] Conferences = Array.Empty<ConferenceState>();
    public ParticipantState[] Participants = Array.Empty<ParticipantState>();

    public void OnGet([FromServices] GetAllEntities<ConferenceState> getAllConferences,
        [FromServices] GetAllEntities<ParticipantState> getAllParticipants)
    {
        Conferences = getAllConferences().ToArray();
        Participants = getAllParticipants().ToArray();
    }
}