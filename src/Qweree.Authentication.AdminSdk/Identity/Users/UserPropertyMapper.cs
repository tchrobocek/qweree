namespace Qweree.Authentication.AdminSdk.Identity.Users;

public static class UserPropertyMapper
{
    public static UserPropertyDto ToDto(UserProperty property)
    {
        return new UserPropertyDto
        {
            Key = property.Key,
            Value = property.Value
        };
    }

    public static UserProperty FromDto(UserPropertyDto property)
    {
        return new UserProperty(property.Key ?? string.Empty, property.Value ?? string.Empty);
    }
}