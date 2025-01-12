# Contexts

Each service is capable to handling more than one tenant. So each request belongs to potentially a different tenant, and user.

The request context is the context within the current request: What the tenant is, and who the authenticated user is.

It is represented by two services that are linked: the ``TenantContext`` and the ``UserContext``.

The "current" tenant refers to the tenant of the user that has been authenticated within the current requests.

In normal cases, the tenant is retrieved from the authenticated user.

## TenantContext

Provides the context of the current tenant, from which you can retrieve the Tenant Id.

This service is scoped to the current request, meaning that it is isolated to the context of the authenticated user.

Who is the current tenant is dependant on the authorized identity, and to which tenant it belongs.

The default implementation gets the Tenant Id from the "tenant_id" claim via the ``HttpContext``. This one is used when communicating via HTTP APIs.

There is a ``ISettableTenantContext`` that is mainly used internally at the service boundaries.

When an asynchronous message is being sent, or published, the Tenant Id is passed with it in the headers without you having to worry about it. And then automatically set at the consuming side.

## UserContext

Provider the context of the current user, from which you can retrieve the User Id.

This service is scoped to the current request, meaning that it is isolated to the context of the authenticated user.

Who is the current user is dependant on the authorized identity. And the user is tied to the current tenant.

The default implementation gets the User Id from the "sub" claim via the ``HttpContext``. This one is used when communicating via HTTP APIs.

There is a ``ISettableUserContext`` that is mainly used internally at the service boundaries.

When an asynchronous message is being sent, or published, the User Id is passed with it in the headers without you having to worry about it. And then automatically set at the consuming side.

## Notes on usage of contexts

You will not likely have to retrieve the ``TenantContext`` or ``UserContext`` in most cases.

These services are mostly used internally. Although you might sometimes want to know what the current tenant or current user is. 

You generally don't have to worry about setting who created what since that is automatically set for entities when they are persisted by the ORM.