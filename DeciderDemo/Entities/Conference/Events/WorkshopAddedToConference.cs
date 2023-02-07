using DeciderDemo.Entities.Conference.Commands;

namespace DeciderDemo.Entities.Conference.Events;

public record WorkshopAddedToConference(Guid ConferenceId, string Id, string WorkshopName, DateOnly Date,
    TimeOnly Start, TimeOnly End, string Facilitator, string Location, int Capacity)
{
    public static WorkshopAddedToConference From(Guid conferenceId, AddWorkshopToConference command) =>
        new(conferenceId, command.Id, command.Name, command.Date, command.Start, command.End, command.Facilitator,
            command.Location, command.Capacity);
}