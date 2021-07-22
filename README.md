
# Qweree

## Docker setup

Services can be run as docker containers. Either individually with docker files located in `docker/` or with `docker-compose` command.

### docker-compose

Run all services by executing command in terminal.

```
   $ docker-compose up -d
```

Start chosen services by listing them.
```
   $ docker-compose up -d qweree-db qweree-auth
```

### Services

#### qweree-proxy
Nginx proxy

#### qweree-db
Mongo database

Connection string: `mongodb://localhost:27017`.

Inner connection string: `mongodb://qweree-db:27017`.

Use for example `mongo compass` application to explore database.

#### qweree-auth
Auth api

Swagger: `http://localhost/auth/swagger/index.html`

OAuth2 endpoint: `http://localhost/auth/api/oauth2/auth`

#### qweree-cdn
Cdn api

Swagger: `http://localhost/cdn/swagger/index.html`

### Credentials
- Init admin user - `admin`:`password`
- Init client - `admin-cli`:`password`
