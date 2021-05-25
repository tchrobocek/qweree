using System;
using System.Collections.Immutable;

namespace Qweree.Authentication.AdminSdk.Authorization.Roles
{
    public class ModifyClientRoleInput
    {
        public ModifyClientRoleInput(string? label, string? description, bool? isGroup, ImmutableArray<Guid>? items)
        {
            Label = label;
            Description = description;
            IsGroup = isGroup;
            Items = items;
        }

        public string? Label { get; }
        public string? Description { get; }
        public bool? IsGroup { get; }
        public ImmutableArray<Guid>? Items { get; }
    }

    public class ModifyUserRoleInput
    {
        public ModifyUserRoleInput(string? label, string? description, bool? isGroup, ImmutableArray<Guid>? items)
        {
            Label = label;
            Description = description;
            IsGroup = isGroup;
            Items = items;
        }

        public string? Label { get; }
        public string? Description { get; }
        public bool? IsGroup { get; }
        public ImmutableArray<Guid>? Items { get; }
    }

}