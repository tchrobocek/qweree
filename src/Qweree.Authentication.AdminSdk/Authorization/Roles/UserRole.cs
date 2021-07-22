using System;
using System.Collections.Immutable;

namespace Qweree.Authentication.AdminSdk.Authorization.Roles
{
    public class UserRole
    {
        public UserRole(Guid id, string key, string label, string description, ImmutableArray<UserRole> items, bool isGroup, DateTime createdAt, DateTime modifiedAt, ImmutableArray<Role> effectiveRoles)
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
        public ImmutableArray<UserRole> Items { get; }
        public bool IsGroup { get; }
        public DateTime CreatedAt { get; }
        public DateTime ModifiedAt { get; }
        public ImmutableArray<Role> EffectiveRoles { get; }
    }
}