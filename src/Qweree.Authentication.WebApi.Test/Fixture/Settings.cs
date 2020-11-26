namespace Qweree.Authentication.WebApi.Test.Fixture
{
    public static class Settings
    {
        public static class Database
        {
            public const string ConnectionString = "mongodb://localhost:27017";
            public const string DatabaseName = "type_down_auth_test";
        }
        public static class Security
        {
            public const string PasswordKey = "$2a$06$A.ioEWhhL4a.S8gSeeF73efPOD7glmt3BKY339wEC2A7fnQDaPLUy";
        }

        public static class Authentication
        {
            public const string AccessTokenKey = "$2a$06$A.ioEWhhL4a.S8gSeeF73efPOD7glmt3BKY339wEC2A7fnQDaPLUy";
        }
    }
}