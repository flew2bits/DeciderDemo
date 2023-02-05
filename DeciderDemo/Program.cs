using DeciderDemo.Entities;
using DeciderDemo.Entities.Conference;
using DeciderDemo.Entities.Conference.Commands;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

builder.Services.Configure<FileSystemConferenceDatabaseOptions>(opt =>
{
    opt.Prefix = "Conference_";
    opt.BasePath = "ConferenceDB";
});
builder.Services.AddSingleton<FileSystemConferenceDatabase>();

var app = builder.Build();

app.MapGet("/Workshops/{conferenceId:guid}", (Guid conferenceId, FileSystemConferenceDatabase database) =>
    Results.Json(database.FindConference(conferenceId).Workshops)
);

app.MapPost("/Workshops/{conferenceId:guid}/remove/{id}", (Guid conferenceId, string id, ConferenceCommandHandler commandHandler) =>
{
    commandHandler.HandleCommand(conferenceId, RemoveConference.From(id, "tander3"));
    return Results.Redirect("/");
});

app.MapRazorPages();

app.Run();