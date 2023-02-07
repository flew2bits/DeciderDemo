using DeciderDemo.Entities.Conference.Commands;
using DeciderDemo.Entities.Conference.Events;
using DeciderDemo.Entities.Conference.Values;

namespace DeciderDemo.Entities.Conference;

public static class ConferenceDecider
{
    private static object[] Decide(ConferenceState state, IConferenceCommand command) =>
        command switch
        {
            StartConference sc => new object[]
                { ConferenceStarted.From(state.ConferenceId, sc.ConferenceName, sc.StartDate, sc.EndDate) },
            IWorkshopCommand workshopCommand => Decide(state, workshopCommand),

            _ => Array.Empty<object>()
        };

    private static object[] Decide(ConferenceState state, IWorkshopCommand workshopCommand) =>
        workshopCommand switch
        {
            AddWorkshopToConference add => state.CanAddWorkshopToConference(add.Id, add.Date, add.Start, add.End,
                add.Location, add.Facilitator, out var failures)
                ? new object[]
                {
                    WorkshopAddedToConference.From(state.ConferenceId, add)
                }
                : new object[]
                {
                    WorkshopNotAddedToConference.From(state.ConferenceId, add, failures),
                },
            RemoveWorkshopFromConference remove => new object[]
                { WorkshopRemovedFromConference.From(state.ConferenceId, remove.Id) },
            _ => Array.Empty<object>()
        };

    private static ConferenceState Evolve(ConferenceState state, object @event) =>
        @event switch
        {
            ConferenceStarted x => state with
            {
                ConferenceName = x.ConferenceName, StartDate = x.StartDate, EndDate = x.EndDate
            },
            WorkshopAddedToConference x => state with
            {
                Workshops = state.Workshops.Append(new Workshop(x.Id, x.WorkshopName, x.Date, x.Start, x.End,
                    x.Location, x.Facilitator, x.Capacity)).ToArray()
            },
            WorkshopRemovedFromConference x => state with
            {
                Workshops = state.Workshops.Where(w => w.Id != x.Id).ToArray()
            },
            _ => state
        };

    private static ConferenceState InitialState(Guid id) =>
        new(id, "Unset", DateOnly.MinValue, DateOnly.MaxValue, Array.Empty<Workshop>());

    private static bool IsTerminal(ConferenceState _) => false;

    private static bool IsCreator(IConferenceCommand c) => c is StartConference;

    public static readonly Decider<ConferenceState, Guid, IConferenceCommand> Decider =
        new(Decide, Evolve, InitialState, IsTerminal, IsCreator);
}