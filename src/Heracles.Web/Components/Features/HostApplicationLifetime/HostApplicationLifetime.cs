using Serilog;

namespace Heracles.Web.Components.Features.HostApplicationLifetime;

public static class HostApplicationLifetime
{
    public static void UseHostApplicationLifetime(this WebApplication app) {
        var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

        lifetime.ApplicationStarted.Register(() => {
            Log.Information("{Application} is starting up...", app.Environment.ApplicationName);
            Log.Information("Environment: {Environment}", app.Environment.EnvironmentName);
            Log.Information("App version: {Version}", typeof(Program).Assembly.GetName().Version);
        });

        lifetime.ApplicationStopping.Register(() => { Log.Warning("{Application} is shutting down...", app.Environment.ApplicationName); });

        lifetime.ApplicationStopped.Register(() => {
            Log.Warning("{Application} has stopped", app.Environment.ApplicationName);
            Log.CloseAndFlush();
        });
    }
}