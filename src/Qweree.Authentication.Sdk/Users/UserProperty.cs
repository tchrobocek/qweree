namespace Qweree.Authentication.Sdk.Users;

public class UserProperty
{
    public UserProperty(string key, string value)
    {
        Key = key;
        Value = value;
    }

    public string Key { get; }
    public string Value { get; }
}