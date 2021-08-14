using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Commands;
using Qweree.ConsoleApplication.Infrastructure.Commands;

namespace Qweree.ConsoleApplication.Commands.Picc
{
    public class PiccStashCommand : ICommand
    {
        private readonly PiccClientFactory _piccClientFactory;
        private readonly HttpClient _httpClient;

        public PiccStashCommand(PiccClientFactory piccClientFactory, HttpMessageHandler httpMessageHandler)
        {
            _piccClientFactory = piccClientFactory;
            _httpClient = new HttpClient(httpMessageHandler);
        }

        public string CommandPath => "picc";

        public async Task<int> ExecuteAsync(OptionsBag optionsBag, CancellationToken cancellationToken = new())
        {
            string? uri = null;

            if (optionsBag.Options.TryGetValue("--uri", out var uris))
            {
                uri = uris.Single();
            }
            if (optionsBag.Options.TryGetValue("-u", out uris))
            {
                uri = uris.Single();
            }

            if (uri == null)
            {
                Console.WriteLine("None uri provided");
                return -1;
            }

            var piccClient = await _piccClientFactory.CreateClientAsync(cancellationToken);

            var fileResponse = await _httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (!fileResponse.IsSuccessStatusCode)
            {
                Console.WriteLine("Could not download requested file.");
                return -1;
            }

            if (!fileResponse.Content.Headers.TryGetValues("Content-Type", out var mimeTypes))
            {
                Console.WriteLine("Could not determine file type.");
                return -1;
            }

            string mimeType = mimeTypes.Single();

            await using var stream = await fileResponse.Content.ReadAsStreamAsync(cancellationToken);
            var piccResponse = await piccClient.PiccUploadAsync(stream, mimeType, cancellationToken);

            if (!piccResponse.IsSuccessful)
            {
                Console.WriteLine("Could not upload to picc stash service.");
                return -1;
            }

            Console.WriteLine("Uploaded.");
            return 0;
        }
    }
}