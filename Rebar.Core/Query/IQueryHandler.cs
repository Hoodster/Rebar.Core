using System.Threading;
using System.Threading.Tasks;

namespace Rebar.Core.Query
{
    public interface IQueryHandler<TQuery, TResult> : IQueryHandlerBase<TQuery, TResult> where TQuery : IQuery<TResult> where TResult : IQueryResponse
    {
        TResult Execute(TQuery query);
    }
}
