using System;

namespace Qweree.Authentication.AdminSdk.Authorization.Roles;

public class Role
{
    public Role(Guid id, string key, string label, string description)
    {
        Id = id;
        Key = key;
        Label = label;
        Description = description;
    }

    public Guid Id { get; }
    public string Key { get; }
    public string Label { get; }
    public string Description { get; }
}