using DeciderDemo;
using DeciderDemo.Entities;
using DeciderDemo.Entities.Conference;
using DeciderDemo.Entities.Conference.Commands;
using DeciderDemo.Entities.Participant;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

builder.Services.AddEntityDatabase(ConferenceDecider.Decider);
builder.Services.AddEntityDatabase(ParticipantDecider.Decider);

builder.Services.AddScoped<ConferenceCommandHandler>();
builder.Services.AddScoped<ParticipantCommandHandler>();

builder.Services.AddSingleton<MessageBus>();

var app = builder.Build();

app.MapGet("/Workshops/{conferenceId:guid}", (Guid conferenceId, GetEntity<ConferenceState, Guid> find) =>
    Results.Json(find(conferenceId).Workshops)
);

app.MapPost("/Workshops/{conferenceId:guid}/remove/{id}", (Guid conferenceId, string id, ConferenceCommandHandler commandHandler) =>
{
    commandHandler.HandleCommand(conferenceId, RemoveWorkshopFromConference.From(id, "tander3"));
    return Results.Redirect("/");
});

app.MapRazorPages();

app.Run();