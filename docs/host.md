# Custom domains

Modify the host file.

```
sudo nano /private/etc/hosts
```

Add your domains, including tenants.

```
127.0.0.1       acme.yourbrand.local
127.0.0.1       identity.yourbrand.local
```

This is what the file would approximately look like:

```
##
# Host Database
#
# localhost is used to configure the loopback interface
# when the system is booting.  Do not change this entry.
##
127.0.0.1       localhost
127.0.0.1       localhost
127.0.0.1       acme.yourbrand.local
127.0.0.1       identity.yourbrand.local
```

Then flush to apply the changes.

```
sudo dscacheutil -flushcache; sudo killall -HUP mDNSResponder
```