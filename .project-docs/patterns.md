This document records proven implementation patterns that have emerged during development. Unlike decisions.md, which defines project rules, this document provides practical examples of how common features are implemented. New features should follow these patterns where appropriate to maintain consistency across the application.

<example entry>
<remove this once an actual pattern has been added>
## Application Theme

- Define the application theme in a dedicated `TaurusTheme` class.
- Expose the theme through a static `Default` property.
- Apply the theme globally using the root `MudThemeProvider`.
- Use MudBlazor Material icons directly where required.
- Introduce semantic application icon identifiers only if repeated usage demonstrates a useful shared vocabulary.
- Keep feature components theme-aware by using MudBlazor semantic colours (`Color.Primary`, `Color.Secondary`, `Color.Success`, etc.) instead of hard-coded colour values.
