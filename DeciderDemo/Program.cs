using DeciderDemo;
using DeciderDemo.Entities;
using DeciderDemo.Entities.Conference;
using DeciderDemo.Entities.Conference.Commands;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddHttpContextAccessor();
builder.Services.ConfigureEntities();

var app = builder.Build();

app.MapGet("/Workshops/{conferenceId:guid}", (Guid conferenceId, Loader<Guid, ConferenceState> find) =>
    Results.Json(find(conferenceId).Workshops)
);

app.MapPost("/Workshops/{conferenceId:guid}/remove/{id}", (Guid conferenceId, string id, ConferenceCommandHandler commandHandler) =>
{
    commandHandler.HandleCommand(conferenceId, RemoveWorkshopFromConference.From(id));
    return Results.Redirect("/");
});

app.MapRazorPages();

app.Run();