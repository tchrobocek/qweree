namespace Qweree.AspNet.Web
{
    public class ErrorResponseDto
    {
        public ErrorDto[]? Errors { get; set; }
    }

    public class ErrorDto
    {
        public string? Message { get; set; }
    }
}