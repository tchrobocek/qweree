namespace Qweree.Sdk;

public class ErrorResponse
{
    public ErrorDto[]? Errors { get; set; }
}

public class ErrorDto
{
    public string? Message { get; set; }
}