using System;

namespace Qweree.Authentication.AdminSdk.Authorization.Roles
{
    public class UserRoleDto
    {
        public Guid? Id { get; set; }
        public string? Key { get; set; }
        public string? Label { get; set; }
        public string? Description { get; set; }
        public UserRoleDto[]? Items { get; set; }
        public bool? IsGroup { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string[]? EffectiveRoles { get; set; }
    }

    public class ClientRoleDto
    {
        public Guid? Id { get; set; }
        public string? Key { get; set; }
        public string? Label { get; set; }
        public string? Description { get; set; }
        public ClientRoleDto[]? Items { get; set; }
        public bool? IsGroup { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string[]? EffectiveRoles { get; set; }
    }
}