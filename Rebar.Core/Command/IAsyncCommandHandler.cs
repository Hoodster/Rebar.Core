using System.Threading;
using System.Threading.Tasks;

namespace Rebar.Core.Command
{
    /// <summary>
    /// Asynchronous command handler interface.
    /// </summary>
    /// <typeparam name="TCommand">[<see cref="ICommand">ICommand</see>] Command.</typeparam>
    public interface IAsyncCommandHandler<TCommand> : ICommandHandlerBase<TCommand> where TCommand : ICommand
    {
        /// <summary>
        /// Describes how command is asynchronously handled.
        /// </summary>
        /// <param name="command">Command.</param>
        /// <param name="token">Cancellation token instance.</param>
        Task ExecuteAsync(TCommand command, CancellationToken token);
    }
}
