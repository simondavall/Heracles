This document records significant architectural and design decisions made during the lifetime of the Heracles.Web project together with the reasoning and trade-offs behind them.

It provides a historical record of why important implementation decisions were made and helps prevent previously resolved discussions from being revisited without new evidence.

2026-09-16

### Separate host authentication from common Infrastructure

#### Decision

Heracles.Web authenticates users through Soteria using OpenID Connect rather than the legacy ASP.NET Core Identity implementation used by the existing MVC Web application.

Common Infrastructure registration does not register a host authentication implementation.

The legacy ASP.NET Core Identity services, AppIdentityDbContext and Entity Framework Identity stores are registered through a separate Identity-specific Infrastructure registration. The existing MVC Web application explicitly opts into that registration.

Heracles.Web registers common Infrastructure without legacy Identity and configures Soteria authentication at its own composition root.

Heracles.Web uses cookie authentication for its local authenticated session and OpenID Connect as its challenge mechanism. Application access requires authentication by default through the authorization fallback policy.

#### Rationale

- Heracles.Web and the legacy MVC Web application use different authentication authorities.
- Authentication configuration is a host concern and should not be introduced implicitly by common Infrastructure registration.
- Registering both authentication implementations caused the legacy `Identity.Application` scheme to conflict with the Heracles.Web cookie authentication scheme.
- Relying on dependency-registration order to resolve competing authentication schemes would create hidden and fragile behaviour.
- Separating Identity registration makes each application's authentication dependency explicit.
- The legacy MVC application can continue using its existing Identity implementation while Heracles.Web migrates to Soteria.
- The separation removes the unused legacy Identity authentication dependency from Heracles.Web without requiring migration of the retiring MVC application's Identity implementation.

2026-09-16

### Use certificate-protected persistent Data Protection keys

#### Decision

Heracles.Web uses ASP.NET Core Data Protection with a persistent filesystem key ring.

Persisted Data Protection keys are encrypted at rest using a dedicated X.509 certificate containing a private key.

The key-ring path, certificate path and certificate password are supplied through environment-based configuration.

The implementation uses a certificate loaded from a PFX rather than relying on a platform-specific certificate store or operating-system-specific key-protection mechanism.

The Data Protection certificate is separate from the HTTPS/TLS certificate.

Heracles.Web uses `Heracles.Web` as its stable Data Protection application name.

#### Rationale

- Data Protection keys must survive application restarts and deployments.
- Persisted key material must be encrypted independently of filesystem permissions.
- Heracles.Web is intended to remain portable across Windows and Linux hosting environments.
- A PFX-based certificate can be deployed consistently across those environments without coupling Data Protection to the Windows certificate store or DPAPI.
- Keeping Data Protection and HTTPS certificates separate allows their purposes and lifecycles to remain independent.
- Environment-based configuration allows local DotNetEnv configuration, IIS App Pool environment variables and future hosting mechanisms to use the same application implementation.

2026-09-17

### Use Serilog for application logging

#### Decision

Heracles.Web uses Serilog as its logging implementation while application code continues to consume logging through the standard `ILogger<T>` abstraction.

Serilog is configured through application configuration with environment-specific logging levels, category overrides and sinks.

The Development environment writes logs to the console.

The Production environment writes logs to daily rolling files with a 31-file retention limit.

The Production log-file path is supplied through environment-based configuration rather than committed application configuration.

#### Rationale

- Serilog provides the logging implementation required by Heracles.Web while retaining integration with the standard .NET logging abstractions.
- Keeping application code dependent on `ILogger<T>` avoids coupling components and services directly to Serilog.
- Environment-specific configuration allows Development and Production to use different logging granularity without application-code changes.
- Console logging provides appropriate local development diagnostics.
- Rolling file logging provides persistent diagnostics for the locally hosted Production environment.
- A retention limit prevents Production log files from accumulating indefinitely.
- Keeping the Production log-file path outside committed configuration allows the deployment environment to control the physical logging location.
- Environment-based configuration allows local DotNetEnv configuration, IIS App Pool environment variables and future hosting mechanisms to supply deployment-specific values without changing the application implementation.

2026-09-17

### Validate application configuration through HeraclesSettings

#### Decision

Heracles application configuration is represented by strongly typed settings records in `Heracles.Application.Configuration`.

`HeraclesSettings` acts as the root configuration object and groups configuration into focused settings objects for individual application concerns.

Heracles.Web creates `HeraclesSettings` explicitly at the application composition root after configuration sources have been loaded. Required values are validated during creation and all detected configuration failures are reported together. Invalid configuration prevents application startup.

Startup registrations consume focused settings objects where they have been migrated to the validated configuration model. Authentication receives `OpenIdConnectSettings` and Data Protection receives `DataProtectionSettings`.

Settings objects are not registered with dependency injection unless a runtime consumer demonstrates a requirement for injection.

Infrastructure database registration continues to receive `IConfiguration` while the existing registration contract is shared with the legacy Web application.

#### Rationale

- Application configuration should be validated once rather than requiring each consumer to retrieve and repeatedly validate configuration values.
- Successfully created settings objects provide consumers with values that have already satisfied their startup validation requirements.
- Focused settings records prevent consumers from receiving unrelated application configuration.
- Locating the configuration model in Application makes it visible to projects throughout the solution without coupling the model to Heracles.Web.
- Explicit creation in the Heracles.Web composition root makes configuration availability and validation order visible during application startup.
- Keeping settings outside dependency injection until runtime injection is required avoids speculative service registrations.
- Reporting all detected configuration failures together provides more useful startup diagnostics than failing on the first invalid value.
- Retaining the existing Infrastructure registration contract avoids changes to the legacy Web application during the Heracles.Web migration.
