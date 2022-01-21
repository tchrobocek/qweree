namespace Qweree.Cdn.WebApi.Domain.Storage;

public class ReadObjectInput
{
    public ReadObjectInput(string path)
    {
        Path = path;
    }

    public string Path { get; }
}