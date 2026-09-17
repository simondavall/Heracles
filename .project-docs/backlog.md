This document records planned enhancements, future ideas and technical debt. Items in this document represent potential future work rather than the current implementation priority. The backlog is intentionally broader than the current development roadmap.

# Enhancements

# Technical Debt
- Change Infrasturcture registration to use Heracles settings when Web is decommissioned. Web currently passes the
IConfiguration whereas Heracles.Web will send Heracles settings. Heracles.Web will continue to send IConfiguration
until Web is no longer required.
- Investigate initial theme flash (see Notes below)
- 
# Nice-to-have


# Notes

### Investigate initial theme flash

Investigate and address the visible theme transition during initial rendering and page refresh when Heracles.Web resolves the effective light or dark theme.

Treat this as an initial rendering/lifecycle concern rather than extending the existing theme preference implementation.

The solution should avoid duplicating theme ownership between startup JavaScript/CSS and MudBlazor.