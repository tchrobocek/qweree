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
        "CDN_MANAGE",
        "CDN_EXPLORE",
        "CDN_STORAGE_STORE",
    ],
    "CreatedAt": ISODate("1970-01-01"),
    "ModifiedAt": ISODate("1970-01-01")
});