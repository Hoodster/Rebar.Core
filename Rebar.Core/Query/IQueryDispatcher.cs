using System;
using System.Threading.Tasks;

namespace Rebar.Core.Query
{
    public interface IQueryDispatcher : IDispatcher
    {
        Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query) where TResult : IQueryResponse;
        TResult Execute<TResult>(IQuery<TResult> query) where TResult : IQueryResponse;
    }
}
