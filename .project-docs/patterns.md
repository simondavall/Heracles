This document records proven implementation patterns that have emerged during development. Unlike decisions.md, which defines project rules, this document provides practical examples of how common features are implemented. New features should follow these patterns where appropriate to maintain consistency across the application.

## MudBlazor Component Styling with CSS Isolation

Use MudBlazor's theme and component APIs where they directly represent the required presentation.

Keep component-specific CSS in the owning component's colocated `.razor.css` file.

When component-specific CSS must target HTML rendered internally by a MudBlazor component, introduce a minimal Heracles-owned wrapper:

```html
<div class="application-header">
    <MudAppBar Elevation="0"
               Fixed="true">
        ...
    </MudAppBar>
</div>
```

Target the MudBlazor-rendered element from that owned CSS-isolation boundary using ::deep:

```css
.application-header ::deep .mud-appbar {
border-bottom: 1px solid #35404a;
}
```

Do not move the rule to application-wide CSS solely to bypass CSS isolation.

Use this pattern only when the required presentation cannot be expressed cleanly through the MudBlazor theme or component API. Keep the wrapper and ::deep path as narrow as practical.
