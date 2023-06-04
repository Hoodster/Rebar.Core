using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Rebar.Core.Cancellation;
using Rebar.Core.Exceptions;

namespace Rebar.Core.Command
{
    /// <summary>
    /// Command dispatcher class. Used to resolve command with its command handler.
    /// </summary>
    internal class CommandDispatcher : ICommandDispatcher
    {
        private readonly IComponentContext _resolver;
        private readonly ICancellationTokenProvider _provider;

        public CommandDispatcher(ICancellationTokenProvider provider, IComponentContext resolver)
        {
            this._provider = provider;
            this._resolver = resolver;
        }

        /// <summary>
        /// Command performer that takes command and resolves it with matching command handler.
        /// </summary>
        /// <typeparam name="TCommand"><see cref="ICommand"/></typeparam>
        /// <param name="command">Command.</param>
        /// <exception cref="ArgumentNullException">No command has been found.</exception>
        /// <exception cref="HandlerNotFoundException">No matching handler has been found.</exception>
        public void Execute<TCommand>(TCommand command) where TCommand : ICommand
        {
            var handler = CreateHandler<ICommandHandler<TCommand>, TCommand>(command);
            handler.Execute(command);
        }

        /// <summary>
        /// Asynchronous command performer that takes command and resolves it with matching command handler.
        /// </summary>
        /// <typeparam name="TCommand"><see cref="ICommand"/></typeparam>
        /// <param name="command">Command.</param>
        /// <exception cref="ArgumentNullException">No command has been found.</exception>
        /// <exception cref="HandlerNotFoundException">No matching handler has been found.</exception>
        public async Task ExecuteAsync<TCommand>(TCommand command) where TCommand : ICommand
        {
            var handler = CreateHandler<IAsyncCommandHandler<TCommand>, TCommand>(command);
            await handler.ExecuteAsync(command, _provider.GetCancellationToken());
        }

        /// <summary>
        /// Tries to resolve command handler and returns handler ready to execute.
        /// </summary>
        /// <typeparam name="THandler"><see cref="ICommandHandlerBase{TCommand}"/></typeparam>
        /// <typeparam name="TCommand"><see cref="ICommand"/></typeparam>
        /// <param name="command">Command.</param>
        /// <returns><see cref="ICommandHandler{TCommand}"/> or <see cref="IAsyncCommandHandler{TCommand}"/></returns>
        /// <exception cref="ArgumentNullException">No command has been found.</exception>
        /// <exception cref="HandlerNotFoundException">No matching handler has been found.</exception>
        private THandler CreateHandler<THandler, TCommand>(TCommand command)
            where TCommand : ICommand
            where THandler : ICommandHandlerBase<TCommand>
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            var handler = _resolver.Resolve<THandler>();

            if (handler == null)
            {
                throw new HandlerNotFoundException(typeof(TCommand).Name);
            }

            return handler;
        }
    }
}
