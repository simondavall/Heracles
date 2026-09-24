This document records planned enhancements, future ideas and technical debt. Items in this document represent potential future work rather than the current implementation priority. The backlog is intentionally broader than the current development roadmap.

# Enhancements

# Technical Debt
- Change Infrasturcture registration to use Heracles settings when Web is decommissioned. Web currently passes the
IConfiguration whereas Heracles.Web will send Heracles settings. Heracles.Web will continue to send IConfiguration
until Web is no longer required.
- Investigate initial theme flash (see Notes below)
- Remove legacy activity presentation metadata from Application (see Notes)
- User-configurable Activity Map settings. (see Notes)

# Nice-to-have


# Notes

### Investigate initial theme flash

Investigate and address the visible theme transition during initial rendering and page refresh when Heracles.Web resolves the effective light or dark theme.

Treat this as an initial rendering/lifecycle concern rather than extending the existing theme preference implementation.

The solution should avoid duplicating theme ownership between startup JavaScript/CSS and MudBlazor.

### Remove legacy activity presentation metadata from Application

`ActivityType` currently retains `ActivityTitleImageAttribute` and `ActivityTitleTextAttribute` because the legacy MVC Web application depends on them.

Heracles.Web does not consume this presentation metadata and owns its own activity title presentation mapping.

When the legacy Web application is decommissioned, remove:

- `ActivityTitleImageAttribute`;
- `ActivityTitleTextAttribute`;
- the corresponding attributes from `ActivityType`;
- any legacy extension methods or other code used solely to consume those attributes.

Do not perform this cleanup while the legacy Web application remains dependent on the metadata.

## User-configurable Activity Map settings

Introduce user-configurable presentation settings for the Activity Map.

Potential settings include:

- Distance units: kilometres or miles.
- Distance-marker visibility.
- Distance-marker interval.
- Distance-marker background colour.
- Distance-marker size.
- Additional map presentation preferences.

Replace the current fixed distance-marker presentation values with
configurable settings where appropriate.

Support regeneration of dynamically generated SVG marker images
when relevant settings change.

Determine the appropriate settings model and persistence mechanism
as part of the application-wide settings implementation.

This work is deferred and is not part of Milestone 2.3.
