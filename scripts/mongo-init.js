conn = new Mongo();
db = conn.getDB("qweree_auth");
db.users.insert({
    "_id": UUID("8caa1d0c-401d-42d7-a726-943a30b73373"),
    "Username": "admin",
    "Password": "$2a$11$BwiSQi56B5UkW4SywF07g.jB3AA0GzHD1YsCWPLMzUlJi9UQUborC",
    "ContactEmail": "admin@admin.com",
    "FullName": "Admin Adminowitch",
    "Roles": [
        "d0a77eeb-972e-4337-a62e-493b3e59f214",
        "c990cc7b-7415-4836-8468-b48c67dd9e45",
        "66a81b6b-fd91-4338-8ca3-e4aed14dd868",
        "20352123-e4c1-4e37-affa-e136d9a66d02",
        "e2ec78fd-c5a0-4b37-b57a-a0a3363e1798",
        "729fde13-52a2-4a82-befa-a4e6666924a6",
        "2b0e761c-cce2-4609-9ab6-94980fbc639e",
        "f7f99a7b-6890-4bf2-b39b-14444585a712",
        "905b39a6-a4bd-480a-b83d-397d1add5569",
        "a1272efa-b687-4fda-a999-11b4b0acd414",
        "98d7d3a1-bedd-4ee1-a633-a4217f5414ee",
        "145e1674-691a-4bd5-956b-4154bc4264da",
        "d98049ab-977e-42ef-bba6-05c16184d054",
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
db.user_roles.insert([{
    "_id": UUID("d0a77eeb-972e-4337-a62e-493b3e59f214"),
    "Key": "AUTH_USERS_CREATE",
    "Label": "",
    "Description": "",
    "Items": [],
    "IsGroup": false,
    "CreatedAt": ISODate("1970-01-01"),
    "ModifiedAt": ISODate("1970-01-01")
}, {
    "_id": UUID("c990cc7b-7415-4836-8468-b48c67dd9e45"),
    "Key": "AUTH_USERS_READ",
    "Label": "",
    "Description": "",
    "Items": [],
    "IsGroup": false,
    "CreatedAt": ISODate("1970-01-01"),
    "ModifiedAt": ISODate("1970-01-01")
}, {
    "_id": UUID("66a81b6b-fd91-4338-8ca3-e4aed14dd868"),
    "Key": "AUTH_USERS_DELETE",
    "Label": "",
    "Description": "",
    "Items": [],
    "IsGroup": false,
    "CreatedAt": ISODate("1970-01-01"),
    "ModifiedAt": ISODate("1970-01-01")
}, {
    "_id": UUID("20352123-e4c1-4e37-affa-e136d9a66d02"),
    "Key": "AUTH_USERS_READ_PERSONAL_DETAIL",
    "Label": "",
    "Description": "",
    "Items": [],
    "IsGroup": false,
    "CreatedAt": ISODate("1970-01-01"),
    "ModifiedAt": ISODate("1970-01-01")
}, {
    "_id": UUID("e2ec78fd-c5a0-4b37-b57a-a0a3363e1798"),
    "Key": "AUTH_CLIENTS_CREATE",
    "Label": "",
    "Description": "",
    "Items": [],
    "IsGroup": false,
    "CreatedAt": ISODate("1970-01-01"),
    "ModifiedAt": ISODate("1970-01-01")
}, {
    "_id": UUID("729fde13-52a2-4a82-befa-a4e6666924a6"),
    "Key": "AUTH_CLIENTS_READ",
    "Label": "",
    "Description": "",
    "Items": [],
    "IsGroup": false,
    "CreatedAt": ISODate("1970-01-01"),
    "ModifiedAt": ISODate("1970-01-01")
}, {
    "_id": UUID("2b0e761c-cce2-4609-9ab6-94980fbc639e"),
    "Key": "AUTH_CLIENTS_DELETE",
    "Label": "",
    "Description": "",
    "Items": [],
    "IsGroup": false,
    "CreatedAt": ISODate("1970-01-01"),
    "ModifiedAt": ISODate("1970-01-01")
}, {
    "_id": UUID("f7f99a7b-6890-4bf2-b39b-14444585a712"),
    "Key": "AUTH_ROLES_CREATE",
    "Label": "",
    "Description": "",
    "Items": [],
    "IsGroup": false,
    "CreatedAt": ISODate("1970-01-01"),
    "ModifiedAt": ISODate("1970-01-01")
}, {
    "_id": UUID("905b39a6-a4bd-480a-b83d-397d1add5569"),
    "Key": "AUTH_ROLES_READ",
    "Label": "",
    "Description": "",
    "Items": [],
    "IsGroup": false,
    "CreatedAt": ISODate("1970-01-01"),
    "ModifiedAt": ISODate("1970-01-01")
}, {
    "_id": UUID("a1272efa-b687-4fda-a999-11b4b0acd414"),
    "Key": "AUTH_ROLES_MODIFY",
    "Label": "",
    "Description": "",
    "Items": [],
    "IsGroup": false,
    "CreatedAt": ISODate("1970-01-01"),
    "ModifiedAt": ISODate("1970-01-01")
}, {
    "_id": UUID("98d7d3a1-bedd-4ee1-a633-a4217f5414ee"),
    "Key": "AUTH_ROLES_DELETE",
    "Label": "",
    "Description": "",
    "Items": [],
    "IsGroup": false,
    "CreatedAt": ISODate("1970-01-01"),
    "ModifiedAt": ISODate("1970-01-01")
}, {
    "_id": UUID("145e1674-691a-4bd5-956b-4154bc4264da"),
    "Key": "CDN_EXPLORE",
    "Label": "",
    "Description": "",
    "Items": [],
    "IsGroup": false,
    "CreatedAt": ISODate("1970-01-01"),
    "ModifiedAt": ISODate("1970-01-01")
}, {
    "_id": UUID("d98049ab-977e-42ef-bba6-05c16184d054"),
    "Key": "CDN_STORAGE_STORE",
    "Label": "",
    "Description": "",
    "Items": [],
    "IsGroup": false,
    "CreatedAt": ISODate("1970-01-01"),
    "ModifiedAt": ISODate("1970-01-01")
}]);