using System.Threading.Tasks;

namespace Rebar.Core.Command
{
    /// <summary>
    /// Command dispatcher interface.
    /// </summary>
    public interface ICommandDispatcher : IDispatcher
    {
        /// <summary>
        /// Command executor that takes command and resolves it with matching command handler.
        /// </summary>
        /// <typeparam name="TCommand"><see cref="ICommand"/></typeparam>
        /// <param name="command">Command.</param>
        /// <returns></returns>
        Task ExecuteAsync<TCommand>(TCommand command) where TCommand : ICommand;
    }
}