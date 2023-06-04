using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Rebar.Core.Cancellation;

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
        /// Command executor that takes command and resolves it with matching command handler.
        /// </summary>
        /// <typeparam name="TCommand"><see cref="ICommand"/></typeparam>
        /// <param name="command">Command.</param>
        /// <exception cref="ArgumentNullException">No command has been found.</exception>
        /// <exception cref="DependencyResolutionException">No matching handler has been found.</exception>
        public async Task ExecuteAsync<TCommand>(TCommand command) where TCommand : ICommand
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            var handler = _resolver.Resolve<ICommandHandler<TCommand>>();

            if (handler == null)
            {
                throw new DependencyResolutionException(typeof(TCommand).Name);
            }

            await handler.ExecuteAsync(command, _provider.GetCancellationToken());
        }
    }
}
