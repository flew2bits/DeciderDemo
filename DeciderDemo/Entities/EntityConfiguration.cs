using DeciderDemo.Entities.Conference;
using DeciderDemo.Entities.Conference.Events;
using DeciderDemo.Entities.Participant;
using DeciderDemo.Entities.Participant.Events;

namespace DeciderDemo.Entities;

public static class EntityConfiguration
{
    public static IServiceCollection AddEntityDatabase<TState, TIdentity>(this IServiceCollection services,
        Action<FileSystemEntityDatabaseOptions<TState, TIdentity>> configure)
        where TState : class where TIdentity : IParsable<TIdentity>
    {
        services.Configure(configure);

        services.AddScoped<FileSystemEntityDatabase<TState, TIdentity>>()
            .AddTransient<GetEntity<TState, TIdentity>>(svc =>
                svc.GetRequiredService<FileSystemEntityDatabase<TState, TIdentity>>().Find)
            .AddTransient<GetAllEntities<TState>>(svc =>
                svc.GetRequiredService<FileSystemEntityDatabase<TState, TIdentity>>().GetAll)
            .AddTransient<SaveEntity<TState, TIdentity>>(svc =>
                svc.GetRequiredService<FileSystemEntityDatabase<TState, TIdentity>>().Save);

        return services;
    }

    public static IServiceCollection AddEntityDatabase<TState, TIdentity>(
        this IServiceCollection services,
        Evolver<TState, TIdentity> evolver,
        Action<FileSystemEntityDatabaseOptions<TState, TIdentity>>? configure = null)
        where TState : class where TIdentity : IParsable<TIdentity>
        => services.AddEntityDatabase<TState, TIdentity>(opt =>
        {
            configure?.Invoke(opt);
            opt.Evolver = evolver;
        });

    public static IServiceCollection ConfigureEntities(this IServiceCollection services) =>
        services
            .AddEntityDatabase(ConferenceDecider.Decider)
            .AddEntityDatabase(ParticipantDecider.Decider)
            .AddTransient<Saver<ParticipantIdentity, ParticipantState>>(s =>
                s.GetRequiredService<
                    FileSystemEntityDatabase<ParticipantState, ParticipantIdentity>>().Save)
            .AddTransient<Saver<ParticipantIdentity, ParticipantState>>(_ =>
                (_, _, events) =>
                {
                    MessageBus.PublishAll(events);
                    return true;
                })
            .AddTransient<Saver<Guid, ConferenceState>>(s =>
                s.GetRequiredService<FileSystemEntityDatabase<ConferenceState, Guid>>().Save)
            .AddTransient<Saver<Guid, ConferenceState>>(_ =>
                (_, _, events) =>
                {
                    MessageBus.PublishAll(events);
                    return true;
                })
            .AddTransient<Loader<Guid, ConferenceState>>(s =>
                s.GetRequiredService<FileSystemEntityDatabase<ConferenceState, Guid>>().Find)
            .AddTransient<Loader<ParticipantIdentity, ParticipantState>>(s =>
                s.GetRequiredService<
                    FileSystemEntityDatabase<ParticipantState, ParticipantIdentity>>().Find)
            .AddTransient<Archiver<ParticipantIdentity>>(s =>
                s.GetRequiredService<FileSystemEntityDatabase<ParticipantState, ParticipantIdentity>>().Archive)
            .AddScoped<ConferenceCommandHandler>()
            .AddScoped<ParticipantCommandHandler>()
            .AddTransient<IEventMetadataProvider, TimeStampEventMetadataProvider>()
            .AddScoped<IEventMetadataProvider, HttpContextEventMetadataProvider>();
}

public delegate TState GetEntity<out TState, in TIdentity>(TIdentity identity);

public delegate TState[] GetAllEntities<out TState>();

public delegate bool SaveEntity<in TState, in TIdentity>(TIdentity identity, TState state,
    IEnumerable<object> events);