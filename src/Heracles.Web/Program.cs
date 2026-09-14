using DotNetEnv;
using Heracles.Application;
using Heracles.Infrastructure;
using Heracles.Web.Components;
using Heracles.Web.Components.Features.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

_ = bool.TryParse(Environment.GetEnvironmentVariable("HERACLES_LOCAL_EXECUTION"), out var isLocalExecution);
if (isLocalExecution) {
    Env
        .NoClobber()
        .TraversePath()
        .Load();

    builder.Configuration.AddEnvironmentVariables();

    builder.WebHost.UseStaticWebAssets();
}

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHeraclesAuthentication(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseRouting();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets()
    .Add(endpointBuilder => endpointBuilder.Metadata.Add(new AllowAnonymousAttribute()));

app
    .MapGet(
        "/authentication/login",
        (string? returnUrl) => {
            var properties = new AuthenticationProperties { RedirectUri = IsLocalReturnUrl(returnUrl) ? returnUrl! : "/" };
            return Results.Challenge(properties, [OpenIdConnectDefaults.AuthenticationScheme]);
        })
    .AllowAnonymous();

app
    .MapGet(
        "/authentication/logout",
        () => {
            var properties = new AuthenticationProperties { RedirectUri = "/" };
            return Results.SignOut(properties, [CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme]);
        })
    .AllowAnonymous();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
return;

// For security: Make sure the return url is a local address
// and not some random other (potentially dangerous) address
static bool IsLocalReturnUrl(string? returnUrl)
{
    if (string.IsNullOrWhiteSpace(returnUrl))
        return false;

    return returnUrl.StartsWith('/')
           && !returnUrl.StartsWith("//")
           && !returnUrl.StartsWith("/\\");
}