using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Qweree.Cdn.Sdk.Storage;
using Qweree.Mongo;
using Qweree.Mongo.Exception;

namespace Qweree.Cdn.WebApi.Infrastructure.Storage;

public class StoredObjectDescriptorRepository :
    MongoRepositoryBase<StoredObjectDescriptor, StoredObjectDescriptorDo>, IStoredObjectDescriptorRepository
{
    public StoredObjectDescriptorRepository(MongoContext context) : base("stored_objects", context)
    {
    }

    protected override Func<StoredObjectDescriptor, StoredObjectDescriptorDo> ToDocument =>
        descriptor => new StoredObjectDescriptorDo
        {
            Id = descriptor.Id,
            Size = descriptor.Size,
            CreatedAt = descriptor.CreatedAt,
            Slug = descriptor.Slug.ToArray(),
            MediaType = descriptor.MediaType,
            ModifiedAt = descriptor.ModifiedAt
        };

    protected override Func<StoredObjectDescriptorDo, StoredObjectDescriptor> FromDocument =>
        descriptor => new StoredObjectDescriptor(descriptor.Id, descriptor.Slug ?? ArraySegment<string>.Empty,
            descriptor.MediaType ?? "", descriptor.Size, descriptor.CreatedAt, descriptor.ModifiedAt);

    public async Task<StoredObjectDescriptor> GetBySlugAsync(string[] slug,
        CancellationToken cancellationToken = new())
    {
        var slugInput = string.Join(@""", """, slug);

        if (slug.Any())
            slugInput = "\"" + slugInput + "\"";

        var query = @$"{{""Slug"": {{""$eq"": [{slugInput}]}}}}";
        var descriptor = (await FindAsync(query, cancellationToken)).FirstOrDefault();

        if (descriptor == null)
            throw new DocumentNotFoundException(@$"Descriptor ""{string.Join("/", slug)}"" was not found.");

        return descriptor;
    }

    public async Task<IEnumerable<StoredPathDescriptorDo>> FindInSlugAsync(string[] slug,
        CancellationToken cancellationToken = new())
    {
        var slugMatchInput = @"";
        var projectSlugInput = @"";

        for (var i = 0; i < slug.Length; i++)
        {
            var item = slug[i];
            slugMatchInput += @$"""Slug.{i}"": ""{item}"",";
            projectSlugInput += $@"{{""$arrayElemAt"": [""$Slug"", {i}]}},";
        }

        slugMatchInput += @$"""Slug.{slug.Length}"": {{""$exists"": true}},";
        projectSlugInput += $@"{{""$arrayElemAt"": [""$Slug"", {slug.Length}]}},";

        var aggregation = $@"[{{
    ""$match"": {{
        {slugMatchInput}
    }}
}},{{
    ""$project"": {{
        ""SearchedSlug"": [{projectSlugInput}],
        ""MediaType"": 1,
        ""Size"": 1,
        ""CreatedAt"": 1,
        ""ModifiedAt"": 1,
        ""Slug"": 1
    }}
}},{{
    ""$group"": {{
        ""_id"": ""$SearchedSlug"",
        ""TotalCount"": {{
            ""$sum"": 1
        }},
        ""TotalSize"": {{
            ""$sum"": ""$Size""
        }},
        ""MinCreatedAt"": {{
            ""$min"": ""$ModifiedAt""
        }},
        ""MaxModifiedAt"": {{
            ""$max"": ""$ModifiedAt""
        }},
        ""FirstId"": {{
            ""$first"": ""$_id""
        }},
        ""FirstSlug"": {{
            ""$first"": ""$Slug""
        }},
        ""FirstMediaType"": {{
            ""$first"": ""$MediaType""
        }},
        ""FirstSize"": {{
            ""$first"": ""$Size""
        }},
        ""FirstCreatedAt"": {{
            ""$first"": ""$CreatedAt""
        }},
        ""FirstModifiedAt"": {{
            ""$first"": ""$ModifiedAt""
        }}
    }}
}}]";
        var bsonPipeline = BsonSerializer.Deserialize<BsonArray>(aggregation);
        var result = await Collection.AggregateAsync<StoredPathDescriptorDo>(
            bsonPipeline.Select(d => d.AsBsonDocument).ToArray(), cancellationToken: cancellationToken);

        return await result.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<StoredObjectStatsDo>> GetObjectsStatsAsync(CancellationToken cancellationToken = new())
    {
        var aggregation = $@"[{{
    ""$group"": {{
        _id: ""$MediaType"",
        Count: {{
            $sum: 1
        }},
        Used: {{
            $sum: ""$Size""
        }}
    }}
}}]";

        var bsonPipeline = BsonSerializer.Deserialize<BsonArray>(aggregation);
        var result = await Collection.AggregateAsync<StoredObjectStatsDo>(
            bsonPipeline.Select(d => d.AsBsonDocument).ToArray(), cancellationToken: cancellationToken);

        return await result.ToListAsync(cancellationToken);
    }

    public async Task DeleteBySlugAsync(string[] slug, CancellationToken cancellationToken = new())
    {
        var slugInput = string.Join(@""", """, slug);

        if (slug.Any())
            slugInput = "\"" + slugInput + "\"";

        var query = @$"{{""Slug"": {{""$eq"": [{slugInput}]}}}}";
        await DeleteAsync(query, cancellationToken);
    }
}