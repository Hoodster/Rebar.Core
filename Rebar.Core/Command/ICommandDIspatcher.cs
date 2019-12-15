using System.Threading.Tasks;

namespace Rebar.Core.Command
{
    public interface ICommandDispatcher : IDispatcher
    {
        Task ExecuteAsync<TCommand>(TCommand command) where TCommand : ICommand;
    }
}
