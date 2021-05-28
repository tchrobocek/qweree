using System;
using System.Collections.Immutable;

namespace Qweree.Authentication.WebApi.Domain.Authorization.Roles
{
    public class UserRole
    {
        public UserRole(Guid id, string key, string label, string description, ImmutableArray<Guid> items, bool isGroup, DateTime createdAt, DateTime modifiedAt)
        {
            Id = id;
            Key = key;
            Label = label;
            Description = description;
            Items = items;
            IsGroup = isGroup;
            CreatedAt = createdAt;
            ModifiedAt = modifiedAt;
        }

        public Guid Id { get; }
        public string Key { get; }
        public string Label { get; }
        public string Description { get; }
        public ImmutableArray<Guid> Items { get; }
        public bool IsGroup { get; }
        public DateTime CreatedAt { get; }
        public DateTime ModifiedAt { get; }
    }

}