namespace DeciderDemo.Entities;

public static class EntityConfiguration
{
    public static IServiceCollection AddEntityDatabase<TState, TIdentity, TEvent>(this IServiceCollection services,
        Action<FileSystemEntityDatabaseOptions<TState, TIdentity, TEvent>> configure)
        where TState : class where TEvent : class
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
    
        where TState : class where TEvent : class
        => services.AddEntityDatabase<TState, TIdentity, TEvent>(opt =>
        {
            configure?.Invoke(opt);
            opt.Evolver = evolver;
        });
}

public delegate TState GetEntity<out TState, in TIdentity>(TIdentity identity);

public delegate TState[] GetAllEntities<out TState>();

public delegate void SaveEntity<in TState, in TIdentity, in TEvent>(TIdentity identity, TState state,
    IEnumerable<TEvent> events);