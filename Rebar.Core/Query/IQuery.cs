using System;
namespace Rebar.Core.Query
{
    public interface IQuery<in TResponse> where TResponse : IQueryResponse
    {
    }
}
