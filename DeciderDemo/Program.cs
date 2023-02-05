using DeciderDemo.Entities;
using DeciderDemo.Entities.Conference;
using DeciderDemo.Entities.Conference.Commands;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
var app = builder.Build();

app.MapGet("/Workshops/{conferenceId:guid}", (Guid conferenceId) =>
    Results.Json(FileSystemConferenceDatabase.FindConference(conferenceId).Workshops)
);

app.MapPost("/Workshops/{conferenceId:guid}/remove/{id}", (Guid conferenceId, string id) =>
{
    ConferenceCommandHandler.HandleCommand(conferenceId, RemoveConference.From(id, "tander3"));
    return Results.Redirect("/");
});

app.MapRazorPages();

app.Run();