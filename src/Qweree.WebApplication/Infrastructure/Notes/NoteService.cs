using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Cdn.Sdk;
using Qweree.Cdn.Sdk.Storage;
using Qweree.Utils;
using Qweree.WebApplication.Infrastructure.Authentication;

namespace Qweree.WebApplication.Infrastructure.Notes
{
    public class NoteService
    {
        private readonly StorageClient _storageClient;
        private readonly ClaimsPrincipalStorage _claimsPrincipalStorage;

        public NoteService(StorageClient storageClient, ClaimsPrincipalStorage claimsPrincipalStorage)
        {
            _storageClient = storageClient;
            _claimsPrincipalStorage = claimsPrincipalStorage;
        }

        public async Task<NoteCollectionDto> GetMyNotesAsync(string entity, CancellationToken cancellationToken = new())
        {
            var path = await GetPathAsync(entity, cancellationToken);
            using var response = await _storageClient.RetrieveAsync(path, cancellationToken);

            if (!response.IsSuccessful)
                return new NoteCollectionDto();

            var stream = await response.ReadPayloadAsStreamAsync(cancellationToken);

            try
            {
                return (await JsonUtils.DeserializeAsync<NoteCollectionDto>(stream, cancellationToken)) ?? new NoteCollectionDto();
            }
            catch (Exception)
            {
                return new NoteCollectionDto();
            }
        }


        public async Task<bool> SetMyNotesAsync(string entity, IEnumerable<NoteDto> notes, CancellationToken cancellationToken = new())
        {
            if (string.IsNullOrWhiteSpace(entity))
                throw new ArgumentException("Invalid entity.");

            var path = await GetPathAsync(entity, cancellationToken);
            await using var stream = new MemoryStream();
            await JsonUtils.SerializeAsync(stream, new NoteCollectionDto{Notes = notes.ToArray()}, cancellationToken);
            stream.Seek(0, SeekOrigin.Begin);
            var response = await _storageClient.StoreAsync(path, MediaTypeNames.Application.Json, stream, true, true, cancellationToken);

            return response.IsSuccessful;
        }

        private async Task<string> GetPathAsync(string entity, CancellationToken cancellationToken = new())
        {
            var identity = await _claimsPrincipalStorage.GetIdentityAsync(cancellationToken);
            return PathHelper.Combine(PathHelper.GetUserRootPath((Guid)identity?.User?.Id!), "notes", $"{entity}.json");
        }
    }
}