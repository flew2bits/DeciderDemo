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

    public IActionResult OnPostApproveParticipant(string userName,
        [FromServices] ParticipantCommandHandler commandHandler)
    {
        if (string.IsNullOrEmpty(userName)) return RedirectToPage();
        commandHandler.HandleCommand(userName, ApproveParticipant.Instance);

        return RedirectToPage();
    }

    public IActionResult OnPostRemoveParticipant(string userName, string reason,
        [FromServices] ParticipantCommandHandler commandHandler)
    {
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(reason))
            return RedirectToPage();

        commandHandler.HandleCommand(userName, new RemoveParticipant(reason));

        return RedirectToPage();
    }
}