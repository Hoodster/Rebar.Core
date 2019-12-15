using System.Threading;
using System.Threading.Tasks;

namespace Rebar.Core.Command
{
    public interface ICommandHandler<TCommand> where TCommand: ICommand
    {
        Task ExecuteAsync(TCommand command, CancellationToken token);
    }
}
