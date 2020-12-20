using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Qweree.Mongo.Exception;

namespace Qweree.Mongo
{
    public abstract class MongoRepositoryBase<TDocumentType> : MongoRepositoryBase<TDocumentType, TDocumentType>
    {
        protected MongoRepositoryBase(string collectionName, MongoContext context) : base(collectionName, context)
        {
        }

        protected sealed override Func<TDocumentType, TDocumentType> ToDocument => publicObject => publicObject;
        protected sealed override Func<TDocumentType, TDocumentType> FromDocument => publicObject => publicObject;
    }

    public abstract class MongoRepositoryBase<TPublicType, TDocumentType>
    {
        protected MongoRepositoryBase(string collectionName, MongoContext context)
        {
            Collection = context.GetCollection<TDocumentType>(collectionName);
        }

        protected IMongoCollection<TDocumentType> Collection { get; }
        protected abstract Func<TPublicType, TDocumentType> ToDocument { get; }
        protected abstract Func<TDocumentType, TPublicType> FromDocument { get; }

        public async Task<IEnumerable<TPublicType>> FindAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return await DoFindAsync(null, null, null, null, cancellationToken);
        }

        public async Task<IEnumerable<TPublicType>> FindAsync(string query, CancellationToken cancellationToken = new CancellationToken())
        {
            return await DoFindAsync(query, null, null, null, cancellationToken);
        }

        public async Task<IEnumerable<TPublicType>> FindAsync(string query, int skip, int take, CancellationToken cancellationToken = new CancellationToken())
        {
            return await DoFindAsync(query, skip, take, null, cancellationToken);
        }

        public async Task<IEnumerable<TPublicType>> FindAsync(string query, int skip, int take, Dictionary<string, int> sort, CancellationToken cancellationToken = new CancellationToken())
        {
            return await DoFindAsync(query, skip, take, sort, cancellationToken);
        }

        public async Task<Pagination<TPublicType>> PaginateAsync(string query, int skip, int take,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var documents = await FindAsync(query, skip, take, cancellationToken);
            var count = await CountAsync(query, cancellationToken);

            return new Pagination<TPublicType>(new PageInfo(skip, take), count, documents);
        }

        public async Task<Pagination<TPublicType>> PaginateAsync(string query, int skip, int take,
            Dictionary<string, int> sort, CancellationToken cancellationToken = new CancellationToken())
        {
            var documents = await FindAsync(query, skip, take, sort, cancellationToken);
            var count = await CountAsync(query, cancellationToken);

            return new Pagination<TPublicType>(new PageInfo(skip, take), count, documents);
        }

        private async Task<IEnumerable<TPublicType>> DoFindAsync(string? query, int? skip, int? take,
            Dictionary<string, int>? sort, CancellationToken cancellationToken = new CancellationToken())
        {
            var findOptions = new FindOptions<TDocumentType>();

            if (skip != null)
                findOptions.Skip = skip;
            if (take != null)
                findOptions.Limit = take;

            if (sort != null)
            {
                var bsonDocument = new BsonDocument(sort);
                findOptions.Sort = new BsonDocumentSortDefinition<TDocumentType>(bsonDocument);
            }

            try
            {
                var list = await (await Collection.FindAsync(query ?? "{}", findOptions, cancellationToken))
                    .ToListAsync(cancellationToken);
                return list.Select(e => FromDocument(e));
            }
            catch (System.Exception e) when(e is MongoCommandException || e is FormatException)
            {
                throw new InvalidFilterException("Invalid filter.", e);
            }
        }

        public async Task<long> CountAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return await CountAsync("{}", cancellationToken);
        }

        public async Task<long> CountAsync(string query, CancellationToken cancellationToken = new CancellationToken())
        {

            try
            {
                return await Collection.CountDocumentsAsync(query, cancellationToken: cancellationToken);
            }
            catch (System.Exception e) when(e is MongoCommandException || e is FormatException)
            {
                throw new InvalidFilterException("Invalid filter.", e);
            }
        }

        public async Task<TPublicType> GetAsync(Guid id, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await FindAsync($@"{{""_id"": UUID(""{id}"")}}", cancellationToken);
            return result.FirstOrDefault() ?? throw new DocumentNotFoundException($@"Document ""{id}"" was not found.");
        }

        public async Task InsertAsync(TPublicType document, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                await Collection.InsertOneAsync(ToDocument(document), cancellationToken: cancellationToken);
            }
            catch (MongoWriteException e)
            {
                throw new InsertDocumentException("Cannot insert document.", e);
            }
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = new CancellationToken())
        {
            await Collection.DeleteOneAsync($@"{{""_id"": UUID(""{id}"")}}", cancellationToken);
        }

        public async Task DeleteAllAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            await Collection.DeleteManyAsync(d => true, cancellationToken);
        }
    }
}