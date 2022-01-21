using System;
using System.Collections.Immutable;

namespace Qweree.Authentication.AdminSdk.Authorization.Roles;

public class ClientRoleCreateInput
{
    public ClientRoleCreateInput(Guid id, string key, string label, string description, bool isGroup, ImmutableArray<Guid> items)
    {
        Id = id;
        Key = key;
        Label = label;
        Description = description;
        IsGroup = isGroup;
        Items = items;
    }

    public Guid Id { get; }
    public string Key { get; }
    public string Label { get; }
    public string Description { get; }
    public bool IsGroup { get; }
    public ImmutableArray<Guid> Items { get; }
}