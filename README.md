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

#### qweree-db
Connection string: `mongodb://localhost:27017`.

Inner connection string: `mongodb://qweree-db:27017`.

Use for example `mongo compass` application to explore database.

#### qweree-auth
Swagger: `http://localhost:8080/swagger/index.html`

OAuth2 endpoint: `http://localhost:8080/api/oauth2/auth`

Inner oauth2 endpoint: `http://qweree-auth:80/api/oauth2/auth`

#### qweree-cdn
Swagger: `http://localhost:8090/swagger/index.html`

### Credentials
- Init admin user - `admin`:`password`