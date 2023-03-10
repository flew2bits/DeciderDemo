using DeciderDemo.Entities.Conference.Commands;
using DeciderDemo.Entities.Conference.Events;
using DeciderDemo.Entities.Conference.Values;

namespace DeciderDemo.Entities.Conference;

public static class ConferenceDecider
{
    private static object[] Events(params object[] events) => events;
    private static readonly object[] NoEvents = Array.Empty<object>();

    private static object[] Decide(ConferenceState state, object command) =>
        command switch
        {
            StartConference sc => Events(ConferenceStarted.From(state.ConferenceId, sc)),
            IWorkshopCommand workshopCommand => Decide(state, workshopCommand),

            _ => NoEvents
        };

    private static object[] Decide(ConferenceState state, IWorkshopCommand workshopCommand) =>
        workshopCommand switch
        {
            AddWorkshopToConference add => state.CanAddWorkshopToConference(add.Id, add.Date, add.Start, add.End,
                add.Location, add.Facilitator, out var failures)
                ? Events(WorkshopAddedToConference.From(state.ConferenceId, add))
                : Events(WorkshopNotAddedToConference.From(state.ConferenceId, add, failures)),
            
            RemoveWorkshopFromConference remove => Events(
                WorkshopRemovedFromConference.From(state.ConferenceId, remove)),
            
            ReserveWorkshopSeat reserve => state.Workshops.CanReserveSeatForWorkshopParticipant(reserve.Id,
                reserve.UserName, out var failures)
                ? Events(WorkshopSeatReserved.From(state.ConferenceId, reserve))
                : Events(WorkshopSeatNotReserved.From(state.ConferenceId, reserve, failures)),
            
            ReleaseWorkshopSeat release => state.TryGetWorkshopById(release.WorkshopId, out var workshop)
                ? workshop!.Reservations.Any(r => r.UserName == release.UserName)
                    ? Events(WorkshopSeatReleased.From(state.ConferenceId, release))
                    : Events(WorkshopSeatNotReleased.From(state.ConferenceId, release, "Participant not registered for workshop"))
                : Events(WorkshopSeatNotReleased.From(state.ConferenceId, release, "Workshop does not exist")),
            
            RemoveWorkshopSeat remove => 
                state.TryGetWorkshopById(remove.WorkshopId, out var workshop)
                ? workshop!.Reservations.Any(r => r.UserName == remove.UserName)
                    ? Events(WorkshopSeatRemoved.From(state.ConferenceId, remove))
                    : Events(WorkshopSeatNotRemoved.From(state.ConferenceId, remove, "Participant not registered for workshop"))
                : Events(WorkshopSeatNotRemoved.From(state.ConferenceId, remove, "Workshop does not exist")),
            _ => NoEvents
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
                    x.Location, x.Facilitator, x.Capacity, Array.Empty<WorkshopReservation>())).ToArray()
            },
            WorkshopRemovedFromConference x => state with
            {
                Workshops = state.Workshops.Where(w => w.Id != x.Id).ToArray()
            },
            WorkshopSeatReserved r => state with
            {
                Workshops = state.Workshops
                    .Select(w => w.Id == r.Id
                        ? w with { Reservations = w.Reservations.Append(new WorkshopReservation(r.UserName)).ToArray() }
                        : w)
                    .ToArray()
            },
            WorkshopSeatRemoved rem => state with
            {
                Workshops = state.Workshops
                    .Select(w => w.Id == rem.WorkshopId
                        ? w with { Reservations = w.Reservations.Where(r => rem.UserName != r.UserName ).ToArray() }
                        : w)
                    .ToArray()
            },
            _ => state
        };

    private static ConferenceState InitialState(Guid id) =>
        new(id, "Unset", DateOnly.MinValue, DateOnly.MaxValue, Array.Empty<Workshop>());

    private static bool IsTerminal(ConferenceState _) => false;

    private static bool IsCreator(object c) => c is StartConference;

    public static readonly Decider<Guid, ConferenceState> Decider =
        new(Decide, Evolve, InitialState, IsTerminal, IsCreator);
}