using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rebar.Core.Query
{
    public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult> where TResult : IQueryResponse
    {
        Task<TResult> ExecuteAsync(TQuery query, CancellationToken cancellationToken);
    }
}
