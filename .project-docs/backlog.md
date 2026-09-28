This document records planned enhancements, future ideas and technical debt. Items in this document represent potential future work rather than the current implementation priority. The backlog is intentionally broader than the current development roadmap.

# Enhancements

# Technical Debt

- Investigate initial theme flash (see Notes below)
- User-configurable Activity Map settings. (see Notes)
- Review weather cache behaviour when changing providers (see Notes).
- Review configuration-based weather provider selection (see Notes).
- Add OpenMeteo attribution before releasing.

# Nice-to-have


# Notes

### Investigate initial theme flash

Investigate and address the visible theme transition during initial rendering and page refresh when Heracles.Web resolves the effective light or dark theme.

Treat this as an initial rendering/lifecycle concern rather than extending the existing theme preference implementation.

The solution should avoid duplicating theme ownership between startup JavaScript/CSS and MudBlazor.

### Review weather cache behaviour when changing providers

Currently cached weather is retained when switching between Open-Meteo and Visual Crossing. Review whether provider changes should invalidate previously cached observations. Preserve the existing cache behaviour until an explicit decision is made.

### Review configuration-based weather provider selection

Currently changing the active weather provider requires changing the Infrastructure DI registration and redeploying. Review selecting the provider through configuration at application startup, with provider-specific settings. Runtime switching and provider factories are not required by the current use case.

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
