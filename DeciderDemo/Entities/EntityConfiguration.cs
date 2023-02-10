using DeciderDemo.Entities.Conference;
using DeciderDemo.Entities.Conference.Events;
using DeciderDemo.Entities.Participant;
using DeciderDemo.Entities.Participant.Events;

namespace DeciderDemo.Entities;

public static class EntityConfiguration
{
    public static IServiceCollection AddEntityDatabase<TIdentity, TState>(this IServiceCollection services,
        Action<FileSystemEntityDatabaseOptions<TIdentity, TState>> configure)
        where TState : class
    #if NET7_0_OR_GREATER
        where TIdentity : IParsable<TIdentity>
    #endif
    {
        services.Configure(configure);

        services.AddScoped<FileSystemEntityDatabase<TIdentity, TState>>()
            .AddTransient<Saver<TIdentity, TState>>(s =>
                s.GetRequiredService<FileSystemEntityDatabase<TIdentity, TState>>().Save)
            .AddTransient<Loader<TIdentity, TState>>(s =>
                s.GetRequiredService<FileSystemEntityDatabase<TIdentity, TState>>().Find)
            .AddTransient<Archiver<TIdentity>>(s =>
                s.GetRequiredService<FileSystemEntityDatabase<TIdentity, TState>>().Archive)
            .AddTransient<GetAllEntities<TState>>(s =>
                s.GetRequiredService<FileSystemEntityDatabase<TIdentity, TState>>().GetAll)
            ;

        return services;
    }

    public static IServiceCollection AddEntityDatabase<TState, TIdentity>(
        this IServiceCollection services,
        Evolver<TIdentity, TState>? evolver = null,
        Action<FileSystemEntityDatabaseOptions<TIdentity, TState>>? configure = null)
        where TState : class
#if NET7_0_OR_GREATER        
        where TIdentity : IParsable<TIdentity>
#endif
        => services.AddEntityDatabase<TIdentity, TState>(opt =>
        {
            configure?.Invoke(opt);
            opt.Evolver = evolver;
        });

    public static IServiceCollection ConfigureEntities(this IServiceCollection services) =>
        services
            .AddEntityDatabase(ConferenceDecider.Decider)
            .AddEntityDatabase<ParticipantState, ParticipantIdentity>() //ParticipantDecider.Decider)
            .AddTransient<Saver<ParticipantIdentity, ParticipantState>>(svc =>
                async (_, _, events) =>
                {
                    await svc.GetRequiredService<MessageBus>().PublishAll(events);
                    return true;
                })
            .AddTransient<Saver<Guid, ConferenceState>>(svc =>
                async (_, _, events) =>
                {
                    await svc.GetRequiredService<MessageBus>().PublishAll(events);
                    return true;
                })
            .AddScoped<ConferenceCommandHandler>()
            .AddScoped<ParticipantCommandHandler>()
            .AddTransient<IEventMetadataProvider, TimeStampEventMetadataProvider>()
            .AddScoped<IEventMetadataProvider, HttpContextEventMetadataProvider>();
}

public delegate Task<ICollection<TState>> GetAllEntities<TState>() where TState: class;