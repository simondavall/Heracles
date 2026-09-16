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

# Logging

- Use the ASP.NET Core logging infrastructure and `Microsoft.Extensions.Logging`.
- Use `ILogger<T>` for application-level logging.
- Configure logging levels through application configuration.

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
