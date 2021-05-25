using System;

namespace Qweree.Authentication.AdminSdk.Authorization.Roles
{
    public class ModifyClientRoleInputDto
    {
        public string? Label { get; set; }
        public string? Description { get; set; }
        public bool? IsGroup { get; set; }
        public Guid[]? Items { get; set; }
    }

    public class ModifyUserRoleInputDto
    {
        public string? Label { get; set; }
        public string? Description { get; set; }
        public bool? IsGroup { get; set; }
        public Guid[]? Items { get; set; }
    }

}