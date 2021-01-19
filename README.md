
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
Mongo db

Connection string: `mongodb://localhost:27017`.

Inner connection string: `mongodb://qweree-db:27017`.

Use for example `mongo compass` application to explore database.

#### qweree-auth
Auth api

Swagger: `http://localhost:10001/swagger/index.html`

OAuth2 endpoint: `http://localhost:10001/api/oauth2/auth`

#### qweree-cdn
Cdn api

Swagger: `http://localhost:10002/swagger/index.html`

#### qweree-qwill
An api for qwill application.

Swagger: `http://localhost:10002/swagger/index.html`

#### qweree-qadmin
Angular admin for micro services qweree-auth and qweree-cdn. Available at `http://localhost:11001`

#### qweree-qwill-app
Angular qwill application. Available at `http://localhost:11000`

### Credentials
- Init admin user - `admin`:`password`