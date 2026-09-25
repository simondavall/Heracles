This document captures the architectural and design decisions that guide development of Heracles.Web. These decisions are considered project rules and should be followed consistently unless implementation evidence provides a compelling reason to change them.

The decisions primarily govern Heracles.Web. Existing Heracles projects are described only where their interaction with Heracles.Web establishes a boundary or requirement for the Web application.

# Application

- Build Heracles.Web as a .NET 10 Blazor Web App.
- Use application-wide Interactive Server rendering.
- Use MudBlazor 9.9.0 as the primary UI component framework.
- Build Heracles.Web as a clean replacement for the existing Heracles Web UI.
- Use the existing application as the reference for established behaviour and information architecture.

# Application Integration

- Heracles.Web consumes existing application functionality through the Application project and its interfaces.
- Configure Infrastructure implementations at the Heracles.Web application composition root.
- UI components depend on Application abstractions rather than concrete Infrastructure implementations.
- Introduce direct dependencies on other application layers only when demonstrated requirements establish a need.

# Project Structure

- Keep routable application pages under `Components/Pages`.
- Keep feature-specific implementation components under `Components/Features`.
- Keep application-wide structural UI under `Components/Layout`.
- Keep reusable cross-feature Heracles UI concepts under `Components/Shared`.
- Introduce feature and shared structures as implementation requires them rather than creating speculative structure.

# Components

- Pages compose feature components.
- Use MudBlazor directly for generic UI controls.
- Implement repeated Heracles-specific visual patterns as shared components once reuse is demonstrated.
- Prefer Razor code-behind for pages and larger components.
- Keep Razor markup declarative.

# Data Access

- Access application data through existing Application services and interfaces.
- Use short-lived Entity Framework Core contexts through `IDbContextFactory<TContext>` for database operations used by Interactive Server functionality.
- Avoid retaining a shared `DbContext` for the lifetime of an interactive Blazor circuit.

# Styling

- Use MudBlazor theming, CSS isolation and application-level styling to implement the Heracles visual design.
- Use colocated `.razor.css` files for component-specific styling.
- Keep application-wide styling under `wwwroot`.
- Define application colours through the Heracles theme and semantic palette rather than feature-specific literal colours where practical.

# Styling

- Use MudBlazor theming, CSS isolation and application-level styling to implement the Heracles visual design.
- Use colocated `.razor.css` files for component-specific styling.
- Keep application-wide styling under `wwwroot` for styles genuinely shared by multiple components.
- Do not move component-specific styling into application-wide CSS solely to work around Blazor CSS isolation.
- Prefer MudBlazor theme and component APIs when they directly represent the required presentation.
- Where component-specific styling must target markup rendered inside a MudBlazor component, introduce the minimum Heracles-owned structural element required to establish the CSS-isolation scope and target the rendered markup using `::deep`.
- Accept the additional structural markup as an explicit compromise in favour of preserving vertical ownership of component-specific styling.
- Define application colours through the Heracles theme and semantic palette rather than feature-specific literal colours where practical.

# Theming

- Support light and dark themes as first-class application requirements.
- Establish the Heracles visual identity through a central MudBlazor theme.
- Retain blue as the principal Heracles application colour with neutral surfaces and semantic colours.
- Use a clean, restrained visual style appropriate for an information-rich application.
- Establish application-wide typography, spacing, shape and elevation through the design system.

# Responsive Design

- Design Heracles.Web for desktop, tablet and mobile layouts from the outset.
- Use the existing desktop application as the initial visual and information-architecture baseline.
- Allow responsive presentation and interaction to adapt while preserving application behaviour.

# JavaScript

- Use Blazor for normal application interaction and state.
- Use JavaScript where browser or third-party functionality requires JavaScript integration.
- Use colocated `.razor.js` ES modules for component-specific JavaScript.
- Manage JavaScript module lifecycle explicitly through Blazor.
- Allow dedicated third-party component containers, such as the activity map, to be managed by their JavaScript integration where required.

# Navigation

- Use Blazor routing and navigation.

# Feature Delivery

- Preserve established application behaviour as the baseline when replacing each feature.
- Treat behavioural enhancements as explicit changes rather than incidental consequences of the rewrite.
- Consider enhancements separately as implementation progresses.

# Authentication

- Authenticate Heracles.Web users through Soteria using OpenID Connect.
- Use the OpenID Connect authorization-code flow with PKCE.
- Use cookie authentication for the Heracles.Web authenticated application session.
- Require authentication by default using the ASP.NET Core authorization fallback policy.
- Explicitly allow anonymous access only where required.
- Provide authentication state to Blazor components through cascading authentication state.
- Keep Heracles.Web authentication configuration at the application composition root.
- Keep legacy ASP.NET Core Identity registration separate from common Infrastructure registration.
- Require application hosts to explicitly opt into host-specific authentication infrastructure.
- Retain the existing MVC Web application's legacy Identity implementation while it remains available for reference.

# Application Configuration

- Represent Heracles application configuration through strongly typed settings records in `Heracles.Application.Configuration`.
- Use `HeraclesSettings` as the root validated application configuration object.
- Group configuration into focused settings objects that can be passed independently to consumers.
- Create `HeraclesSettings` explicitly at the application composition root after configuration sources have been loaded.
- Validate required application configuration once during startup and prevent application startup when validation fails.
- Collect configuration validation failures and report them together rather than failing on the first invalid setting.
- Pass focused settings objects to startup registrations rather than passing `IConfiguration` where the consumer has been migrated to the validated configuration model.
- Do not register settings objects with dependency injection until a runtime consumer requires injection.
- Keep Infrastructure database registration on `IConfiguration` while compatibility with the legacy Web application requires the existing registration contract.

# Logging

- Use Serilog as the Heracles.Web logging implementation.
- Continue to use `ILogger<T>` as the application-facing logging abstraction.
- Configure Serilog through application configuration.
- Keep logging levels and category overrides environment-specific.
- Use console logging in the Development environment.
- Use daily rolling file logging in the Production environment.
- Retain 31 Production log files.
- Supply the Production log-file path through environment-based configuration rather than committed application configuration.

# Local Configuration

- Use DotNetEnv during local execution to load local environment configuration.
- Keep OpenID Connect client secrets outside committed application configuration.
- Add environment variables to ASP.NET Core configuration so application configuration is consumed through the normal IConfiguration hierarchy.
- Limit DotNetEnv loading to explicitly identified local execution.

# Shared Design

- Prefer explicit behaviour over hidden behaviour.
- Prefer readability over cleverness.
- Introduce abstractions when demonstrated implementations show clear value.
- Avoid speculative abstractions.
- Prefer small abstractions with focused responsibilities.
- Optimise for maintainability and consistency.

# Data Protection

- Use ASP.NET Core Data Protection for Heracles.Web protected application data.
- Persist the Data Protection key ring to a configurable filesystem location.
- Encrypt persisted Data Protection keys at rest using a dedicated X.509 certificate containing a private key.
- Keep the Data Protection certificate separate from the HTTPS/TLS certificate.
- Use `Heracles.Web` as the stable Data Protection application name.
- Supply the key-ring path, certificate path and certificate password through environment-based configuration.
- Avoid platform-specific certificate stores and operating-system-specific key protection so the Data Protection configuration remains portable between Windows and Linux.
- Validate required Data Protection configuration and certificate requirements during application startup.
- Treat the Data Protection certificate and persisted key ring as durable application state.
- Do not automatically generate or replace the Data Protection certificate during application startup.

### User state persistence

- Lightweight browser-specific user preferences are persisted in browser LocalStorage.
- Browser-local user state belongs to Heracles.Web.
- `UserState` contains persisted preference values.
- `UserStateService` owns loading and saving UserState.
- `UserStateService` has scoped lifetime and must not be registered as a singleton.
- UserState is persisted as a single JSON document.
- LocalStorage access occurs through JavaScript interop after interactive rendering is available.
- Missing persisted state resolves to a default UserState.
- Invalid persisted JSON is discarded and resolves to default UserState.
- UserState must contain only non-sensitive preferences.
- No generic LocalStorage repository or persistence abstraction is introduced without a demonstrated requirement.
- No schema versioning or migration mechanism is introduced until persisted-state compatibility requires one.

### Theme preference behaviour

- Light and dark themes are defined centrally through `HeraclesTheme`.
- Explicit theme preference is persisted through `UserState.IsDarkMode`.
- A null `IsDarkMode` means no explicit preference and causes Heracles.Web to follow the browser/system colour scheme.
- An explicit light or dark preference takes precedence over subsequent system preference changes.
- The detected system preference is not persisted as an explicit user preference.
- Theme selection uses a single action-oriented icon toggle between light and dark.
- The theme toggle icon represents the available action rather than the currently active theme.
- There is intentionally no UI mechanism for returning to automatic system preference after an explicit selection.
- Theme preference resolution remains owned by the Blazor/MudBlazor application lifecycle.

# Activity Import

- Implement activity import directly through the Blazor application.
- Support multiple GPX files through browser-file selection and drag-and-drop.
- Automatically begin importing selected files.
- Keep import limits configurable through validated ImportSettings.
- Use asynchronous browser-file processing.
- Coordinate file processing and persistence through IImportService.
- Keep existing-track detection state local to each import operation.
- Preserve established GPX processing, validation and duplicate detection.
- Retain transactional bulk persistence through Infrastructure.
- Report import progress directly through callbacks consumed by Blazor.
- Retain weighted progress calculation across file processing and database persistence.
- Do not retain legacy HTTP polling or process-identifier-based progress infrastructure.
- Support cancellation and prevent overlapping operations within the Import page.
- Report individual file-processing failures separately from operation-level persistence failures.
- Report successful imports only after persistence has completed successfully.
