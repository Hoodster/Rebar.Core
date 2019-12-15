using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Rebar.Core.Cancellation;

namespace Rebar.Core.Command
{
    internal class CommandDispatcher : ICommandDispatcher
    {
        private IComponentContext _resolver;
        private readonly ICancellationTokenProvider _provider;

        public CommandDispatcher(ICancellationTokenProvider provider, IComponentContext resolver)
        {
            this._provider = provider;
            this._resolver = resolver;
        }

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
