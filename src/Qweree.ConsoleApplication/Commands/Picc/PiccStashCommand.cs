using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Commands;
using Qweree.ConsoleApplication.Infrastructure.Commands;
using Qweree.PiccStash.Sdk;

namespace Qweree.ConsoleApplication.Commands.Picc;

public class PiccStashCommand : ICommand
{
    private readonly PiccClientFactory _piccClientFactory;
    private readonly HttpClient _httpClient;
    private readonly Infrastructure.RunContext.Context _context;

    public PiccStashCommand(PiccClientFactory piccClientFactory, HttpMessageHandler httpMessageHandler, Infrastructure.RunContext.Context context)
    {
        _piccClientFactory = piccClientFactory;
        _context = context;
        _httpClient = new HttpClient(httpMessageHandler);
    }

    public string CommandPath => "picc";

    public async Task<int> ExecuteAsync(OptionsBag optionsBag, CancellationToken cancellationToken = new())
    {
        if (optionsBag.Options.ContainsKey("--uri") || optionsBag.Options.ContainsKey("-u"))
        {
            return await DownloadFromUriAsync(optionsBag, cancellationToken);
        }

        if (optionsBag.Options.ContainsKey("--directory") || optionsBag.Options.ContainsKey("-d"))
        {
            return await UploadDirectoryAsync(optionsBag, cancellationToken);
        }

        Console.WriteLine("Did not provide any action.");
        return -1;
    }

    private async Task<int> UploadDirectoryAsync(OptionsBag optionsBag, CancellationToken cancellationToken)
    {
        string? path = null;

        if (optionsBag.Options.TryGetValue("--directory", out var paths))
        {
            path = paths.Single();
        }
        if (optionsBag.Options.TryGetValue("-d", out paths))
        {
            path = paths.Single();
        }

        if (path == null)
        {
            Console.WriteLine("None path provided");
            return -1;
        }

        if (!Directory.Exists(path))
        {
            Console.WriteLine(@$"Path ""{path}"" does not exist.");
            return -1;
        }

        var isRecursive = false;
        if (optionsBag.Options.TryGetValue("--recursive", out var recursiveValues))
        {
            isRecursive = recursiveValues.Any();
        }
        if (optionsBag.Options.TryGetValue("-r", out recursiveValues))
        {
            isRecursive = recursiveValues.Any();
        }

        var option = SearchOption.TopDirectoryOnly;
        if (isRecursive)
        {
            option = SearchOption.AllDirectories;
        }

        var piccClient = await _piccClientFactory.CreateClientAsync(cancellationToken);
        var config = await _context.GetConfigurationAsync(cancellationToken);

        var files = Directory.GetFiles(path, "*", option);
        foreach (var filePath in files)
        {
            await using var stream = File.OpenRead(filePath);
            var extension = Path.GetExtension(stream.Name);
            var mimeType = GuessMimeType(extension);
            Console.WriteLine(extension);

            try
            {
                var picc = await UploadAsync(piccClient, stream, mimeType, cancellationToken);
                Console.WriteLine(new Uri(new Uri(new Uri(config.PiccUri ?? string.Empty), "api/v1/picc/"), picc?.Id.ToString() ?? string.Empty));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return -1;
            }
        }

        return 0;
    }

    private async Task<int> DownloadFromUriAsync(OptionsBag optionsBag, CancellationToken cancellationToken = new())
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
        var config = await _context.GetConfigurationAsync(cancellationToken);


        try
        {
            var picc = await UploadAsync(piccClient, stream, mimeType, cancellationToken);
            Console.WriteLine(new Uri(new Uri(new Uri(config.PiccUri ?? string.Empty), "api/v1/picc/"), picc?.Id.ToString() ?? string.Empty));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return -1;
        }

        return 0;
    }

    private async Task<PiccDto?> UploadAsync(PiccClient client, Stream stream, string mimeType, CancellationToken cancellationToken = new())
    {
        var piccResponse = await client.PiccUploadAsync(stream, mimeType, cancellationToken);

        if (!piccResponse.IsSuccessful)
        {
            var errors = await piccResponse.ReadErrorsAsync(cancellationToken);
            throw new ArgumentException(piccResponse.StatusCode + " - " + string.Join(" ", errors.Errors?.Select(e => e.Message) ?? Array.Empty<string?>()));
        }

        var picc = await piccResponse.ReadPayloadAsync(cancellationToken);
        return picc;
    }

    private string GuessMimeType(string extension)
    {
        var dictionary = new Dictionary<string, string[]>
        {
            {"image/png", new[] {".png"}},
            {"image/jpeg", new[] {".jpg", ".jpeg"}}
        };

        foreach (var pair in dictionary)
        {
            if (pair.Value.Contains(extension))
            {
                return pair.Key;
            }
        }

        return "application/octet-stream";
    }
}