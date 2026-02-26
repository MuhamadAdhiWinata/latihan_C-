namespace SharedKernel.Abstraction.Web;

public static class ConfigExtensions
{
    public static IServiceCollection AddConfig<T>(
        this IServiceCollection services,
        IConfiguration config,
        string sectionName) where T : class, new()
    {
        // Bind to POCO
        var settings = new T();
        config.GetSection(sectionName).Bind(settings);

        // Register for IOptions<T>
        services.Configure<T>(config.GetSection(sectionName));

        // Register direct injection
        services.AddSingleton(settings);

        return services;
    }
}