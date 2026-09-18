This document records significant architectural and design decisions made during the lifetime of the Heracles.Web project together with the reasoning and trade-offs behind them.

It provides a historical record of why important implementation decisions were made and helps prevent previously resolved discussions from being revisited without new evidence.

## Separate host authentication from common Infrastructure
(16-09-2026)

### Decision

Heracles.Web authenticates users through Soteria using OpenID Connect rather than the legacy ASP.NET Core Identity implementation used by the existing MVC Web application.

Common Infrastructure registration does not register a host authentication implementation.

The legacy ASP.NET Core Identity services, AppIdentityDbContext and Entity Framework Identity stores are registered through a separate Identity-specific Infrastructure registration. The existing MVC Web application explicitly opts into that registration.

Heracles.Web registers common Infrastructure without legacy Identity and configures Soteria authentication at its own composition root.

Heracles.Web uses cookie authentication for its local authenticated session and OpenID Connect as its challenge mechanism. Application access requires authentication by default through the authorization fallback policy.

### Rationale

- Heracles.Web and the legacy MVC Web application use different authentication authorities.
- Authentication configuration is a host concern and should not be introduced implicitly by common Infrastructure registration.
- Registering both authentication implementations caused the legacy `Identity.Application` scheme to conflict with the Heracles.Web cookie authentication scheme.
- Relying on dependency-registration order to resolve competing authentication schemes would create hidden and fragile behaviour.
- Separating Identity registration makes each application's authentication dependency explicit.
- The legacy MVC application can continue using its existing Identity implementation while Heracles.Web migrates to Soteria.
- The separation removes the unused legacy Identity authentication dependency from Heracles.Web without requiring migration of the retiring MVC application's Identity implementation.

## Use certificate-protected persistent Data Protection keys
(16-09-2026)

### Decision

Heracles.Web uses ASP.NET Core Data Protection with a persistent filesystem key ring.

Persisted Data Protection keys are encrypted at rest using a dedicated X.509 certificate containing a private key.

The key-ring path, certificate path and certificate password are supplied through environment-based configuration.

The implementation uses a certificate loaded from a PFX rather than relying on a platform-specific certificate store or operating-system-specific key-protection mechanism.

The Data Protection certificate is separate from the HTTPS/TLS certificate.

Heracles.Web uses `Heracles.Web` as its stable Data Protection application name.

### Rationale

- Data Protection keys must survive application restarts and deployments.
- Persisted key material must be encrypted independently of filesystem permissions.
- Heracles.Web is intended to remain portable across Windows and Linux hosting environments.
- A PFX-based certificate can be deployed consistently across those environments without coupling Data Protection to the Windows certificate store or DPAPI.
- Keeping Data Protection and HTTPS certificates separate allows their purposes and lifecycles to remain independent.
- Environment-based configuration allows local DotNetEnv configuration, IIS App Pool environment variables and future hosting mechanisms to use the same application implementation.

2026-09-17

## Use Serilog for application logging
(17-09-2026)

### Decision

Heracles.Web uses Serilog as its logging implementation while application code continues to consume logging through the standard `ILogger<T>` abstraction.

Serilog is configured through application configuration with environment-specific logging levels, category overrides and sinks.

The Development environment writes logs to the console.

The Production environment writes logs to daily rolling files with a 31-file retention limit.

The Production log-file path is supplied through environment-based configuration rather than committed application configuration.

### Rationale

- Serilog provides the logging implementation required by Heracles.Web while retaining integration with the standard .NET logging abstractions.
- Keeping application code dependent on `ILogger<T>` avoids coupling components and services directly to Serilog.
- Environment-specific configuration allows Development and Production to use different logging granularity without application-code changes.
- Console logging provides appropriate local development diagnostics.
- Rolling file logging provides persistent diagnostics for the locally hosted Production environment.
- A retention limit prevents Production log files from accumulating indefinitely.
- Keeping the Production log-file path outside committed configuration allows the deployment environment to control the physical logging location.
- Environment-based configuration allows local DotNetEnv configuration, IIS App Pool environment variables and future hosting mechanisms to supply deployment-specific values without changing the application implementation.

## Validate application configuration through HeraclesSettings
(17-09-2026)

### Decision

Heracles application configuration is represented by strongly typed settings records in `Heracles.Application.Configuration`.

`HeraclesSettings` acts as the root configuration object and groups configuration into focused settings objects for individual application concerns.

Heracles.Web creates `HeraclesSettings` explicitly at the application composition root after configuration sources have been loaded. Required values are validated during creation and all detected configuration failures are reported together. Invalid configuration prevents application startup.

Startup registrations consume focused settings objects where they have been migrated to the validated configuration model. Authentication receives `OpenIdConnectSettings` and Data Protection receives `DataProtectionSettings`.

Settings objects are not registered with dependency injection unless a runtime consumer demonstrates a requirement for injection.

Infrastructure database registration continues to receive `IConfiguration` while the existing registration contract is shared with the legacy Web application.

### Rationale

- Application configuration should be validated once rather than requiring each consumer to retrieve and repeatedly validate configuration values.
- Successfully created settings objects provide consumers with values that have already satisfied their startup validation requirements.
- Focused settings records prevent consumers from receiving unrelated application configuration.
- Locating the configuration model in Application makes it visible to projects throughout the solution without coupling the model to Heracles.Web.
- Explicit creation in the Heracles.Web composition root makes configuration availability and validation order visible during application startup.
- Keeping settings outside dependency injection until runtime injection is required avoids speculative service registrations.
- Reporting all detected configuration failures together provides more useful startup diagnostics than failing on the first invalid value.
- Retaining the existing Infrastructure registration contract avoids changes to the legacy Web application during the Heracles.Web migration.

## Persist lightweight user state in browser LocalStorage
(17-09-2026)

### Decision

Persist lightweight browser-specific user preferences through a scoped `UserStateService` in Heracles.Web.

UserState is stored as a single JSON document in browser LocalStorage and accessed through an isolated JavaScript ES module after browser interop becomes available.

Missing state resolves to default UserState. Invalid persisted JSON is discarded and reset to default state.

### Rationale

The anticipated preferences are small, browser-specific presentation settings that do not currently justify database persistence.

Keeping this capability within Heracles.Web reflects the browser-specific nature of the state and avoids introducing persistence abstractions or dependencies that are not yet required.

A single JSON document provides simple load/save behaviour while allowing straightforward additions and removals of properties. Testing confirmed that unknown persisted properties are ignored during deserialization and removed on subsequent saves.

Schema versioning and migration infrastructure are deferred until a concrete compatibility requirement exists.

## Persist explicit theme preference through UserState
(17-09-2026)

### Decision

Use `UserState.IsDarkMode` as the persisted representation of the user's theme preference.

A null value represents no explicit preference and causes Heracles.Web to follow the browser/system colour scheme. Explicit true or false values select dark or light mode respectively and take precedence over subsequent system preference changes.

Replace the previous System / Light / Dark selector with a single action-oriented light/dark icon toggle.

There is intentionally no UI mechanism for returning to automatic system preference after the user makes an explicit selection.

### Rationale

The nullable preference represents the required three states without maintaining a separate theme-mode abstraction.

Persisting only explicit user selections allows Heracles.Web to respect the system preference by default while retaining a user's deliberate light or dark selection across browser sessions.

A direct light/dark toggle keeps the frequently used interaction simple. Returning to system-controlled behaviour is not currently important enough to justify additional UI.

## Preserve component-specific CSS ownership across MudBlazor component boundaries
(18-09-2026)

### Decision

Heracles.Web keeps component-specific styling colocated with the owning Razor component in its `.razor.css` file.

Application-wide CSS under `wwwroot` is reserved for styling genuinely shared by multiple components. Component-specific CSS is not moved into application-wide stylesheets solely because a MudBlazor component boundary prevents normal CSS-isolation selectors from reaching its rendered markup.

Where MudBlazor natively represents a presentation requirement through its theme or component API, that mechanism is preferred.

Where component-specific styling must reach markup rendered by a MudBlazor component, the owning Heracles component introduces the minimum structural markup required to establish an owned CSS-isolation scope and uses `::deep` to target the required rendered markup.

This is an explicit compromise and should be reviewed if repeated use produces excessive structural markup or fragile dependencies on MudBlazor implementation details.

### Rationale

- Component markup and component-specific presentation should remain vertically colocated.
- Moving private component styles into global CSS weakens ownership and makes the global stylesheet responsible for unrelated component implementation details.
- Blazor CSS isolation does not automatically scope CSS to HTML rendered internally by child MudBlazor components.
- An Heracles-owned wrapper provides a predictable CSS-isolation boundary from which `::deep` can deliberately cross into the child component.
- MudBlazor's theme and component APIs remain preferable where they directly model the required presentation.
- Additional wrapper markup is accepted as a visible technical compromise rather than hiding component-specific styling in application-wide CSS.
- The approach can be reconsidered if experience across further components demonstrates that its structural or maintenance cost is too high.
