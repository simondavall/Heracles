# Heracles.Web Delivery Plan

This document expands the roadmap into implementation milestones and tasks.

Unlike the roadmap, which describes the long-term delivery of the project, this document records the expected sequence of implementation work.

The roadmap should remain relatively stable.

The delivery plan is expected to evolve as implementation progresses and understanding of the existing Heracles application increases.

Tasks listed here are intentionally concise. Detailed scope, goals and implementation notes belong in the associated development task.

Heracles.Web is focused solely on replacing the existing Heracles Web UI with a new .NET 10 Blazor application.

The existing Heracles application is the reference implementation for behaviour and information architecture.

---

# Phase 1 – Application Foundation

## Milestone 1.1 – Project Foundation

- ✓ Create the Heracles.Web application.
  - ✓ Create the .NET 10 Blazor Web App project.
  - ✓ Configure application-wide Interactive Server rendering.
  - ✓ Reference the Application project.
  - ✓ Configure the Infrastructure project at the application composition root.
  - ✓ Configure dependency injection.
  - ✓ Verify the application builds.
  - ✓ Verify the application runs successfully.

- ✓ Configure MudBlazor.
  - ✓ Add MudBlazor 9.9.0.
  - ✓ Configure the required MudBlazor services.
  - ✓ Configure the required MudBlazor providers.
  - ✓ Verify MudBlazor components render correctly.

- ✓ Establish the project structure.
  - ✓ Establish the feature component structure.
  - ✓ Establish the shared component structure.
  - ✓ Establish the layout component structure.
  - ✓ Establish JavaScript isolation conventions.
  - ✓ Establish component CSS isolation conventions.
  - ✓ Verify the structure supports feature development.

- ✓ Establish application data access.
  - ✓ Integrate the existing application services.
  - ✓ Configure EF Core.
  - ✓ Configure DbContext factory usage.
  - ✓ Verify database access from Interactive Server components.
  - ✓ Establish application logging.

- ✓ Create the initial project documentation.
  - ✓ Create the project document.
  - ✓ Create the architecture document.
  - ✓ Create the decisions document.
  - ✓ Create the roadmap.
  - ✓ Create the delivery plan.
  - ✓ Create the backlog.

**Deliverable**

A clean, runnable Heracles.Web Blazor application with the technical foundations required for feature development.

---

## Milestone 1.2 – Heracles Design System

- ✓ Establish the Heracles theme.
  - ✓ Define the primary colour palette.
  - ✓ Define the light palette.
  - ✓ Define the dark palette.
  - ✓ Configure typography.
  - ✓ Configure icons.
  - ✓ Configure standard spacing.
  - ✓ Configure borders and elevation.
  - ✓ Establish semantic colours for success, warning, error and information.

- ✓ Establish theme switching.
  - ✓ Support light mode.
  - ✓ Support dark mode.
  - ✓ Support system theme preference.
  - ✓ Verify shared components in each theme.

**Deliverable**

Heracles.Web has a reusable visual design system supporting light and dark themes and responsive application development.

---

## Milestone 1.3 - Foundation services

- ✓ Implement DotEnv
- ✓ Implement Authentication via Soteria
- ✓ Implement DataProtection
- ✓ Implement Serilog
- ✓ Implement HeraclesSettings
- ✓ Implement LocalStorage for user state
- ✓ Persist and simplify theme preference
- ✓ Create staging environment

**Deliverable**

A clean, runnable Heracles.Web Blazor application with authentication, logging, data protection with working local production environment.

---

## Milestone 1.4 – Application Shell

```text
┌──────────────────────────────────────────────────────────────┐
│ Global top AppBar: Heracles                         Logout   │
└──────────────────────────────────────────────────────────────┘
            ┌───────────────────────────────────────┐
            │         Primary Navigation            │
            ├───────────────────────────────────────┤
            │                                       │
            │                                       │
            │                                       │
            │         Main Content Area             │
            │                                       │
            │                                       │
            │                                       │
            │                                       │
            └───────────────────────────────────────┘
```

- ✓ Create the main application layout.
  - ✓ Create the Heracles application header.
  - ✓ Create the main application body
    - ✓ Create primary application navigation
    - ✓ Create the main content area
- ✓ Verify desktop, tablet and mobile layouts.

**Deliverable**

A recognisable Heracles application shell providing the shared layout and navigation for all application features.

---

# Phase 2 – Activity Management

## Milestone 2.1 – Activity Navigation

- ✓ Create the activity navigation component.
  - ✓ Display activity years.
  - ✓ Display activity counts by years.
  - ✓ Expand and collapse activity years.
  - ✓ Display activity months.
  - ✓ Display activity counts by months.
  - ✓ Expand and collapse activity months.
  - ✓ Load activities for the selected month.
  - ✓ Display activity summaries.
  - ✓ Highlight the selected activity.
  - ✓ Navigate between activities using Blazor routing.
  - ✓ Add activity navigation loading states.
  - ✓ Add activity navigation empty states.
  - ✓ Implement activity navigation interactions using Blazor component state.

**Deliverable**

Users can browse and select their existing activities using a native Blazor activity navigator.

---

## Milestone 2.2 – Activity Details

- ✓ Create the activity title component.
  - ✓ Display Activity icon.
  - ✓ Display Title
  - ✓ Display Date and Time.
- ✓ Create reusable activity metric components.
  - ✓ Display activity distance.
  - ✓ Display activity duration.
  - ✓ Display average pace.
  - ✓ Display activity rank.
- ✓ Verify light and dark theme presentation.

**Deliverable**

Users can view the principal information for an activity using the new Heracles.Web component architecture.

---

## Milestone 2.3 – Activity Map

- ✓ Create the initial Activity Map component.
  - ✓ Integrate Mapbox GL JS.
  - ✓ Display recorded activity routes.
  - ✓ Preserve independent recording segments.
  - ✓ Display start, finish, pause and resume markers.
  - ✓ Automatically fit the viewport to the selected activity.
  - ✓ Support activity navigation without recreating the map.
  - ✓ Implement light and dark map presentation.
  - ✓ Implement responsive presentation.
  - ✓ Manage Mapbox initialisation and disposal.
  - ✓ Implement fade transitions when switching activities.

- ✓ Implement activity distance markers.
  - ✓ Calculate cumulative distance across recording segments.
  - ✓ Exclude distance accumulated during recording pauses.
  - ✓ Calculate marker positions at whole-kilometre intervals.
  - ✓ Interpolate marker positions between recorded GPS coordinates.
  - ✓ Handle zero-distance GPS points.
  - ✓ Extend the Activity Map presentation contract.
  - ✓ Introduce a dedicated GeoJSON source and symbol layer.
  - ✓ Generate distance-marker SVG images dynamically.
  - ✓ Display distance values and kilometre labels.
  - ✓ Integrate distance markers with existing map transitions.
  - ✓ Verify distance-marker presentation and behaviour.

**Deliverable**

Users can view recorded activity routes and significant geographic events through a lifecycle-safe 
Blazor map component, including distance markers consistent with the recorded activity distance.

---

# Phase 3 – Activity Import

## Milestone 3.1 – File Import

- Create the activity import page.
  - Support only GPX files.
  - Support multiple file selection.
  - Configure file-count limits.
  - Configure file-size limits.
  - Integrate browser file selection with the existing import services.
  - Import selected activity files.
  - Persist imported activities.
  - Display successfully imported file counts.
  - Display failed imports.
  - Display import errors.
  - Display selected file information.
  - Display import progress.
  - Ensure progress updates are rendered during processing.
  - Display individual file failures.
  - Display final import results.
  - Prevent duplicate import operations.
  - Handle cancellation where supported.
  - Review memory usage during multi-file imports.
  - Verify large multi-file imports.
  - Implement the complete upload workflow in Blazor.
  - Implement import progress through Blazor component state.

**Deliverable**

Users can import supported activity files directly through the Blazor application. Users receive clear progress and results while importing multiple activity files.

---

# Phase 4 – Dashboard and Reporting

## Milestone 4.1 – Dashboard

- Inventory the existing dashboard functionality.
- Identify dashboard information to preserve.
- Create the dashboard page.
- Create reusable dashboard components.
- Display activity summary information.
- Implement dashboard loading and empty states.
- Verify responsive presentation.
- Verify light and dark theme presentation.

**Deliverable**

Users can view their principal Heracles information from the new dashboard.

---

## Milestone 4.2 – Reports

- Inventory existing reports.
- Establish shared report components.
- Migrate report selection.
- Migrate report filtering.
- Migrate report visualisations.
- Migrate report tables.
- Preserve existing calculations.
- Verify report results against the existing application.
- Verify responsive report presentation.

**Deliverable**

Existing Heracles reporting capabilities are available through Heracles.Web.

---

## Milestone 4.3 – Complete Activity Experience

- Inventory remaining activity functionality in the existing application.
- Migrate required remaining activity information.
- Migrate activity editing where required.
- Migrate activity deletion where required.
- Implement activity loading feedback.
- Implement activity error handling.
- Optimise activity database queries.
- Verify activity navigation behaviour.
- Verify responsive activity layouts.
- Verify existing Heracles activity behaviour is preserved where required.

**Deliverable**

The existing Heracles activity workflow is available through the new Heracles.Web interface.

---

# Phase 5 – User Experience

## Milestone 5.1 – Responsive Experience

- Optimise desktop layouts.
- Optimise tablet layouts.
- Optimise mobile layouts.
- Refine responsive navigation.
- Optimise activity navigation for smaller displays.
- Optimise maps and reports for smaller displays.

**Deliverable**

Heracles.Web provides a coherent responsive experience across supported devices.

---

## Milestone 5.2 – Application Experience

- Improve application navigation.
- Improve loading feedback.
- Improve validation presentation.
- Improve error presentation.
- Improve empty-state presentation.
- Improve visual consistency.
- Review keyboard navigation.
- Review accessibility.
- Review application performance.
- Review perceived performance.

**Deliverable**

Heracles.Web provides a polished, consistent and accessible application experience.

---

# Phase 6 – User Preferences and Settings

## Milestone 6.1 – User Preferences

- Persist theme preference.
- Apply system theme preference.
- Preserve preferences across sessions.
- Identify additional user-interface preferences.
- Establish preference storage strategy.

**Deliverable**

Users can personalise their Heracles.Web experience and retain those preferences between sessions.

---

## Milestone 6.2 – Application Settings

- Inventory existing Heracles settings.
- Migrate required settings screens.
- Implement settings UI using the Heracles.Web component architecture.
- Preserve existing settings behaviour.
- Verify settings validation.
- Verify settings persistence.

**Deliverable**

Required Heracles application settings are available through Heracles.Web.

---

# Phase 7 – Migration and Production Readiness

## Milestone 7.1 – Legacy Feature Parity

- Inventory remaining Web UI features.
- Inventory remaining interactive UI behaviour.
- Inventory remaining JavaScript integrations.
- Inventory remaining visual and layout requirements.
- Migrate required remaining functionality.
- Verify required existing behaviour.
- Verify all required Web UI features are implemented in Heracles.Web.
- Verify all required interactive behaviour is implemented.
- Verify all required visual and layout behaviour is implemented.

**Deliverable**

Heracles.Web provides the functionality required to replace the existing Heracles web application.

---

## Milestone 7.2 – Application Hardening

- Review authentication and authorisation.
- Review application security.
- Review database access patterns.
- Review database indexes and query performance.
- Review server-side circuit resource usage.
- Review JavaScript interop lifecycle management.
- Review file-upload resource usage.
- Implement automated tests for critical workflows.
- Perform supported-browser testing.
- Perform responsive-device testing.
- Perform accessibility testing.
- Review application logging and diagnostics.

**Deliverable**

Heracles.Web is suitable for production deployment.

---

## Milestone 7.3 – Production Cutover

- Configure the production environment.
- Configure production database connectivity.
- Configure production authentication.
- Configure production Mapbox configuration.
- Configure persistent Data Protection keys where required.
- Configure application logging.
- Deploy Heracles.Web alongside the existing application.
- Perform production smoke testing.
- Verify critical workflows.
- Switch users to Heracles.Web.
- Retain rollback capability during the initial cutover period.
- Complete the Heracles.Web production cutover.

**Deliverable**

Heracles.Web replaces the legacy Heracles MVC application in production.

---

# Enhancements

Future enhancements outside the planned migration phases will be recorded here as the project evolves.

Enhancements accepted during delivery will be added here or incorporated into the appropriate milestone.
