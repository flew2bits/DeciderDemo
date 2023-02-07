using DeciderDemo.Entities.Conference;
using DeciderDemo.Entities.Conference.Events;
using DeciderDemo.Entities.Participant;
using DeciderDemo.Entities.Participant.Events;

namespace DeciderDemo.Entities;

public static class EntityConfiguration
{
    public static IServiceCollection AddEntityDatabase<TState, TIdentity, TEvent>(this IServiceCollection services,
        Action<FileSystemEntityDatabaseOptions<TState, TIdentity, TEvent>> configure)
        where TState : class where TEvent : class where TIdentity : IParsable<TIdentity>
    {
        var options = new FileSystemEntityDatabaseOptions<TState, TIdentity, TEvent>();
        configure(options);

        services.AddSingleton(new FileSystemEntityDatabase<TState, TIdentity, TEvent>(options))
            .AddTransient<GetEntity<TState, TIdentity>>(svc =>
                svc.GetRequiredService<FileSystemEntityDatabase<TState, TIdentity, TEvent>>().Find)
            .AddTransient<GetAllEntities<TState>>(svc =>
                svc.GetRequiredService<FileSystemEntityDatabase<TState, TIdentity, TEvent>>().GetAll)
            .AddTransient<SaveEntity<TState, TIdentity, TEvent>>(svc =>
                svc.GetRequiredService<FileSystemEntityDatabase<TState, TIdentity, TEvent>>().Save);

        return services;
    }

    public static IServiceCollection AddEntityDatabase<TState, TIdentity, TEvent>(
        this IServiceCollection services,
        Evolver<TState, TIdentity, TEvent> evolver,
        Action<FileSystemEntityDatabaseOptions<TState, TIdentity, TEvent>>? configure = null)
        where TState : class where TEvent : class where TIdentity : IParsable<TIdentity>
        => services.AddEntityDatabase<TState, TIdentity, TEvent>(opt =>
        {
            configure?.Invoke(opt);
            opt.Evolver = evolver;
        });

    public static IServiceCollection ConfigureEntities(this IServiceCollection services) =>
        services
            .AddEntityDatabase(ConferenceDecider.Decider)
            .AddEntityDatabase(ParticipantDecider.Decider)
            .AddTransient<Saver<ParticipantIdentity, ParticipantState, IParticipantEvent>>(s =>
                s.GetRequiredService<
                    FileSystemEntityDatabase<ParticipantState, ParticipantIdentity, IParticipantEvent>>().Save)
            .AddTransient<Saver<ParticipantIdentity, ParticipantState, IParticipantEvent>>(_ =>
                (_, _, events) =>
                {
                    MessageBus.PublishAll(events);
                    return true;
                })
            .AddTransient<Saver<Guid, ConferenceState, IConferenceEvent>>(s =>
                s.GetRequiredService<FileSystemEntityDatabase<ConferenceState, Guid, IConferenceEvent>>().Save)
            .AddTransient<Saver<Guid, ConferenceState, IConferenceEvent>>(_ =>
                (_, _, events) =>
                {
                    MessageBus.PublishAll(events);
                    return true;
                })
            .AddTransient<Loader<Guid, ConferenceState>>(s =>
                s.GetRequiredService<FileSystemEntityDatabase<ConferenceState, Guid, IConferenceEvent>>().Find)
            .AddTransient<Loader<ParticipantIdentity, ParticipantState>>(s =>
                s.GetRequiredService<
                    FileSystemEntityDatabase<ParticipantState, ParticipantIdentity, IParticipantEvent>>().Find)
            .AddTransient<Archiver<ParticipantIdentity>>(s => s.GetRequiredService<FileSystemEntityDatabase<ParticipantState, ParticipantIdentity, IParticipantEvent>>().Archive)
            .AddScoped<ConferenceCommandHandler>()
            .AddScoped<ParticipantCommandHandler>();
}

public delegate TState GetEntity<out TState, in TIdentity>(TIdentity identity);

public delegate TState[] GetAllEntities<out TState>();

public delegate bool SaveEntity<in TState, in TIdentity, in TEvent>(TIdentity identity, TState state,
    IEnumerable<TEvent> events);