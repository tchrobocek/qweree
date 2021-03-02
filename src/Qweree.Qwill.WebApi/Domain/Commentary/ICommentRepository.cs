using System;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Mongo;
using Qweree.Qwill.WebApi.Domain.Persistence;

namespace Qweree.Qwill.WebApi.Domain.Commentary
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<Pagination<Comment>> PaginateBySubjectAsync(Guid subjectId, int skip, int take,
            CancellationToken cancellationToken = new());
    }
}