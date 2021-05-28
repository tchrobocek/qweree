using System;
using System.Collections.Immutable;

namespace Qweree.Authentication.AdminSdk.Authorization.Roles
{
    public class ClientRole
    {
        public ClientRole(Guid id, string key, string label, string description, ImmutableArray<ClientRole> items, bool isGroup, DateTime createdAt, DateTime modifiedAt, ImmutableArray<string> effectiveRoles)
        {
            Id = id;
            Key = key;
            Label = label;
            Description = description;
            Items = items;
            IsGroup = isGroup;
            CreatedAt = createdAt;
            ModifiedAt = modifiedAt;
            EffectiveRoles = effectiveRoles;
        }

        public Guid Id { get; }
        public string Key { get; }
        public string Label { get; }
        public string Description { get; }
        public ImmutableArray<ClientRole> Items { get; }
        public bool IsGroup { get; }
        public DateTime CreatedAt { get; }
        public DateTime ModifiedAt { get; }
        public ImmutableArray<string> EffectiveRoles { get; }
    }
}