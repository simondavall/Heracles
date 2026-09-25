# Heracles Data Protection Certificate

## Purpose

Heracles.Web uses ASP.NET Core Data Protection to protect application data, including authentication cookies.

The Data Protection key ring is persisted to the filesystem and its sensitive key material is encrypted at rest using a dedicated X.509 certificate.

The Data Protection certificate is separate from the HTTPS/TLS certificate used to secure the Heracles.Web endpoint.

This document describes how to create or recreate the Data Protection certificate.

## Configuration

Heracles.Web obtains its Data Protection configuration through the standard ASP.NET Core configuration system.

The following environment variables are required:

```text
DataProtection__KeyPath
DataProtection__CertificatePath
DataProtection__CertificatePassword
```

For local execution these values are normally supplied through the local .env file.

For an IIS deployment they are supplied through the application's App Pool environment variables.

The certificate password must not be committed to source control.

### Create a Certificate on Windows

The following PowerShell creates a dedicated self-signed RSA certificate suitable for Heracles Data Protection:

```powershell
$certificate = New-SelfSignedCertificate `
    -Subject "CN=Heracles Data Protection" `
    -CertStoreLocation "Cert:\CurrentUser\My" `
    -KeyAlgorithm RSA `
    -KeyLength 4096 `
    -KeyExportPolicy Exportable `
    -NotAfter (Get-Date).AddYears(5)
```


Create a secure password for the exported PFX:

```powershell
$password = ConvertTo-SecureString `
    -String "<certificate-password>" `
    -Force `
    -AsPlainText
```

Export the certificate and private key:

```powershell
Export-PfxCertificate `
    -Cert $certificate `
    -FilePath "<certificate-path>\heracles-dataprotection.pfx" `
    -Password $password
```

Replace <certificate-password> and <certificate-path> with the appropriate deployment values.

The password used when exporting the PFX must match the value configured through:

`DataProtection__CertificatePassword`

The resulting PFX path must match:

`DataProtection__CertificatePath`

### Local Configuration

Example .env configuration:

```text
DataProtection__KeyPath=<local-path>\DataProtection\Keys
DataProtection__CertificatePath=<local-path>\DataProtection\Certificates\heracles-dataprotection.pfx
DataProtection__CertificatePassword=<certificate-password>
```


These paths are examples only and are not required application locations.

The PFX file, populated .env file and persisted Data Protection key ring must not be committed to source control.

### IIS Configuration

For IIS deployments configure the following App Pool environment variables:

```text
DataProtection__KeyPath
DataProtection__CertificatePath
DataProtection__CertificatePassword
```

The IIS App Pool identity must have sufficient filesystem permissions to:

- Read the Data Protection certificate.
- Read the existing Data Protection key ring.
- Create and update files in the Data Protection key directory.

The certificate password must be the password used when the PFX was exported.

### Verification

Start Heracles.Web using the configured certificate and key-ring directory.

On first use, ASP.NET Core Data Protection should create a key file in the configured key directory:

key-<guid>.xml

Inspecting the XML should show that the sensitive Data Protection key material is encrypted rather than persisted in plaintext.

To verify persistence across application restarts:

1. Start Heracles.Web. 
2. Authenticate through Soteria. 
3. Confirm the authenticated application is available. 
4. Stop Heracles.Web without logging out. 
5. Restart Heracles.Web using the same Data Protection key directory and certificate. 
6. Refresh the existing browser session. 
7. Confirm the existing authentication session remains valid without another Soteria login.

This demonstrates that the new process can read the persisted key ring, decrypt it using the configured certificate, and unprotect data created by the previous process.

### Certificate Replacement

The Data Protection certificate protects the persisted Data Protection key ring.

Do not delete or replace the existing certificate simply because a new certificate has been created.

Existing Data Protection keys encrypted with the previous certificate require that certificate to remain decryptable. Replacing the certificate without planning for existing protected keys can prevent Heracles.Web from reading the existing key ring and can invalidate protected application data, including authentication sessions.

Certificate rotation must therefore be treated separately from certificate recreation.

If the existing certificate and private key have been permanently lost, the existing encrypted Data Protection keys cannot be recovered using a newly generated certificate.

In that situation a new certificate and key ring must be established. Data protected exclusively by the lost key ring can no longer be unprotected.

### Security

The PFX contains the private key used to decrypt the Data Protection key ring and must therefore be protected appropriately.

The certificate password is a secret and must not be stored in committed application configuration.

Access to the certificate, persisted key ring and their containing directories should be restricted to the identities that require them.

Do not reuse the Heracles HTTPS/TLS certificate for Data Protection.
