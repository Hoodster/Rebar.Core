using System;
namespace Rebar.Core.Query
{
	public interface IQueryHandlerBase<TQuery, TResult> where TQuery : IQuery<TResult> where TResult : IQueryResponse
    {
	}
}

