# Architecture overview

Heracles is a .NET application for importing, managing and analysing activity data.

The current project introduces Heracles.Web as a new .NET 10 Blazor Web App replacing the existing MVC Web user interface.

Heracles.Web provides the presentation and interactive application experience while continuing to use the existing Heracles Application, Domain and Infrastructure functionality.

Heracles.Web uses Interactive Server rendering. UI interactions execute on the server and the browser maintains an interactive Blazor connection to the application.

## System context

The relevant application structure is:

```text
User
  |
  | Uses Heracles through a browser
  v
Heracles.Web
  |
  | Uses application services and interfaces
  v
Application
  |
  | Uses domain concepts and infrastructure abstractions
  v
Domain / Infrastructure
  |
  | Persists and retrieves Heracles data
  v
SQL Server
```

Heracles.Web is responsible for the user-facing application while the existing application layers continue to provide the underlying Heracles functionality.

## Application structure

The relevant projects are:

```text
Heracles.Web
  ├── Application
  │     └── Domain
  └── Infrastructure
        ├── Application
        └── Domain
```

### Heracles.Web

Heracles.Web is the new application host and presentation layer.

It owns:
- Blazor pages and components.
- MudBlazor presentation.
- Application layout and navigation.
- Responsive UI behaviour.
- Interactive UI state.
- User-facing validation and feedback.
- Browser and third-party JavaScript integration.
- Application composition and hosting.

Heracles.Web consumes application functionality through interfaces exposed by the Application project.

Infrastructure is referenced at the application composition root so its implementations can be registered with dependency injection.

### Application

The existing Application project provides application services and interfaces consumed by Heracles.Web.

Heracles.Web treats these interfaces as the boundary through which it accesses existing Heracles application functionality.

### Domain

The existing Domain project contains the domain concepts used by the Heracles application.

Heracles.Web does not require a direct dependency on Domain unless a demonstrated implementation requirement establishes one.

### Infrastructure

The existing Infrastructure project provides technical implementations required by the application, including persistence.

Infrastructure implementations are registered by the Heracles.Web composition root and consumed through application abstractions.

## Dependency direction

Heracles.Web components depend on Application abstractions rather than concrete Infrastructure implementations.

The Web application composition root is responsible for connecting those abstractions to their implementations.

The normal UI dependency flow is:

```text
Blazor page or component
        |
        v
Application interface
        |
        v
Application service
        |
        v
Infrastructure abstraction
        |
        v
Infrastructure implementation
        |
        v
Persistence
```

## Rendering model

Heracles.Web uses global Interactive Server rendering.

Application interaction executes on the server while Blazor maintains the interactive connection with the browser.

This allows components to use existing server-side Heracles application services directly through dependency injection.

## Data access

Heracles data is persisted using Entity Framework Core and SQL Server.

Interactive components access data through the existing application services rather than directly through Entity Framework Core.

EF Core operations use short-lived DbContext instances created through IDbContextFactory<TContext>.

This avoids retaining a shared scoped DbContext for the lifetime of an Interactive Server circuit.

## Web application structure

Heracles.Web separates routable presentation from feature implementation.

```text
Heracles.Web
└── Components
    ├── Features
    ├── Layout
    ├── Pages
    └── Shared
```

### Pages

Components/Pages contains routable application screens.

Pages form the primary presentation surface and compose the feature components required to implement each screen.

### Features

Components/Features contains components implementing specific Heracles functionality.

Feature structure is introduced as functionality is implemented rather than being created speculatively.

### Layout

Components/Layout contains application-wide structural UI including the application shell, header and navigation.

### Shared

Components/Shared contains reusable Heracles UI concepts that are used across features.

Shared components are introduced where demonstrated reuse establishes a common application concept.

Generic UI controls are provided directly by MudBlazor.

## Styling and theming

MudBlazor provides the primary UI component framework.

The Heracles visual design is implemented through:

- The Heracles MudBlazor theme.
- Light and dark semantic palettes.
- Component-specific CSS isolation.
- Application-level styling.

Component-specific styling is colocated using .razor.css files.

Application-wide styling and static assets are maintained under wwwroot.

## Responsive design

Heracles.Web supports desktop, tablet and mobile layouts.

The existing desktop Web application provides the initial reference for information architecture and established application behaviour.

Responsive layouts may adapt presentation and interaction to suit the available viewport while preserving the underlying functionality.

## JavaScript integration

Blazor owns normal application interaction and state.

JavaScript is used where browser functionality or third-party libraries require JavaScript integration.

Component-specific JavaScript uses colocated .razor.js ES modules with explicit Blazor lifecycle management.

Dedicated third-party component containers, such as the activity map, may allow their JavaScript integration to manage the DOM within the component-owned container.

## Navigation

Heracles.Web uses Blazor routing and navigation.

Routable application screens are implemented under Components/Pages.

## Authentication

Heracles.Web authenticates users through Soteria using OpenID Connect.

Heracles.Web uses the OpenID Connect authorization-code flow with PKCE and establishes the local authenticated application session using cookie authentication.

Application routes require authentication by default through the ASP.NET Core authorization fallback policy. Endpoints that must be accessible without authentication explicitly allow anonymous access.

Authentication state is cascaded to Blazor components so application UI can respond to the authenticated user.

Heracles.Web owns its authentication configuration at the application composition root.

The legacy ASP.NET Core Identity implementation used by the existing MVC Web application is registered separately from common Infrastructure. The existing MVC application explicitly opts into that Identity infrastructure while Heracles.Web uses Soteria.

## Data Protection

Heracles.Web uses ASP.NET Core Data Protection for application data protection, including authentication-cookie protection.

The Data Protection key ring is persisted to a configurable filesystem location so protected data can remain valid across application restarts and deployments.

Persisted Data Protection keys are encrypted at rest using a dedicated X.509 certificate containing a private key.

The Data Protection certificate is separate from the HTTPS/TLS certificate.

The key-ring path, certificate path and certificate password are supplied through environment-based configuration. Local execution uses DotNetEnv while hosted environments use their normal environment-variable configuration mechanism.

The Data Protection implementation does not depend on a platform-specific certificate store or operating-system-specific key protection mechanism and is intended to operate consistently across Windows and Linux environments.

Heracles.Web uses a stable Data Protection application name of `Heracles.Web`.

Data Protection configuration and certificate loading are validated during application startup so invalid or incomplete configuration prevents the application from starting.

## Logging

Heracles.Web uses Serilog as its logging implementation.

Application code continues to consume logging through `ILogger<T>` so logging remains integrated with the standard .NET dependency-injection and logging abstractions.

Serilog configuration is environment-specific and supplied through application configuration.

The Development environment writes logs to the console. The Production environment writes logs to daily rolling files with a 31-file retention limit.

Logging levels and category overrides are configured independently for each environment so logging granularity can be appropriate to the environment.

The Production log-file path is supplied through environment-based configuration rather than committed application configuration. Local execution uses DotNetEnv while hosted environments use their normal environment-variable configuration mechanism.
