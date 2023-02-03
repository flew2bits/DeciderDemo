using DeciderDemo.Entities.Conference;
using DeciderDemo.Entities.Conference.Commands;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
var app = builder.Build();

app.MapRazorPages();

app.Run();

ConferenceCommandHandler.HandleCommand(Guid.NewGuid(), StartConference.From("Test", DateOnly.Parse("2023-01-01"), DateOnly.Parse("2023-01-05"), "tander3"));