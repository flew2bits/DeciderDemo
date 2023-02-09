using DeciderDemo.Entities.Participant;
using DeciderDemo.Entities.Participant.Commands;
using Microsoft.AspNetCore.Mvc;

namespace DeciderDemo.Pages;

public partial class Index
{
    public async Task<IActionResult> OnPostAddParticipant(string userName, string firstName, string lastName, [FromServices] ParticipantCommandHandler commandHandler)
    {
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
        {
            return RedirectToPage();
        }

        var (_, events) = await commandHandler.HandleCommand(userName, new SignupParticipant(firstName, lastName));

        TempData["Events"] = this.SerializeEvents(HttpContext.StoredEvents());

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostApproveParticipant(string userName,
        [FromServices] ParticipantCommandHandler commandHandler)
    {
        if (string.IsNullOrEmpty(userName)) return RedirectToPage();
        var (_, events) = await commandHandler.HandleCommand(userName, ApproveParticipant.Instance);

        TempData["Events"] = this.SerializeEvents(HttpContext.StoredEvents());

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRemoveParticipant(string userName, string reason,
        [FromServices] ParticipantCommandHandler commandHandler)
    {
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(reason))
            return RedirectToPage();

        var (_, events) = await commandHandler.HandleCommand(userName, new RemoveParticipant(reason));

        TempData["Events"] = this.SerializeEvents(HttpContext.StoredEvents());

        return RedirectToPage();
    }
}