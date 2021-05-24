using System;
using System.Collections.Immutable;

namespace Qweree.Authentication.AdminSdk.Authorization
{
    public class ModifyClientRoleInput
    {
        public ModifyClientRoleInput(Guid id, string? label, string? description, bool? isGroup, ImmutableArray<Guid>? items)
        {
            Id = id;
            Label = label;
            Description = description;
            IsGroup = isGroup;
            Items = items;
        }

        public Guid Id { get; }
        public string? Label { get; }
        public string? Description { get; }
        public bool? IsGroup { get; }
        public ImmutableArray<Guid>? Items { get; }
    }

    public class ModifyUserRoleInput
    {
        public ModifyUserRoleInput(Guid id, string? label, string? description, bool? isGroup, ImmutableArray<Guid>? items)
        {
            Id = id;
            Label = label;
            Description = description;
            IsGroup = isGroup;
            Items = items;
        }

        public Guid Id { get; }
        public string? Label { get; }
        public string? Description { get; }
        public bool? IsGroup { get; }
        public ImmutableArray<Guid>? Items { get; }
    }

}