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

- ✓ Create the activity import page.
  - ✓ Support GPX files.
  - ✓ Support multiple file selection.
  - ✓ Support drag-and-drop.
  - ✓ Automatically initiate imports.
  - ✓ Configure file-count limits.
  - ✓ Configure combined file-size limits.
  - ✓ Integrate import limits with validated HeraclesSettings.
  - ✓ Implement asynchronous browser-file processing.
  - ✓ Coordinate file processing and persistence through the Application import service.
  - ✓ Preserve GPX validation and duplicate detection.
  - ✓ Persist imported activities using transactional bulk operations.
  - ✓ Display successfully imported file counts.
  - ✓ Display individual file failures and their reasons.
  - ✓ Display operation-level import errors.
  - ✓ Display import progress throughout processing and persistence.
  - ✓ Implement progress reporting through Blazor component state.
  - ✓ Prevent overlapping import operations.
  - ✓ Implement cancellation.
  - ✓ Implement theme-aware import presentation.
  - ✓ Verify large multi-file imports.

**Deliverable**

Users can import multiple GPX activity files directly through Heracles.Web using file selection or drag-and-drop. The application provides progress reporting, cancellation, duplicate detection and import results.

The complete import operation is coordinated through the Application layer, with GPX processing and transactional persistence provided by Infrastructure.

---

# Phase 4 – Activity Details Modules

## Milestone 4.1 – Modules

- ✓ Add weather details.
    - ✓ Integrate historical weather into Activity Title.
    - ✓ Implement the Application weather service.
    - ✓ Implement historical weather retrieval through Visual Crossing.
    - ✓ Implement historical weather retrieval through Open-Meteo.
    - ✓ Establish a provider-independent weather observation model.
    - ✓ Persist and reuse retrieved activity weather.
    - ✓ Implement weather condition and apparent-temperature presentation.
    - ✓ Verify weather retrieval and persistence through automated tests.
- ✓ Add Activity Type filtering to Activity Navigation.
    - ✓ Add an Activity Type selector to Activity Navigation.
    - ✓ Populate available Activity Types from distinct types present in activity data.
    - ✓ Include `All` as the unfiltered selection.
    - ✓ Persist the selected Activity Type through the existing browser-local UserState.
    - ✓ Filter activity year, month and activity-list queries by Activity Type.
    - ✓ Use the persisted Activity Type when selecting the most recent activity on application entry.
    - ✓ Preserve explicit activity routes independently of the Activity Type filter.
    - ✓ Keep Activity Navigation filtering independent of the currently displayed Activity Details.
    - ✓ Reset a persisted Activity Type to `All` when that type is no longer present in activity data.
- ✓ Add pace chart
    - ✓ Add configurable pace WindowRadius setting.
    - ✓ Persist cumulative distance and active elapsed time as derived activity data.
    - ✓ Generate missing cumulative track-point data lazily and reuse persisted data.
    - ✓ Exclude recording pauses and inter-segment geographic distance from cumulative track-point data.
    - ✓ Calculate pace using centred rolling windows over cumulative distance and active time.
    - ✓ Calculate pace observations at every eligible track point.
    - ✓ Shrink calculation windows at activity boundaries.
    - ✓ Render pace against cumulative activity distance using Chart.js.
    - ✓ Present a continuous pace series across recording segments.
    - ✓ Support responsive presentation and live light/dark theme changes.
    - ✓ Verify pace calculation, persistence and chart presentation.
- ✓ Add elevation chart
    - ✓ Generalise persisted cumulative pace data into reusable TrackPointData.
    - ✓ Introduce Application track-point data service and Infrastructure repository responsibilities.
    - ✓ Persist elevation alongside cumulative distance and active elapsed time.
    - ✓ Reuse existing TrackPointData when available and lazily generate it when absent.
    - ✓ Refactor pace calculation to consume shared TrackPointData.
    - ✓ Render recorded elevation against cumulative activity distance using Chart.js.
    - ✓ Display elevation in metres and cumulative activity distance in kilometres.
    - ✓ Support responsive presentation and live light/dark theme changes.
    - ✓ Verify track-point data generation and persistence, existing pace behaviour and elevation-chart presentation.
- ✓ Add speed chart
    - ✓ Reuse shared TrackPointData without changing its persisted representation.
    - ✓ Calculate speed from cumulative geographic distance and cumulative active elapsed time.
    - ✓ Apply the established centred rolling-window calculation behaviour.
    - ✓ Calculate speed observations at every eligible track point.
    - ✓ Preserve shrinking calculation windows at activity boundaries.
    - ✓ Render speed against cumulative activity distance using Chart.js.
    - ✓ Display speed in kilometres per hour.
    - ✓ Display the Speed chart for Cycling activities instead of the Pace chart.
    - ✓ Retain the Pace chart for non-Cycling activities.
    - ✓ Support responsive presentation and live light/dark theme changes.
    - ✓ Verify speed calculation and conditional Pace/Speed chart presentation.
- ✓ Synchronise Activity Details chart and map interaction.
    - ✓ Use TrackPointId as the common interaction identity for Pace, Speed, Elevation and Activity Map.
    - ✓ Remove Pace and Speed Stride configuration and calculate observations at every eligible track point.
    - ✓ Synchronise chart hover state between the visible Pace/Speed chart and Elevation chart.
    - ✓ Display synchronized chart tooltips and crosshairs.
    - ✓ Display the corresponding recorded TrackPoint location on the Activity Map.
    - ✓ Clear synchronized interaction state when chart hover ends or the selected activity changes.
    - ✓ Leave Pace/Speed unselected when the synchronized TrackPoint has no valid Pace/Speed observation.
    - ✓ Preserve existing chart responsiveness, theme behaviour and Activity Map presentation.
    - ✓ Verify synchronized interaction for Pace, Speed, Elevation and Activity Map.
- Add splits
- Add ranking listing

# Phase 5 – Dashboard and Reporting

## Milestone 5.1 – Reports

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

## Milestone 5.2 – Dashboard

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

# Enhancements

- ✓ Change database from SQL Server to sqlite3

Future enhancements outside the planned migration phases will be recorded here as the project evolves.
