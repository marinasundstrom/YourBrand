# Instructions

Nginx has been replaced by YARP - the Proxy project.

* Portal: https://localhost:5174/ (Actually the reverse proxy)
* Identity server: https://localhost:5040/

You need to create and seed ``--seed``:

* ApiKeys
* IdentityService
* AppService
* HumanResources

You have to create a company.

Default credentials:

```
admin@email.com
Abc123!?
```

## Syncing

Solution - will recreate everything:

    seed.sh

Individual projects - will recreate service and sync users:

    seed.sh --sync-users
