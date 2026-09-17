using Heracles.Application.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Heracles.Web.Components.Features.Authentication;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddHeraclesAuthentication(this IServiceCollection services, OpenIdConnectSettings settings) {
        services
            .AddAuthentication(options => {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect(options => {
                options.Authority = settings.Authority.ToString();
                options.ClientId = settings.ClientId;
                options.ClientSecret = settings.ClientSecret;
                
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.UsePkce = true;
                options.SaveTokens = true;
                options.MapInboundClaims = false;

                options.TokenValidationParameters.NameClaimType = "name";
                options.TokenValidationParameters.RoleClaimType = "role";

                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");
                options.Scope.Add("offline_access");

                options.Events.OnRemoteFailure = context => {
                    var error = context.Failure?.Data["error"]?.ToString();

                    var reason = error switch {
                        "access_denied" => "unauthorized",
                        "unauthorized_client" => "disabled",
                        _ => null
                    };

                    if (reason is not null) {
                        context.Response.Redirect($"/access-denied?reason={reason}");
                        context.HandleResponse();
                    }

                    return Task.CompletedTask;
                };
            });

        services.AddAuthorization(options => { options.FallbackPolicy = options.DefaultPolicy; });

        services.AddCascadingAuthenticationState();

        return services;
    }

    internal static WebApplication UseHeraclesAuthentication(this WebApplication app) {

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
                    return Results.SignOut(
                        properties,
                        [CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme]);
                })
            .AllowAnonymous();

        return app;
    }

    // For security: Make sure the return url is a local address
    // and not some random other (potentially dangerous) address    
    private static bool IsLocalReturnUrl(string? returnUrl) {
        return !string.IsNullOrWhiteSpace(returnUrl)
               && returnUrl.StartsWith('/')
               && !returnUrl.StartsWith("//")
               && !returnUrl.StartsWith("/\\");
    }
}