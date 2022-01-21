using System.Collections.Generic;
using System.Collections.Immutable;

namespace Qweree.Mongo;

public class Pagination<TDocumentType>
{
    public Pagination(PageInfo pageInfo, long totalCount, IEnumerable<TDocumentType> documents)
    {
        PageInfo = pageInfo;
        TotalCount = totalCount;
        Documents = documents.ToImmutableArray();
    }

    public PageInfo PageInfo { get; }
    public long TotalCount { get; }
    public IEnumerable<TDocumentType> Documents { get; }
}

public class PageInfo
{
    public PageInfo(int skip, int take)
    {
        Skip = skip;
        Take = take;
    }

    public int Skip { get; }
    public int Take { get; }
}