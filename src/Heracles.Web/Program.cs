using DotNetEnv;
using Heracles.Application;
using Heracles.Application.Configuration;
using Heracles.Infrastructure;
using Heracles.Web.Components;
using Heracles.Web.Components.Features.Authentication;
using Heracles.Web.Components.Features.DataProtection;
using Heracles.Web.Components.Features.UserState;
using Microsoft.AspNetCore.Authorization;
using MudBlazor.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

_ = bool.TryParse(Environment.GetEnvironmentVariable("HERACLES_LOCAL_EXECUTION"), out var isLocalExecution);
if (isLocalExecution) {
    Env.NoClobber()
        .TraversePath()
        .Load();

    builder.Configuration.AddEnvironmentVariables();

    builder.WebHost.UseStaticWebAssets();
}

builder.Services.AddSerilog((services, configuration) => configuration
    .ReadFrom.Configuration(builder.Configuration)
    .ReadFrom.Services(services));

var settings = HeraclesSettings.Create(builder.Configuration);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddHeraclesDataProtection(settings.DataProtection);
builder.Services.AddHeraclesAuthentication(settings.OpenIdConnect);

builder.Services.AddScoped<UserStateService>();

builder.Services.AddMudServices();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets()
    .Add(endpointBuilder => endpointBuilder.Metadata.Add(new AllowAnonymousAttribute()));

app.UseHeraclesAuthentication();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
