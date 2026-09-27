# Heracles.Web Roadmap

This roadmap describes the high-level delivery phases for Heracles.Web.

The delivery plan defines the milestones, implementation tasks and delivery sequence within each phase.

---

# Phase 1 – Application Foundation

**Goal**

Establish the technical, architectural and presentation foundation for Heracles.Web.

**Includes**

- Establish the .NET 10 Blazor application structure.
- Configure MudBlazor.
- Integrate existing Application and Infrastructure functionality.
- Establish application configuration and logging.
- Integrate authentication and Data Protection.
- Establish browser-local user state.
- Establish the Heracles design system.
- Implement theme switching.
- Create the responsive application shell and primary navigation.
- Establish the staging environment.

**Deliverable**

A runnable Heracles.Web application with the technical and presentation foundation required for feature development.

---

# Phase 2 – Activity Management

**Goal**

Provide the core Heracles activity browsing and viewing experience.

**Includes**

- Activity navigation.
- Activity title.
- Activity metrics.
- Activity Map.
- Recorded activity routes.
- Geographic event markers.
- Activity distance markers.

**Deliverable**

Users can browse existing activities and view their principal activity information and recorded routes.

---

# Phase 3 – Activity Import

**Goal**

Provide the Heracles activity import experience.

**Includes**

- Multiple-file GPX import.
- File selection and drag-and-drop.
- Import validation and duplicate detection.
- Transactional persistence.
- Import progress and cancellation.
- Import results and error reporting.

**Deliverable**

Users can import multiple GPX activity files directly through Heracles.Web.

---

# Phase 4 – Activity Details Modules

**Goal**

Complete the additional activity detail modules.

**Includes**

- Weather details.
- Ranking listing.
- Splits.
- Pace chart.
- Elevation chart.

**Deliverable**

Users can view the additional information and visualisations associated with their activities.

---

# Phase 5 – Dashboard and Reporting

**Goal**

Provide the Heracles dashboard and reporting experience.

**Includes**

- Inventory and migrate existing reports.
- Establish shared reporting components.
- Implement report selection and filtering.
- Migrate report visualisations and tables.
- Preserve existing calculations.
- Implement the dashboard.
- Display activity summaries.
- Provide responsive presentation.

**Deliverable**

Users can view their activity information, summaries and reports through Heracles.Web.

---

# Phase 6 – User Preferences and Settings

**Goal**

Provide user preferences and required application settings functionality.

**Includes**

- Preserve existing theme preference functionality.
- Identify additional user-interface preferences.
- Establish the required preference storage strategy.
- Inventory existing application settings.
- Migrate required settings screens.
- Preserve existing settings behaviour.
- Verify settings validation and persistence.

**Deliverable**

Users can personalise Heracles.Web and manage the required application settings.

---

# Future Enhancements

Future enhancements outside the planned implementation phases are recorded in `backlog.md`.