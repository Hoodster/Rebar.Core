using System.Threading;
using System.Threading.Tasks;

namespace Rebar.Core.Command
{
    /// <summary>
    /// Command handler interface.
    /// </summary>
    /// <typeparam name="TCommand">[<see cref="ICommand">ICommand</see>] Command.</typeparam>
    public interface ICommandHandler<TCommand> where TCommand: ICommand
    {
        /// <summary>
        /// Describes how command is handled.
        /// </summary>
        /// <param name="command">Command.</param>
        /// <param name="token">Cancellation token instance.</param>
        /// <returns></returns>
        Task ExecuteAsync(TCommand command, CancellationToken token);
    }
}
