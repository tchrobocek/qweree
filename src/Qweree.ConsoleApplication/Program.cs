using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Qweree.Authentication.Sdk.Authentication;
using Qweree.Cdn.Sdk.Storage;

namespace Qweree.ConsoleApplication
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            using var httpClient = new HttpClient();
            var authUri = new Uri("http://localhost:8080/api/oauth2/auth");
            var cdnUri = new Uri("http://localhost:8090/api/v1/storage");
            var authenticationAdapter = new OAuth2Adapter(authUri, httpClient);
            var tokenInfo = await authenticationAdapter.SignInAsync(new PasswordGrantInput("admin", "password"));

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", tokenInfo.AccessToken);
            var storageAdapter = new StorageAdapter(cdnUri, httpClient);

            var random = new Random();

            var imageUri = args[0];
            await using var image = File.OpenRead(imageUri);

            var folders = new[] {"a", "b", "c", "d"};

            Console.WriteLine($@"Storing to ""{cdnUri}"".");
            for (var i = 0; i < 1000; i++)
            {
                var path = Path.Combine(folders[random.Next(folders.Length)],
                    folders[random.Next(folders.Length)], Guid.NewGuid().ToString());
                image.Seek(0, SeekOrigin.Begin);
                await storageAdapter.StoreAsync(path, args[1], image);
                Console.WriteLine($@"Created object ""{path}""");
            }

            return 0;
        }
    }
}