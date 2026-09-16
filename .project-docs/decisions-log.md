This document records significant architectural and design decisions made during the lifetime of the Heracles.Web project together with the reasoning and trade-offs behind them.

It provides a historical record of why important implementation decisions were made and helps prevent previously resolved discussions from being revisited without new evidence.

<example decision for structure and content>
<remove this once the first actual decision has been added>
2026-09-09

### Use a validated Taurus-owned application settings authority

#### Decision

Taurus represents application configuration through an immutable `TaurusSettings` model owned by `Taurus.Application`.

The Web composition root loads the configured ASP.NET Core configuration sources and creates `TaurusSettings` during startup.

Creation validates all represented settings and prevents application startup if any are missing or invalid. Validation failures are collected and reported together through a startup exception.

After successful creation, Web, Application and Infrastructure consume the validated settings model rather than independently reading raw configuration for migrated settings.

Settings will be migrated into the authority incrementally as existing configuration usage is reviewed.

#### Rationale

- Application code should be able to rely on required configuration being complete and valid.
- Central validation removes repeated null handling, parsing and fallback behaviour from individual consumers.
- Strongly typed settings make configuration dependencies explicit.
- Owning the settings model in Application allows both Web and Infrastructure to consume it without introducing an invalid project dependency.
- Passing the settings authority avoids increasingly large dependency-registration signatures containing individual configuration values.
- Constructing the authority in Web preserves configuration providers and startup as host responsibilities.
- Keeping `IConfiguration` at the composition boundary prevents lower layers from independently interpreting the same configuration.
- Incremental migration allows the pattern to be adopted without expanding the current task into an application-wide configuration refactoring.
