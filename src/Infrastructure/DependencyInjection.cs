using DotNetEnv;

namespace DrPoro.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Loads the .env file from the project root and binds the "Discord" configuration
    /// section to <see cref="DiscordSettings"/>, making it available via
    /// <see cref="Microsoft.Extensions.Options.IOptions{TOptions}"/>.
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Reload the configuration so it picks up the variables just loaded from .env.
        if (configuration is IConfigurationRoot root)
        {
            root.Reload();
        }

        // Bind the "Discord" section -> DiscordSettings and register with IOptions<>.
        services.Configure<DiscordSettings>(configuration.GetSection("Discord"));

        Console.WriteLine(configuration.GetSection("Discord").ToString());
        return services;
    }
}
