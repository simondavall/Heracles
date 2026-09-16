This document provides a snapshot of the current implementation state of the project. 
It records completed work, the feature currently being developed, and the next expected 
steps. It should be updated regularly and is intended to help developers quickly understand 
where development should continue.

# Current phase

- Phase 1 – Application Foundation

# Current milestone

- Milestone 1.2 – Heracles Design System

# Current task

- Establish the Heracles theme.

# Remaining milestone tasks

- Establish reusable UI patterns.
- Establish theme switching.
- Establish responsive foundations.

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