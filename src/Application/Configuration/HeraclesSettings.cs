#nullable enable
using Microsoft.Extensions.Configuration;

namespace Heracles.Application.Configuration;

public sealed record HeraclesSettings(
    DatabaseSettings DatabaseSettings,
    OpenIdConnectSettings OpenIdConnect,
    DataProtectionSettings DataProtection,
    MapboxSettings Mapbox, ImportSettings Import)
{
    public static HeraclesSettings Create(IConfiguration configuration) {
        var errors = new List<string>();

        var openIdConnectAuthority = GetAbsoluteUri(configuration, "OpenIdConnect:Authority", errors);
        var openIdConnectClientId = GetRequiredString(configuration, "OpenIdConnect:ClientId", errors);
        var openIdConnectClientSecret = GetRequiredString(configuration, "OpenIdConnect:ClientSecret", errors);
        
        var databasePath = GetAbsoluteFilePath(configuration, "Database:DatabasePath", errors);
        
        var dataProtectionKeyPath = GetRequiredString(configuration, "DataProtection:KeyPath", errors);
        var dataProtectionCertificatePath = GetRequiredString(configuration, "DataProtection:CertificatePath", errors);
        var dataProtectionCertificatePassword = GetRequiredString(configuration, "DataProtection:CertificatePassword", errors);

        var mapboxAccessToken = GetRequiredString(configuration, "Mapbox:AccessToken", errors);
        
        var maximumFileCount = GetPositiveInt(configuration, "Import:MaximumFileCount", errors);
        var maximumCombinedSizeMb = GetPositiveInt(configuration, "Import:MaximumCombinedSizeMb", errors);
        
        if (errors.Count > 0)
            throw new InvalidOperationException(
                "Invalid Heracles configuration:"
                + Environment.NewLine
                + string.Join(Environment.NewLine, errors.Select(error => $" - {error}")));
        
        return new HeraclesSettings(
            new DatabaseSettings(
                databasePath!),
            new OpenIdConnectSettings(
                openIdConnectAuthority!,
                openIdConnectClientId!,
                openIdConnectClientSecret!),
            new DataProtectionSettings(
                dataProtectionKeyPath!,
                dataProtectionCertificatePath!,
                dataProtectionCertificatePassword!),
            new MapboxSettings(mapboxAccessToken!), 
            new ImportSettings(
                maximumFileCount!.Value, 
                maximumCombinedSizeMb!.Value));
    }

    private static string? GetRequiredString(IConfiguration configuration, string key, List<string> errors) {
        var value = configuration[key];

        if (!string.IsNullOrWhiteSpace(value))
            return value;

        errors.Add($"{key} is required.");
        return null;
    }

    private static string? GetAbsoluteFilePath(IConfiguration configuration, string key, List<string> errors) {

        var value = GetRequiredString(configuration, key, errors);

        if (value is null)
            return null;

        if (!Path.IsPathFullyQualified(value)) {
            errors.Add($"{key} must contain a fully qualified filesystem path.");
            return null;
        }

        if (Path.GetFileName(value).Length == 0) {
            errors.Add($"{key} must include a database filename.");
            return null;
        }

        return value;
    }
    
    private static Uri? GetAbsoluteUri(IConfiguration configuration, string key, List<string> errors) {
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
    
    private static int? GetPositiveInt(IConfiguration configuration, string key, List<string> errors)
    {
        var value = configuration[key];

        if (string.IsNullOrWhiteSpace(value)) {
            errors.Add($"{key} is required.");
            return null;
        }

        if (int.TryParse(value, out var parsedValue) && parsedValue > 0)
            return parsedValue;

        errors.Add($"{key} must be a positive integer.");
        return null;
    }
}

public sealed record OpenIdConnectSettings(
    Uri Authority,
    string ClientId,
    string ClientSecret);

public sealed record DatabaseSettings(
    string DatabasePath);

public sealed record DataProtectionSettings(
    string KeyPath,
    string CertificatePath,
    string CertificatePassword);
    
public sealed record MapboxSettings(
    string AccessToken);

public sealed record ImportSettings(
    int MaximumFileCount, 
    int MaximumCombinedSizeMb);