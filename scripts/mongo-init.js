conn = new Mongo();
db = conn.getDB("type_down");
db.users.insert({
    "_id": UUID("8caa1d0c-401d-42d7-a726-943a30b73373"),
    "Username": "admin",
    "Password": "$2a$06$A.ioEWhhL4a.S8gSeeF73enq.6k5nw227OkdZNClrNlONq0wKYXo2",
    "ContactEmail": "admin@admin.com",
    "FullName": "Admin Adminowitch",
    "Roles": ["UserUpdate"],
    "CreatedAt": ISODate("1970-01-01"),
    "ModifiedAt": ISODate("1970-01-01")
});