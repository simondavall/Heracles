# Foundation Decisions

The following decisions establish the initial technical direction for Heracles.Web. They may be revisited through the project decision process if implementation evidence requires a change.

Heracles.Web is focused solely on replacing the existing Heracles Web UI with a new .NET 10 Blazor application.

## Application
- Heracles.Web.

## Framework
- .NET 10 Blazor Web App.
- Interactive Server, configured application-wide.
- MudBlazor 9.9.0.

## Migration strategy
- Build a clean replacement Web UI using the existing Heracles application as the reference for behaviour and information architecture.

## Application integration
- Heracles.Web components depend on the Application project and its interfaces.

## Infrastructure integration
- Heracles.Web configures the Infrastructure implementation at the application composition root in `Program.cs`.

## Project structure
- Routable application pages are first-class presentation components under `Components/Pages`.
- Feature-specific implementation components are organised under `Components/Features`.
- Application-wide layout components are under `Components/Layout`, and reusable cross-feature UI components are under `Components/Shared`.

## Data access
- EF Core database operations used by interactive components use short-lived DbContext instances through `IDbContextFactory<TContext>`.

## Styling
- Use the MudBlazor theme, CSS isolation and application-level styling to implement the Heracles visual design.

## Theming
- Light and dark themes are first-class requirements.
- Colours are defined through the Heracles theme and semantic palette.

## Component model
- Pages compose feature components.
- Repeated Heracles visual patterns are implemented as shared Heracles components.
- Generic UI controls use MudBlazor directly.

## Component styling
- Component-specific styling uses colocated `.razor.css` CSS isolation.
- Application-wide styling is kept under `wwwroot`.

## Responsive design
- The UI supports desktop, tablet and mobile layouts.
- The existing desktop application provides the initial visual and information-architecture baseline.

## JavaScript
- Use Blazor for application interaction and state.
- Use JavaScript for browser and third-party functionality that requires JavaScript integration.

## JavaScript interop
- Component-specific JavaScript uses colocated `.razor.js` ES modules and explicit Blazor lifecycle management.
- Dedicated third-party component containers, such as the activity map, may be managed by their JavaScript integration.

## Navigation
- Use Blazor routing and navigation.

## Feature delivery
- Existing application behaviour provides the baseline for each feature.
- Enhancements may be considered separately as implementation progresses.

## Authentication
- Authentication will be introduced as a separate milestone.
- The initial Heracles.Web application will run without authentication.

## Logging
- Use the ASP.NET Core logging infrastructure and `Microsoft.Extensions.Logging`.
- Application components and services use `ILogger<T>` where logging is required.
- Logging levels are configured through application configuration.