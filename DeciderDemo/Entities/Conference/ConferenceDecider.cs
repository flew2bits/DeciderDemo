using DeciderDemo.Entities.Conference.Commands;
using DeciderDemo.Entities.Conference.Events;
using DeciderDemo.Entities.Conference.Values;

namespace DeciderDemo.Entities.Conference;

public static class ConferenceDecider
{
    private static IConferenceEvent[] Decide(ConferenceState state, IConferenceCommand command) =>
        command switch
        {
            StartConference sc => new IConferenceEvent[] { ConferenceStarted.From(sc.ConferenceName, sc.StartDate, sc.EndDate, sc.User) },
            IWorkshopCommand workshopCommand => Decide(state, workshopCommand), 

            _ => Array.Empty<IConferenceEvent>()
        };

    private static IConferenceEvent[] Decide(ConferenceState state, IWorkshopCommand workshopCommand) =>
        workshopCommand switch
        {
            AddWorkshopToConference add => state.LocationAvailableBetweenTimes(add.Location, add.Date, add.Start, add.End)
                ? new IConferenceEvent[]
                {
                    WorkshopAddedToConference.From(state.ConferenceId, add)
                }
                : new IConferenceEvent[]
                {
                    WorkshopNotAddedToConference.From(state.ConferenceId, add, "Location not available at selected times"), 
                },
            _ => Array.Empty<IConferenceEvent>()
        };
    
    private static ConferenceState Evolve(ConferenceState state, IConferenceEvent @event) =>
        @event switch
        {
            ConferenceStarted x => state with
            {
                ConferenceName = x.ConferenceName, StartDate = x.StartDate, EndDate = x.EndDate
            },
            WorkshopAddedToConference x => state with
            {
                Workshops = state.Workshops.Append(new Workshop(x.Id, x.WorkshopName, x.Date, x.Start, x.End, x.Location, x.Facilitator, x.Capacity)).ToArray()
            },
            _ => state
        };

    private static ConferenceState InitialState(Guid id) => new (id, "Unset", DateOnly.MinValue, DateOnly.MaxValue, Array.Empty<Workshop>());

    private static bool IsTerminal(ConferenceState _) => false;

    private static bool IsCreator(IConferenceCommand c) => c is StartConference;
    
    public static readonly Decider<ConferenceState, Guid, IConferenceCommand, IConferenceEvent> Decider =
        new(Decide, Evolve, InitialState, IsTerminal, IsCreator);
}