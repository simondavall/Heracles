using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Heracles.Web.Components.Features.Authentication;

public static class AuthenticationServices
{
    internal static IServiceCollection AddHeraclesAuthentication(this IServiceCollection services, IConfiguration configuration) {
        services
            .AddAuthentication(options => {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect(options => {
                // todo: Make a settings object that checks that settings are complete and valid
                options.Authority = configuration["OpenIdConnect:Authority"];
                options.ClientId = configuration["OpenIdConnect:ClientId"];
                options.ClientSecret = configuration["OpenIdConnect:ClientSecret"];

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
}