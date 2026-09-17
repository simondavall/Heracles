using System.Security.Cryptography.X509Certificates;
using Heracles.Application.Configuration;
using Microsoft.AspNetCore.DataProtection;

namespace Heracles.Web.Components.Features.DataProtection;

public static class DataProtectionExtensions
{
    public static IServiceCollection AddHeraclesDataProtection(this IServiceCollection services, DataProtectionSettings settings) {
        var certificatePath = settings.CertificatePath;
        var certificatePassword = settings.CertificatePassword;

        var keyDirectory = new DirectoryInfo(settings.KeyPath);

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
}