This document provides a snapshot of the current implementation state of the project. 
It records completed work, the feature currently being developed, and the next expected 
steps. It should be updated regularly and is intended to help developers quickly understand 
where development should continue.

# Current phase

- Phase 2 – Activity Management

# Current milestone

- Milestone 2.2 – Activity Details

# Current task

- Create reusable activity metric components.
    - Display activity distance.
    - Display activity duration.
    - Display average pace.
    - Display activity rank.

# Remaining milestone tasks

- Verify light and dark theme presentation.

# Completed work

- Created the new Heracles.Web .NET 10 Blazor Web App project alongside the existing Web application.
- Added Heracles.Web to the existing Heracles solution.
- Added project references to Application and Infrastructure, with Infrastructure used as the composition root dependency.
- Configured application-wide Interactive Server rendering.
- Added MudBlazor 9.9.0 and registered the required MudBlazor services and providers.
- Added a temporary MudBlazor control to the Home page and verified that MudBlazor rendered and operated correctly.
- Removed the temporary MudBlazor test control.
- Registered the existing Application services through AddApplication().
- Registered the existing Infrastructure implementation through AddInfrastructure() in Program.cs.
- Established the UI dependency boundary: components consume Application interfaces while Infrastructure is configured at the application composition root.
- Established the initial component structure around Pages, Features, Layout, and Shared components.
- Established conventions for component-specific CSS isolation using colocated .razor.css files.
- Established conventions for component-specific JavaScript isolation using colocated .razor.js ES modules.
- Configured EF Core database access through the existing Infrastructure registration and IDbContextFactory<TContext>.
- Added temporary Home page code using IActivityService.GetMostRecentActivityAsync() to test the complete dependency chain.
- Successfully retrieved and displayed the most recent activity from the existing Heracles database.
- Verified database access from an Interactive Server component through UI → Application → Infrastructure → EF Core → SQL Server.
- Removed the temporary database/dependency-chain test code after verification.
- Established use of the built-in ASP.NET Core / Microsoft.Extensions.Logging logging infrastructure with configuration-driven log levels.
- Established the initial architectural decisions and conventions in architecture.md.
- Established the Heracles theme.
- Defined light and dark colour palettes.
- Configured application typography and visual styling.
- Integrated MudBlazor styling and JavaScript.
- Implemented System, Light and Dark theme switching.
- Verified theme switching behaviour.
- Integrated Heracles.Web authentication with Soteria using OpenID Connect.
- Configured cookie authentication for the Heracles.Web authenticated session.
- Configured OpenID Connect authorization-code flow with PKCE.
- Configured authenticated-by-default authorization using the fallback authorization policy.
- Added Soteria login, logout and access-denied handling.
- Added cascading authentication state for Blazor components.
- Integrated authenticated user information into the application layout.
- Separated legacy ASP.NET Core Identity registration from common Infrastructure registration.
- Updated the existing MVC Web application to explicitly opt into its legacy Identity infrastructure.
- Verified Soteria authentication, authenticated application access and logout.
- Verified the existing MVC Web Identity login continues to operate.
- Implemented DotNetEnv loading for local execution.
- Configured local OpenID Connect secrets through environment-based configuration.
- Implemented cross-platform ASP.NET Core Data Protection.
- Configured a stable Heracles.Web Data Protection application name.
- Configured persistent filesystem storage for the Data Protection key ring.
- Configured encryption at rest for persisted Data Protection keys using a dedicated X.509 certificate.
- Configured the Data Protection key path, certificate path and certificate password through environment-based configuration.
- Added fail-fast validation for missing Data Protection configuration, missing certificates, invalid certificate credentials and certificates without a private key.
- Verified Data Protection key persistence and encryption at rest.
- Verified an authenticated Soteria session remains valid across Heracles.Web application restarts using the persisted key ring.
- Verified Data Protection startup failure paths.
- Documented Data Protection certificate creation, configuration, verification and replacement considerations.
- Implemented Serilog as the Heracles.Web logging implementation.
- Retained `ILogger<T>` as the application-facing logging abstraction.
- Configured Serilog through environment-specific application configuration.
- Configured Development logging to the console.
- Configured Production logging to daily rolling files with a 31-file retention limit.
- Configured environment-specific logging levels and category overrides.
- Configured the Production log-file path through environment-based configuration.
- Verified Development console logging and Production file logging.
- Implemented strongly typed Heracles application configuration through `HeraclesSettings`.
- Located the application configuration model in `Heracles.Application.Configuration` for solution-wide visibility.
- Split application configuration into focused `DatabaseSettings`, `OpenIdConnectSettings` and `DataProtectionSettings` records.
- Configured Heracles.Web to create and validate `HeraclesSettings` explicitly at the application composition root.
- Added startup validation for required application configuration with aggregated validation failures.
- Configured OpenID Connect authority as a validated absolute URI.
- Updated Authentication registration to consume validated `OpenIdConnectSettings`.
- Updated Data Protection registration to consume validated `DataProtectionSettings`.
- Retained Data Protection-specific filesystem and certificate validation within Data Protection registration.
- Retained the existing Infrastructure `IConfiguration` database registration for compatibility with the legacy Web application.
- Verified valid Development and Production configuration allows normal application startup.
- Verified invalid configuration prevents startup and reports multiple configuration failures together.
- Implemented browser-local UserState persistence in Heracles.Web.
- Added scoped `UserStateService` for loading and saving lightweight user preferences.
- Persisted UserState as a single JSON document in browser LocalStorage.
- Added isolated JavaScript ES module for LocalStorage access.
- Added nullable Dark Mode preference with null representing no explicit user preference.
- Added recovery from invalid persisted UserState by discarding corrupt browser state and returning to defaults.
- Verified UserState persistence across page reloads and application restarts.
- Verified missing and invalid persisted state fall back to default UserState.
- Verified persisted JSON tolerates removed properties and redundant properties are removed on subsequent saves.
- Persisted explicit light and dark theme preferences through browser-local UserState.
- Added system colour-scheme fallback when no explicit theme preference exists.
- Retained system theme change monitoring while no explicit preference exists.
- Explicit user theme selection now takes precedence over subsequent system preference changes.
- Replaced the System / Light / Dark theme selector with a single action-oriented theme toggle.
- Removed the redundant ThemeMode state model.
- Verified light and dark theme selections persist across page refreshes and application restarts.
- Verified system theme preference remains the default until the user explicitly selects a theme.
- Verified immediate light/dark theme switching across the application.
- Created staging environment
- Published site to staging
- Verified staging site working correctly
- Created the permanent Heracles.Web main application layout.
- Created the global Heracles application header.
- Created the constrained application body and main content area.
- Configured the AppBar to retain the Heracles dark background and light text in both light and dark themes.
- Established the application body as theme-responsive while keeping the AppBar presentation invariant.
- Established the primary navigation region ready for the primary navigation component.
- Implemented responsive application-shell behaviour for desktop, tablet and mobile layouts.
- Verified the application shell in light and dark themes.
- Verified existing authentication, logout and theme persistence behaviour through the new application shell.
- Established the MudBlazor CSS-isolation styling convention for component-specific presentation.
- Created the primary navigation component.
- Added authenticated user summary with profile image and display name.
- Added Active Since presentation using the earliest recorded activity with the existing UTC-now fallback.
- Added route-aware Dashboard, Reports and Import navigation.
- Added distinct inactive, hover and selected navigation states.
- Added Reports and Import placeholder pages as routable navigation targets.
- Implemented theme-aware primary navigation presentation for light and dark modes.
- Implemented responsive primary navigation behaviour.
- Kept primary-navigation presentation colocated in the component's isolated stylesheet.
- Created the Activity Navigation feature component.
- Added year-based activity navigation with activity counts.
- Added month-based navigation beneath expanded years with activity counts.
- Implemented year and month expansion using MudBlazor expansion panels.
- Initialised the navigator with the current activity year and month expanded.
- Loaded activity summaries for the selected month through the existing Application activity service.
- Added activity summary presentation with date, activity type and distance.
- Added selected activity highlighting.
- Added selected year and month presentation using the application primary colour.
- Added Blazor navigation to `/activity/{id}` when an activity is selected.
- Added `/activity/{id:guid}` as an activity route on the Home page ready for Activity Details.
- Added Activity Navigation loading and empty states.
- Kept Activity Navigation presentation colocated in its isolated stylesheet.
- Implemented theme-aware Activity Navigation presentation.
- Implemented responsive Activity Navigation presentation.
- Verified activity year and month expansion and collapse.
- Verified the current activity year and month are expanded on initial load.
- Verified activity selection and Blazor route navigation.
- Created the Activity Title feature component.
- Added activity type image, title and date time.
- Added automatic loading of the most recent activity when entering the application without an activity route.
- Retained the existing Activity Navigation empty state with blank Activity Details when no activities exist.
- Implemented responsive Activity Details presentation with Activity Navigation above Activity Details on mobile viewports.
- Verified Activity Title presentation in light and dark themes.
