using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.DataProtection;

namespace Heracles.Web.Components.Features.DataProtection;

public static class DataProtectionExtensions
{
    public static IServiceCollection AddHeraclesDataProtection(this IServiceCollection services, IConfiguration configuration) {
        var keyPath = GetRequiredConfiguration(configuration, "DataProtection:KeyPath");
        var certificatePath = GetRequiredConfiguration(configuration, "DataProtection:CertificatePath");
        var certificatePassword = GetRequiredConfiguration(configuration, "DataProtection:CertificatePassword");

        var keyDirectory = new DirectoryInfo(keyPath);

        if (!keyDirectory.Exists) 
            keyDirectory.Create();

        if (!File.Exists(certificatePath))
            throw new InvalidOperationException($"The Data Protection certificate does not exist: '{certificatePath}'.");

        X509Certificate2 certificate;

        try {
            certificate = X509CertificateLoader.LoadPkcs12FromFile(certificatePath, certificatePassword);
        }
        catch (Exception ex) {
            throw new InvalidOperationException($"The Data Protection certificate could not be loaded from '{certificatePath}'.", ex);
        }

        if (!certificate.HasPrivateKey)
            throw new InvalidOperationException($"The Data Protection certificate '{certificatePath}' does not contain a private key.");

        services
            .AddDataProtection()
            .SetApplicationName("Heracles.Web")
            .PersistKeysToFileSystem(keyDirectory)
            .ProtectKeysWithCertificate(certificate);

        return services;
    }

    private static string GetRequiredConfiguration(IConfiguration configuration, string key) { 
        var value = configuration[key];

        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException($"Required configuration value '{key}' has not been provided.");

        return value;
    }
}