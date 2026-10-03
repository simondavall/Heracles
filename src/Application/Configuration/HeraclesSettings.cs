using Microsoft.Extensions.Configuration;

namespace Heracles.Application.Configuration;

public sealed record HeraclesSettings
{
    public required DatabaseSettings Database { get; init; }
    public required OpenIdConnectSettings OpenIdConnect { get; init; }
    public required DataProtectionSettings DataProtection { get; init; }
    public required MapboxSettings Mapbox { get; init; }
    public required ImportSettings Import { get; init; }
    public required WeatherApiSettings WeatherApi { get; init; }
    public required PaceSettings Pace { get; init; }
    public required SpeedSettings Speed { get; init; }
    public required ElevationSettings Elevation { get; init; }
    public required RankSettings Rank { get; init; }

    public static HeraclesSettings Create(IConfiguration configuration) {
        var errors = new List<string>();

        var oidcAuthority = GetAbsoluteUri(configuration, "OpenIdConnect:Authority", errors);
        var oidcClientId = GetRequiredString(configuration, "OpenIdConnect:ClientId", errors);
        var oidcClientSecret = GetRequiredString(configuration, "OpenIdConnect:ClientSecret", errors);

        var databasePath = GetAbsoluteFilePath(configuration, "Database:DatabasePath", errors);

        var dpKeyPath = GetRequiredString(configuration, "DataProtection:KeyPath", errors);
        var dpCertificatePath = GetRequiredString(configuration, "DataProtection:CertificatePath", errors);
        var dpCertificatePassword = GetRequiredString(configuration, "DataProtection:CertificatePassword", errors);

        var mapboxAccessToken = GetRequiredString(configuration, "Mapbox:AccessToken", errors);

        var maximumFileCount = GetPositiveInt(configuration, "Import:MaximumFileCount", errors);
        var maximumCombinedSizeMb = GetPositiveInt(configuration, "Import:MaximumCombinedSizeMb", errors);

        var weatherApiKey = GetRequiredString(configuration, "WeatherApi:Key", errors);
        var weatherApiUri = GetAbsoluteUri(configuration, "WeatherApi:Uri", errors);

        var paceWindowRadius = GetPositiveInt(configuration, "Charts:Pace:WindowRadius", errors);
        var speedWindowRadius = GetPositiveInt(configuration, "Charts:Speed:WindowRadius", errors);
        var elevationMinimumChartRange = GetPositiveInt(configuration, "Charts:Elevation:MinimumChartRange", errors);
        var rankRangePercentage = GetPositivePercentage(configuration, "Charts:Rank:RangePercentage", errors);

        if (errors.Count > 0)
            throw new InvalidOperationException(
                "Invalid Heracles configuration:"
                + Environment.NewLine
                + string.Join(Environment.NewLine, errors.Select(error => $" - {error}")));

        return new HeraclesSettings {
            Database = new DatabaseSettings(databasePath!),
            OpenIdConnect = new OpenIdConnectSettings(
                oidcAuthority!,
                oidcClientId!,
                oidcClientSecret!),
            DataProtection = new DataProtectionSettings(
                dpKeyPath!,
                dpCertificatePath!,
                dpCertificatePassword!),
            Mapbox = new MapboxSettings(mapboxAccessToken!),
            Import = new ImportSettings(
                maximumFileCount!.Value,
                maximumCombinedSizeMb!.Value),
            WeatherApi = new WeatherApiSettings(
                weatherApiKey!,
                weatherApiUri!),
            Pace = new PaceSettings(paceWindowRadius!.Value),
            Speed = new SpeedSettings(speedWindowRadius!.Value),
            Elevation = new ElevationSettings(elevationMinimumChartRange!.Value),
            Rank = new RankSettings(rankRangePercentage!.Value)
        };
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

    private static int? GetPositiveInt(IConfiguration configuration, string key, List<string> errors) {
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
    
    private static double? GetPositivePercentage(IConfiguration configuration, string key, List<string> errors) {
        var value = configuration[key];

        if (string.IsNullOrWhiteSpace(value)) {
            errors.Add($"{key} is required.");
            return null;
        }

        if (double.TryParse(value, out var parsedValue) && parsedValue is > 0 and <= 1)
            return parsedValue;

        errors.Add($"{key} must be a positive percentage between 0 and 1. 1 represents 100%");
        return null;
    }
}

public sealed record OpenIdConnectSettings(
    Uri Authority,
    string ClientId,
    string ClientSecret);

public sealed record DatabaseSettings(string DatabasePath);

public sealed record DataProtectionSettings(
    string KeyPath,
    string CertificatePath,
    string CertificatePassword);

public sealed record MapboxSettings(string AccessToken);

public sealed record ImportSettings(
    int MaximumFileCount,
    int MaximumCombinedSizeMb);

public sealed record WeatherApiSettings(
    string Key,
    Uri Uri);

public sealed record PaceSettings(int WindowRadius);

public sealed record SpeedSettings(int WindowRadius);

public sealed record ElevationSettings(int MinimumChartRange);

public sealed record RankSettings(double RangePercentage);