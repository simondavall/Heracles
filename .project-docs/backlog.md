This document records planned enhancements, future ideas and technical debt. Items in this document represent potential future work rather than the current implementation priority. The backlog is intentionally broader than the current development roadmap.

# Enhancements

# Technical Debt

- Investigate initial theme flash (see Notes below)
- User-configurable Activity Map settings. (see Notes)

# Nice-to-have


# Notes

### Investigate initial theme flash

Investigate and address the visible theme transition during initial rendering and page refresh when Heracles.Web resolves the effective light or dark theme.

Treat this as an initial rendering/lifecycle concern rather than extending the existing theme preference implementation.

The solution should avoid duplicating theme ownership between startup JavaScript/CSS and MudBlazor.

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
