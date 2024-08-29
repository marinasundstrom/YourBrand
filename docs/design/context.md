# Context

## Tenant

Provided via ``IUserContext``. Check "User".

## Organization

By injecting the service ```IOrganizationProvider``` you can access the current organization, subscribe to when the organization context changes, and to set new organization.

```csharp
@inject IOrganizationProvider OrganizationProvider

//...

var organization = await OrganizationProvider.GetCurrentOrganizationAsync();
```

If you just want access the organization in a component, then you can inject the Organization as a cascading parameter.

```csharp
<span>Current org: @Organization?.Name</span>

@code 
{
    [CascadingParameter(Name = "Organization")]
    public YourBrand.Portal.Services.Organization? Organization { get; set; }
}
```

This will automatically get updated when a new organization is set.

## User

User information can be obtained via ```IUserContext```.

```csharp
var userId = userContext.GetUserId();

var tenantId = userContext.GetTenantId();
```