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

## Authentication

Heracles.Web authenticates users through Soteria which is a self-hosted IAM system using OpenID Connect.
