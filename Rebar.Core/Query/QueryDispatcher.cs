using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Rebar.Core.Cancellation;
using Rebar.Core.Exceptions;

namespace Rebar.Core.Query
{
    /// <summary>
    /// Query dispatcher class.
    /// </summary>
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

        /// <summary>
        /// Takes query and executes by matching <see cref="IQueryHandler{TQuery, TResult}">query handler</see>
        /// </summary>
        /// <typeparam name="TResult"><see cref="IQueryResponse">IQueryResponse</see> inherited class. See also: <seealso cref="IQuery{TResponse}">IQuery</seealso></typeparam>
        /// <param name="query">Query to execute.</param>
        /// <returns>Corresponding query response.</returns>
        /// <exception cref="ArgumentNullException">No command has been found.</exception>
        /// <exception cref="HandlerNotFoundException">No handler has been found.</exception>
        public TResult Execute<TResult>(IQuery<TResult> query) where TResult : IQueryResponse
        {
            var queryHandler = CreateHandler<IQueryHandler<IQuery<TResult>, TResult>, IQuery<TResult>, TResult>(query);
            return (TResult)queryHandler.Execute((dynamic)query);
        }

        /// <summary>
        /// Takes query and executes asynchronously by matching <see cref="IQueryHandler{TQuery, TResult}">query handler</see>
        /// </summary>
        /// <typeparam name="TResult"><see cref="IQueryResponse">IQueryResponse</see> inherited class. See also: <seealso cref="IQuery{TResponse}">IQuery</seealso></typeparam>
        /// <param name="query">Query to execute.</param>
        /// <returns>Corresponding query response.</returns>
        /// <exception cref="ArgumentNullException">No command has been found.</exception>
        /// <exception cref="HandlerNotFoundException">No handler has been found.</exception>
        public async Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query) where TResult : IQueryResponse
        {
            var queryHandler = CreateHandler<IAsyncQueryHandler<IQuery<TResult>, TResult>, IQuery<TResult>, TResult>(query);
            return await(Task<TResult>)queryHandler.ExecuteAsync((dynamic)query, _cancellationTokenProvider.GetCancellationToken());
        }

        /// <summary>
        /// Combines query files and creates handler.
        /// </summary>
        /// <param name="queryType">Type of query.</param>
        /// <param name="resultType">Type of query result.</param>
        /// <typeparam name="THandler"><see cref="IQueryHandlerBase{TQuery, TResult}"/></typeparam>
        /// <typeparam name="TQuery"><see cref="IQuery{TResponse}"/></typeparam>
        /// <typeparam name="TResult"><see cref="IQueryResponse"/></typeparam>
        /// <returns>Handler type</returns>
        private Type GetHandlerType<THandler, TQuery, TResult>(Type queryType, Type resultType)
            where TResult : IQueryResponse
            where TQuery : IQuery<TResult>
            where THandler : IQueryHandlerBase<TQuery, TResult>
        {
            if (Handlers.ContainsKey(queryType))
            {
                return Handlers[queryType];
            }

            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, resultType);
            Handlers[queryType] = handlerType;

            return handlerType;
        }

        /// <summary>
        /// Tries to resolve query handler and returns handler ready to execute.
        /// </summary>
        /// <typeparam name="THandler"><see cref="IQueryHandlerBase{TQuery, TResult}"/></typeparam>
        /// <typeparam name="TQuery"><see cref="IQuery{TResponse}"/></typeparam>
        /// <typeparam name="TResult"><see cref="IQueryResponse"/></typeparam>
        /// <param name="query">Query.</param>
        /// <returns><see cref="IQueryHandler{TQuery, TResult}"/> or <see cref="IAsyncQueryHandler{TQuery, TResult}"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="HandlerNotFoundException"></exception>
        private THandler CreateHandler<THandler, TQuery, TResult>(TQuery query)
            where TResult : IQueryResponse
            where TQuery : IQuery<TResult>
            where THandler : IQueryHandlerBase<TQuery, TResult>
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var queryType = query.GetType();
            var handlerType = GetHandlerType<THandler, TQuery, TResult>(queryType, typeof(TResult));

            if (!_resolver.TryResolve(handlerType, out dynamic queryHandler))
            {
                throw new HandlerNotFoundException($"Can't handle {nameof(query)}. Make sure provided query contains all files and is accessible.");
            }

            return queryHandler;
        }
    }
}
