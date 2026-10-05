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
- Keep authentication registration separate from common Infrastructure registration.
- Require application hosts to explicitly configure their authentication infrastructure.

# Application Configuration

- Represent Heracles application configuration through strongly typed settings records in `Heracles.Application.Configuration`.
- Use `HeraclesSettings` as the root validated application configuration object.
- Group configuration into focused settings objects that can be passed independently to consumers.
- Create `HeraclesSettings` explicitly at the application composition root after configuration sources have been loaded.
- Validate required application configuration once during startup and prevent application startup when validation fails.
- Collect configuration validation failures and report them together rather than failing on the first invalid setting.
- Pass focused settings objects to startup registrations rather than passing `IConfiguration` where the consumer has been migrated to the validated configuration model.
- Do not register settings objects with dependency injection until a runtime consumer requires injection.

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

# Historical Weather

- Retrieve historical weather through the Application weather service.
- Define the external weather provider contract in Application.
- Implement external weather integrations in Infrastructure.
- Keep provider-specific weather codes and response formats within their respective Infrastructure implementations.
- Translate provider-specific weather conditions into the common Application WeatherCode enumeration.
- Keep weather presentation independent of the selected provider.
- Use the first recorded GPS point as the weather lookup location.
- Use the activity midpoint as the weather lookup timestamp.
- Treat weather observations as approximate historical information.
- Persist retrieved weather observations against their activities.
- Reuse persisted observations rather than repeatedly requesting historical weather.
- Retain cached observations when changing providers until a different cache policy is explicitly agreed.
- Select the active weather provider through Infrastructure dependency injection.

# Activity Track-Point Data

- Treat imported TrackPoint data as immutable after persistence.
- Maintain reusable derived TrackPointData independently of individual presentation features.
- Persist one TrackPointData row for each source TrackPoint.
- Store the source TrackPointId, zero-based activity-wide sequence, cleaned cumulative geographic distance, cumulative active elapsed time and recorded elevation.
- Generate TrackPointData lazily when first required for an activity.
- Reuse persisted TrackPointData when present without completeness checking or replacement.
- Treat TrackPointData as regenerable derived data. When its calculation or representation changes, existing data may be deleted and regenerated.
- Calculate geographic distance only between consecutive points within the same recording segment.
- After calculating the initial cumulative geographic-distance series, iteratively clean its interior values using time-weighted interpolation between each point's surrounding cumulative distances.
- Configure the number of cleaning iterations through application-wide DataSettings.
- Each cleaning iteration operates on the complete result of the previous iteration.
- Preserve the first and final cumulative distances during cleaning so that cleaning redistributes intermediate distance without changing the calculated total activity distance.
- Treat recording-segment boundaries as pauses and add neither elapsed time nor geographic distance between segments.
- Keep cumulative track-point data independent of feature-specific calculation and presentation settings.
- Use the Application track-point data service to own generation and reuse of TrackPointData.
- Use the Infrastructure track-point data repository to own persistence of TrackPointData.

# Activity Pace

- Calculate activity pace from shared TrackPointData using cumulative geographic distance and cumulative active elapsed time.
- Allow pace calculation windows to span recording-segment boundaries after pause time and inter-segment distance have been removed from the cumulative representation.
- Calculate rolling pace from differences in cumulative distance and cumulative active time rather than averaging individual point-to-point pace values.
- Configure rolling pace calculation using an application-wide WindowRadius setting.
- Keep WindowRadius independent of persisted TrackPointData so presentation calculations can be tuned without regenerating persisted data.
- Calculate a pace observation at every eligible TrackPointData centre.
- Use shrinking centred windows at the beginning and end of an activity.
- Retain the centre TrackPointId on each calculated pace observation.
- Omit an observation when its calculation window does not contain positive distance and positive elapsed time.
- Integrate Chart.js through a colocated JavaScript ES module owned by the pace-chart Blazor component.
- Keep Chart.js-specific concerns within Web presentation and keep pace calculation independent of chart presentation.

# Activity Elevation

- Display recorded TrackPoint elevation against cumulative activity distance.
- Reuse shared TrackPointData rather than independently recalculating cumulative distance for elevation presentation.
- Display cumulative activity distance in kilometres and elevation in metres.
- Represent every recorded track point in the elevation series.
- Integrate Chart.js through a colocated JavaScript ES module owned by the elevation-chart Blazor component.
- Keep Chart.js-specific concerns within Web presentation and keep elevation data preparation independent of chart presentation.

# Activity Speed

- Calculate activity speed from shared TrackPointData using cumulative geographic distance and cumulative active elapsed time.
- Calculate speed independently from pace rather than converting calculated pace observations into speed observations.
- Configure rolling speed calculation using its application-wide WindowRadius setting.
- Calculate a speed observation at every eligible TrackPointData centre.
- Apply the same centred rolling-window and shrinking-boundary behaviour used for pace calculation.
- Allow speed calculation windows to span recording-segment boundaries after pause time and inter-segment distance have been removed from TrackPointData.
- Retain the centre TrackPointId on each calculated speed observation.
- Omit an observation when its calculation window does not contain positive distance and positive elapsed time.
- Do not persist calculated speed observations.
- Do not extend TrackPointData specifically for speed calculation.
- Display speed in kilometres per hour.
- Display the Speed chart instead of the Pace chart for Cycling activities.
- Display the Pace chart for non-Cycling activities.
- Keep the Elevation chart independent of Pace/Speed selection and display it for all activity types.
- Integrate Chart.js through a colocated JavaScript ES module owned by the speed-chart Blazor component.
- Keep Chart.js-specific concerns within Web presentation and keep speed calculation independent of chart presentation.

# Activity Details Interaction

- Use source TrackPointId as the common interaction identity between Activity Details charts and the Activity Map.
- Keep synchronized interaction state within Heracles.Web because it is presentation state rather than application-domain state.
- Use scoped Activity Details interaction state to coordinate sibling presentation components.
- Treat the full chart plotting area as the Activity Details chart interaction surface.
- Resolve source-chart pointer selection solely from horizontal cumulative-distance position by selecting the chart point whose distance is closest to the pointer's X-axis value.
- Do not use vertical proximity to the plotted line when determining the selected TrackPoint.
- Publish the resulting TrackPointId as the synchronized selection rather than cumulative distance.
- Use the same selected TrackPointId for the source chart's point, tooltip and crosshair.
- Synchronize Pace/Speed and Elevation chart selection using exact TrackPointId matching.
- Do not substitute a neighbouring Pace/Speed observation when the selected TrackPointId has no valid Pace/Speed observation.
- Display synchronized chart selection using the chart tooltip and a vertical crosshair.
- Resolve Activity Map selection directly from the source TrackPointId and use the recorded longitude and latitude.
- Do not calculate or interpolate geographic position for synchronized map selection.
- Keep the Activity Map passive; chart interaction drives map selection, while map interaction does not drive chart selection.
- Clear synchronized interaction when the pointer leaves the chart or the selected activity changes.
- Keep Chart.js and Mapbox-specific synchronized presentation behaviour within their owning Web components and colocated JavaScript modules.
