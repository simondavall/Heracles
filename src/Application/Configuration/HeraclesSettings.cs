#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Heracles.Application.Configuration;

public sealed record HeraclesSettings(
    DatabaseSettings DatabaseSettings,
    OpenIdConnectSettings OpenIdConnect,
    DataProtectionSettings DataProtection)
{
    public static HeraclesSettings Create(IConfiguration configuration) {
        var errors = new List<string>();

        var openIdConnectAuthority = GetAbsoluteUri(configuration, "OpenIdConnect:Authority", errors);
        var openIdConnectClientId = GetRequiredString(configuration, "OpenIdConnect:ClientId", errors);
        var openIdConnectClientSecret = GetRequiredString(configuration, "OpenIdConnect:ClientSecret", errors);

        var heraclesDb = GetRequiredConnectionString(configuration, "HeraclesDb", errors);
        var heraclesAuthDb = GetRequiredConnectionString(configuration, "HeraclesAuthDb", errors);

        var dataProtectionKeyPath = GetRequiredString(configuration, "DataProtection:KeyPath", errors);
        var dataProtectionCertificatePath = GetRequiredString(configuration, "DataProtection:CertificatePath", errors);
        var dataProtectionCertificatePassword = GetRequiredString(configuration, "DataProtection:CertificatePassword", errors);

        if (errors.Count > 0)
            throw new InvalidOperationException(
                "Invalid Heracles configuration:"
                + Environment.NewLine
                + string.Join(Environment.NewLine, errors.Select(error => $" - {error}")));
        
        return new HeraclesSettings(
            new DatabaseSettings(heraclesDb!, heraclesAuthDb!),
            new OpenIdConnectSettings(openIdConnectAuthority!, openIdConnectClientId!, openIdConnectClientSecret!),
            new DataProtectionSettings(dataProtectionKeyPath!, dataProtectionCertificatePath!, dataProtectionCertificatePassword!));
    }

    private static string? GetRequiredString(IConfiguration configuration, string key, ICollection<string> errors) {
        var value = configuration[key];

        if (!string.IsNullOrWhiteSpace(value))
            return value;

        errors.Add($"{key} is required.");
        return null;
    }

    private static string? GetRequiredConnectionString(IConfiguration configuration, string name, List<string> errors) {
        var value = configuration.GetConnectionString(name);

        if (!string.IsNullOrWhiteSpace(value))
            return value;

        errors.Add($"ConnectionStrings:{name} is required.");
        return null;
    }

    private static Uri? GetAbsoluteUri(IConfiguration configuration, string key, ICollection<string> errors) {
        var value = configuration[key];

        if (string.IsNullOrWhiteSpace(value)) {
            errors.Add($"{key} is required.");
            return null;
        }

        if (Uri.TryCreate(value, UriKind.Absolute, out var uri))
            return uri;

        errors.Add($"{key} must be a valid absolute URI.");
        return null;
    }
}

public sealed record OpenIdConnectSettings(
    Uri Authority,
    string ClientId,
    string ClientSecret);

public sealed record DatabaseSettings(
    string HeraclesDb,
    string HeraclesAuthDb);

public sealed record DataProtectionSettings(
    string KeyPath,
    string CertificatePath,
    string CertificatePassword);