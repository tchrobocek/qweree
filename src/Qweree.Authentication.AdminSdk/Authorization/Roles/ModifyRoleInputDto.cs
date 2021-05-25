using System;

namespace Qweree.Authentication.AdminSdk.Authorization.Roles
{
    public class ModifyClientRoleInputDto
    {
        public Guid? Id { get; set; }
        public string? Label { get; set; }
        public string? Description { get; set; }
        public bool? IsGroup { get; set; }
        public Guid[]? Items { get; set; }
    }

    public class ModifyUserRoleInputDto
    {
        public Guid? Id { get; set; }
        public string? Label { get; set; }
        public string? Description { get; set; }
        public bool? IsGroup { get; set; }
        public Guid[]? Items { get; set; }
    }

}