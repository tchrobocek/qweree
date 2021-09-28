namespace Qweree.WebApplication.Infrastructure.View.Text
{
    public static class Plurality
    {
        public static string Decide(long value, string singular, string plural)
        {
            if (value == 1)
                return singular;

            return plural;
        }
    }
}