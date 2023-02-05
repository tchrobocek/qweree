namespace Qweree.Authentication.WebApi.Domain.Identity;

public class ClientModifyInput
{
    public ClientModifyInput(string? applicationName, string? origin)
    {
        ApplicationName = applicationName;
        Origin = origin;
    }

    public string? ApplicationName { get; }
    public string? Origin { get; }
}