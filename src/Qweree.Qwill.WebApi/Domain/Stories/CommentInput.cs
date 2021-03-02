namespace Qweree.Qwill.WebApi.Domain.Stories
{
    public class CommentInput
    {
        public CommentInput(string text)
        {
            Text = text;
        }

        public string Text { get; }
    }
}