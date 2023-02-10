using DeciderDemo;
using DeciderDemo.Entities;
using DeciderDemo.Entities.Conference;
using DeciderDemo.Entities.Conference.Commands;
using DeciderDemo.Entities.Participant;
using DeciderDemo.Hubs;
using Microsoft.AspNetCore.Mvc.Razor.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages()
    .AddRazorRuntimeCompilation();
builder.Services.AddSignalR();
builder.Services.AddHttpContextAccessor();
builder.Services.ConfigureEntities();
builder.Services.AddScoped<MessageBus>();

#if !NET7_0_OR_GREATER
builder.Services.AddSingleton(new Parser<Guid>(Guid.Parse, Guid.TryParse));
builder.Services.AddSingleton(new Parser<ParticipantIdentity>(ParticipantIdentity.Parse, ParticipantIdentity.TryParse));
#endif

builder.Services.AddDateOnlyTimeOnlyStringConverters();

var app = builder.Build();
app.UseStaticFiles();
// app.MapGet("/Workshops/{conferenceId:guid}", (Guid conferenceId, Loader<Guid, ConferenceState> find) =>
//     Results.Json(find(conferenceId).Workshops)
// );

app.MapPost("/Workshops/{conferenceId:guid}/remove/{id}",async (Guid conferenceId, string id, ConferenceCommandHandler commandHandler) =>
{
    await commandHandler.HandleCommand(conferenceId, RemoveWorkshopFromConference.From(id));
    return Results.Redirect("/");
});
app.MapHub<AllEventsHub>("/hubs/allEvents");
app.MapRazorPages();

app.Run();