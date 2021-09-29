conn = new Mongo();
db = conn.getDB("qweree_auth");
testDb = conn.getDB("qweree_auth_test");

users = [{
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
}];

clients = [{
    "_id": UUID("9e8fc855-4feb-4c27-8903-f4fed64bf243"),
    "ClientId": "admin-cli",
    "ClientSecret": "$2a$11$BwiSQi56B5UkW4SywF07g.jB3AA0GzHD1YsCWPLMzUlJi9UQUborC",
    "ApplicationName": "Admin CLI",
    "Origin": "localhost",
    "OwnerId": "8caa1d0c-401d-42d7-a726-943a30b73373",
    "CreatedAt": ISODate("1970-01-01"),
    "ModifiedAt": ISODate("1970-01-01"),
    "ClientRoles": [
        "f946f6cd-5d17-4dc5-b383-af0201a8b431",
        "92024488-4c7d-42a9-8fc5-19fc73853c8e",
        "2ec76030-c18d-450c-9a0e-5ff9efb1721d"
    ],
    "UserRoles": [
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
        "d98049ab-977e-42ef-bba6-05c16184d054"
    ]
}, {
    "_id": UUID("13147148-3c9e-4b52-b214-e6b04d66cb21"),
    "ClientId": "test-cli",
    "ClientSecret": "$2a$11$BwiSQi56B5UkW4SywF07g.jB3AA0GzHD1YsCWPLMzUlJi9UQUborC",
    "ApplicationName": "Test CLI",
    "Origin": "",
    "OwnerId": "8caa1d0c-401d-42d7-a726-943a30b73373",
    "CreatedAt": ISODate("1970-01-01"),
    "ModifiedAt": ISODate("1970-01-01"),
    "ClientRoles": [
        "f946f6cd-5d17-4dc5-b383-af0201a8b431",
        "92024488-4c7d-42a9-8fc5-19fc73853c8e",
        "2ec76030-c18d-450c-9a0e-5ff9efb1721d"
    ],
    "UserRoles": []
}];

userRoles = [{
    "_id": UUID("d0a77eeb-972e-4337-a62e-493b3e59f214"),
    "Key": "qweree.auth.users.create",
    "Label": "Create users",
    "Description": "Allows user to create users.",
    "Items": [],
    "IsGroup": false,
    "CreatedAt": ISODate("1970-01-01"),
    "ModifiedAt": ISODate("1970-01-01")
}, {
    "_id": UUID("c990cc7b-7415-4836-8468-b48c67dd9e45"),
    "Key": "qweree.auth.users.read",
    "Label": "Read users",
    "Description": "Allows user to read and filter users.",
    "Items": [],
    "IsGroup": false,
    "CreatedAt": ISODate("1970-01-01"),
    "ModifiedAt": ISODate("1970-01-01")
}, {
    "_id": UUID("66a81b6b-fd91-4338-8ca3-e4aed14dd868"),
    "Key": "qweree.auth.users.delete",
    "Label": "Delete users",
    "Description": "Allows user to delete users.",
    "Items": [],
    "IsGroup": false,
    "CreatedAt": ISODate("1970-01-01"),
    "ModifiedAt": ISODate("1970-01-01")
}, {
    "_id": UUID("20352123-e4c1-4e37-affa-e136d9a66d02"),
    "Key": "qweree.auth.users.read_personal",
    "Label": "Read personal",
    "Description": "Allows user to read users personal data.",
    "Items": [],
    "IsGroup": false,
    "CreatedAt": ISODate("1970-01-01"),
    "ModifiedAt": ISODate("1970-01-01")
}, {
    "_id": UUID("e2ec78fd-c5a0-4b37-b57a-a0a3363e1798"),
    "Key": "qweree.auth.clients.create",
    "Label": "Create clients",
    "Description": "Allows user to create clients.",
    "Items": [],
    "IsGroup": false,
    "CreatedAt": ISODate("1970-01-01"),
    "ModifiedAt": ISODate("1970-01-01")
}, {
    "_id": UUID("729fde13-52a2-4a82-befa-a4e6666924a6"),
    "Key": "qweree.auth.clients.read",
    "Label": "Read clients",
    "Description": "Allows user to read and filter clients.",
    "Items": [],
    "IsGroup": false,
    "CreatedAt": ISODate("1970-01-01"),
    "ModifiedAt": ISODate("1970-01-01")
}, {
    "_id": UUID("2b0e761c-cce2-4609-9ab6-94980fbc639e"),
    "Key": "qweree.auth.clients.delete",
    "Label": "Delete clients",
    "Description": "Allows user to delete clients.",
    "Items": [],
    "IsGroup": false,
    "CreatedAt": ISODate("1970-01-01"),
    "ModifiedAt": ISODate("1970-01-01")
}, {
    "_id": UUID("f7f99a7b-6890-4bf2-b39b-14444585a712"),
    "Key": "qweree.auth.roles.create",
    "Label": "Create roles",
    "Description": "Allows user to create roles.",
    "Items": [],
    "IsGroup": false,
    "CreatedAt": ISODate("1970-01-01"),
    "ModifiedAt": ISODate("1970-01-01")
}, {
    "_id": UUID("905b39a6-a4bd-480a-b83d-397d1add5569"),
    "Key": "qweree.auth.roles.read",
    "Label": "Read roles",
    "Description": "Allows user to read and filter roles.",
    "Items": [],
    "IsGroup": false,
    "CreatedAt": ISODate("1970-01-01"),
    "ModifiedAt": ISODate("1970-01-01")
}, {
    "_id": UUID("a1272efa-b687-4fda-a999-11b4b0acd414"),
    "Key": "qweree.auth.roles.modify",
    "Label": "Modify roles",
    "Description": "Allows user to modify roles.",
    "Items": [],
    "IsGroup": false,
    "CreatedAt": ISODate("1970-01-01"),
    "ModifiedAt": ISODate("1970-01-01")
}, {
    "_id": UUID("98d7d3a1-bedd-4ee1-a633-a4217f5414ee"),
    "Key": "qweree.auth.roles.delete",
    "Label": "Delete roles",
    "Description": "Allows user to delete roles.",
    "Items": [],
    "IsGroup": false,
    "CreatedAt": ISODate("1970-01-01"),
    "ModifiedAt": ISODate("1970-01-01")
}, {
    "_id": UUID("145e1674-691a-4bd5-956b-4154bc4264da"),
    "Key": "qweree.cdn.storage.explore",
    "Label": "Cdn storage explore",
    "Description": "Allows user to explore cdn.",
    "Items": [],
    "IsGroup": false,
    "CreatedAt": ISODate("1970-01-01"),
    "ModifiedAt": ISODate("1970-01-01")
}, {
    "_id": UUID("d98049ab-977e-42ef-bba6-05c16184d054"),
    "Key": "qweree.cdn.storage.store",
    "Label": "Cdn storage store",
    "Description": "Allows user to store files into cdn.",
    "Items": [],
    "IsGroup": false,
    "CreatedAt": ISODate("1970-01-01"),
    "ModifiedAt": ISODate("1970-01-01")
}];

clientRoles = [{
    "_id": UUID("f946f6cd-5d17-4dc5-b383-af0201a8b431"),
    "Key": "qweree.auth.grant_type.password",
    "Label": "Password grant",
    "Description": "Allows client to authenticate with password grant type.",
    "Items": [],
    "IsGroup": false,
    "CreatedAt": ISODate("1970-01-01"),
    "ModifiedAt": ISODate("1970-01-01")
},{
    "_id": UUID("92024488-4c7d-42a9-8fc5-19fc73853c8e"),
    "Key": "qweree.auth.grant_type.refresh_token",
    "Label": "Refresh token grant",
    "Description": "Allows client to authenticate with refresh token grant type.",
    "Items": [],
    "IsGroup": false,
    "CreatedAt": ISODate("1970-01-01"),
    "ModifiedAt": ISODate("1970-01-01")
},{
    "_id": UUID("2ec76030-c18d-450c-9a0e-5ff9efb1721d"),
    "Key": "qweree.auth.grant_type.client_credentials",
    "Label": "Client credentials grant",
    "Description": "Allows client to authenticate with client credentials grant type.",
    "Items": [],
    "IsGroup": false,
    "CreatedAt": ISODate("1970-01-01"),
    "ModifiedAt": ISODate("1970-01-01")
}];

db.users.insert(users);
db.clients.insert(clients);
db.user_roles.insert(userRoles);
db.client_roles.insert(clientRoles);

testDb.user_roles.insert(userRoles);
testDb.client_roles.insert(clientRoles);