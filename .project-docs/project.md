This document provides a high-level overview of the project. It describes what is being built, why it exists, the technologies it uses, and the overall design philosophy. It should help a developer understand the purpose and scope of the project before looking at implementation details.

# Project overview

Heracles.Web is a modern replacement for the existing Heracles Web application.

Heracles is an existing activity-tracking application with established Application, Domain and Infrastructure projects. Rather than incrementally modernising the existing MVC user interface, this project replaces the Web UI with a new .NET 10 Blazor application while continuing to use the existing Heracles application functionality.

The existing Web application provides the initial reference for behaviour and information architecture. The replacement will preserve the established Heracles functionality while modernising the presentation, interaction and responsive experience.

The project is focused on the Heracles.Web replacement. The existing Application, Domain and Infrastructure projects continue to provide the underlying application functionality.

The project aims to provide a clean, maintainable and responsive Web application that is straightforward to understand, extend and support. The implementation favours explicit behaviour and pragmatic design over unnecessary abstraction.

# Goals

The primary goals of the project are:

- Replace the existing Heracles Web UI with Heracles.Web.
- Preserve existing Heracles functionality and workflows.
- Modernise the user interface using Blazor and MudBlazor.
- Provide light and dark application themes.
- Provide a responsive experience across desktop, tablet and mobile layouts.
- Establish clear and maintainable UI component boundaries.
- Integrate cleanly with the existing Heracles application functionality.
- Build a foundation that supports future enhancements without introducing unnecessary complexity.

# Technology stack

## Application

- .NET 10
- ASP.NET Core Blazor Web App
- Interactive Server rendering
- MudBlazor 9.9.0

## Data

- Entity Framework Core
- SQL Server

## Frontend

- Razor components
- MudBlazor
- CSS isolation
- JavaScript ES modules where browser or third-party integration requires JavaScript

## Technology Guidance

The technologies and versions listed above are the source of truth for this project.

When suggesting framework-specific implementations:

- Verify behaviour against the versions used by the project rather than relying on memory.
- Do not recommend APIs or features that are unavailable in the project's versions.
- Where official framework documentation conflicts with prior knowledge, prefer the documentation for the project's versions.

# High-level architecture

Heracles.Web is the presentation application for the existing Heracles application.

Heracles.Web consumes application functionality through the existing Application interfaces. Infrastructure implementations are configured at the Heracles.Web application composition root.

The relevant dependency structure is:

```text
Heracles.Web
  ├── Application
  │     └── Domain
  └── Infrastructure
        ├── Application
        └── Domain
```

Heracles.Web components consume Application abstractions rather than Infrastructure implementations.

# Solution structure

```text
Heracles
│
├── src
│   ├── Application
│   ├── Domain
│   ├── Infrastructure
│   ├── Web
│   └── Heracles.Web
│       └── Components
│           ├── Features
│           ├── Layout
│           ├── Pages
│           └── Shared
│
├── tests
│
└── .project-docs
```

# Design principles

The project values:

- Explicit behaviour over hidden behaviour.
- Readability over cleverness.
- Purposeful abstraction over repeated implementation.
- Small, focused components.
- Clear ownership of responsibilities.
- Responsive design from the outset.
- Consistent user experience.
- Proven implementation patterns over speculative abstractions.

# Documentation overview

| Document | Purpose |
|----------|---------|
| project.md | What the project is. |
| architecture.md | How the application is structured. |
| decisions.md | Architectural decisions and project rules. |
| roadmap.md | What capabilities are delivered, and in what order. |
| delivery-plan.md | How we intend to deliver the current and upcoming work. |
| current-state.md | Current progress and next feature. |
| completed-development.md | Record of completed implementation work. |
| patterns.md | Proven implementation patterns. |
| backlog.md | Future work and enhancements. |
| collaboration.md | How the assistant should collaborate. |
| coding-conventions.md | Coding style and conventions. |
| decisions-log.md | Record of important architectural decisions. |
