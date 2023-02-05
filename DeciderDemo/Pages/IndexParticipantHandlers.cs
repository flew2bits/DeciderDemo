using DeciderDemo.Entities.Participant;
using DeciderDemo.Entities.Participant.Commands;
using Microsoft.AspNetCore.Mvc;

namespace DeciderDemo.Pages;

public partial class Index
{
    public IActionResult OnPostAddParticipant(string userName, string firstName, string lastName, [FromServices] ParticipantCommandHandler commandHandler)
    {
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
        {
            return RedirectToPage();
        }

        commandHandler.HandleCommand(userName, new SignupParticipant(firstName, lastName));

        return RedirectToPage();
    }
}