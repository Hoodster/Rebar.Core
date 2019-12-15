using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Rebar.Core.Cancellation;
using Rebar.Core.Exceptions;

namespace Rebar.Core.Query
{
    internal class QueryDispatcher : IQueryDispatcher
    {
        private static readonly Dictionary<Type, Type> Handlers = new Dictionary<Type, Type>();
        private readonly IComponentContext _resolver;
        private readonly ICancellationTokenProvider _cancellationTokenProvider;

        public QueryDispatcher(IComponentContext resolver, ICancellationTokenProvider provider)
        {
            _resolver = resolver;
            _cancellationTokenProvider = provider;
        }

        public async Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query) where TResult : IQueryResponse
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var queryType = query.GetType();
            var handlerType = GetHandlerType(queryType, typeof(TResult));

            if (!_resolver.TryResolve(handlerType, out dynamic queryHandler))
            {
                throw new HandlerNotFoundException($"Can't handle {nameof(query)}.");
            }
            return await(Task<TResult>)queryHandler.ExecuteAsync((dynamic)query, _cancellationTokenProvider.GetCancellationToken());
        }


        public Type GetHandlerType(Type queryType, Type resultType)
        {
            if (Handlers.ContainsKey(queryType))
            {
                return Handlers[queryType];
            }

            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, resultType);
            Handlers[queryType] = handlerType;

            return handlerType;
        }
    }
}
