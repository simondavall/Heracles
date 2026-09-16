This document records significant architectural and design decisions made during the lifetime of the Heracles.Web project together with the reasoning and trade-offs behind them.

It provides a historical record of why important implementation decisions were made and helps prevent previously resolved discussions from being revisited without new evidence.

2026-09-16

### Separate host authentication from common Infrastructure

#### Decision

Heracles.Web authenticates users through Soteria using OpenID Connect rather than the legacy ASP.NET Core Identity implementation used by the existing MVC Web application.

Common Infrastructure registration does not register a host authentication implementation.

The legacy ASP.NET Core Identity services, AppIdentityDbContext and Entity Framework Identity stores are registered through a separate Identity-specific Infrastructure registration. The existing MVC Web application explicitly opts into that registration.

Heracles.Web registers common Infrastructure without legacy Identity and configures Soteria authentication at its own composition root.

Heracles.Web uses cookie authentication for its local authenticated session and OpenID Connect as its challenge mechanism. Application access requires authentication by default through the authorization fallback policy.

#### Rationale

- Heracles.Web and the legacy MVC Web application use different authentication authorities.
- Authentication configuration is a host concern and should not be introduced implicitly by common Infrastructure registration.
- Registering both authentication implementations caused the legacy `Identity.Application` scheme to conflict with the Heracles.Web cookie authentication scheme.
- Relying on dependency-registration order to resolve competing authentication schemes would create hidden and fragile behaviour.
- Separating Identity registration makes each application's authentication dependency explicit.
- The legacy MVC application can continue using its existing Identity implementation while Heracles.Web migrates to Soteria.
- The separation removes the unused legacy Identity authentication dependency from Heracles.Web without requiring migration of the retiring MVC application's Identity implementation.
