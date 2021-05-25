conn = new Mongo();
db = conn.getDB("qweree_auth");
db.users.insert({
    "_id": UUID("8caa1d0c-401d-42d7-a726-943a30b73373"),
    "Username": "admin",
    "Password": "$2a$11$BwiSQi56B5UkW4SywF07g.jB3AA0GzHD1YsCWPLMzUlJi9UQUborC",
    "ContactEmail": "admin@admin.com",
    "FullName": "Admin Adminowitch",
    "Roles": [
        "AUTH_MANAGE",
        "AUTH_USERS_CREATE",
        "AUTH_USERS_READ",
        "AUTH_USERS_DELETE",
        "AUTH_USERS_READ_PERSONAL_DETAIL",
        "AUTH_CLIENTS_CREATE",
        "AUTH_CLIENTS_READ",
        "AUTH_CLIENTS_DELETE",
        "AUTH_ROLES_CREATE",
        "AUTH_ROLES_READ",
        "AUTH_ROLES_MODIFY",
        "AUTH_ROLES_DELETE",
        "CDN_MANAGE",
        "CDN_EXPLORE",
        "CDN_STORAGE_STORE",
    ],
    "CreatedAt": ISODate("1970-01-01"),
    "ModifiedAt": ISODate("1970-01-01")
});
db.clients.insert({
    "_id": UUID("9e8fc855-4feb-4c27-8903-f4fed64bf243"),
    "ClientId": "swagger",
    "ClientSecret": "$2a$11$BwiSQi56B5UkW4SywF07g.jB3AA0GzHD1YsCWPLMzUlJi9UQUborC",
    "ApplicationName": "Swagger",
    "Origin": "localhost",
    "OwnerId": "8caa1d0c-401d-42d7-a726-943a30b73373",
    "CreatedAt": ISODate("1970-01-01"),
    "ModifiedAt": ISODate("1970-01-01")
});
db.clients.insert({
    "_id": UUID("13147148-3c9e-4b52-b214-e6b04d66cb21"),
    "ClientId": "tests",
    "ClientSecret": "$2a$11$BwiSQi56B5UkW4SywF07g.jB3AA0GzHD1YsCWPLMzUlJi9UQUborC",
    "ApplicationName": "Tests",
    "Origin": "",
    "OwnerId": "8caa1d0c-401d-42d7-a726-943a30b73373",
    "CreatedAt": ISODate("1970-01-01"),
    "ModifiedAt": ISODate("1970-01-01")
});