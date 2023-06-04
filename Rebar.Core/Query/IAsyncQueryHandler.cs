using System.Threading;
using System.Threading.Tasks;

namespace Rebar.Core.Query
{
    public interface IAsyncQueryHandler<TQuery, TResult> : IQueryHandlerBase<TQuery, TResult> where TQuery : IQuery<TResult> where TResult : IQueryResponse
    {
        Task<TResult> ExecuteAsync(TQuery query, CancellationToken cancellationToken);
    }
}
