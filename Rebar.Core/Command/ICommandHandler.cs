﻿using System.Threading;
using System.Threading.Tasks;

namespace Rebar.Core.Command
{
    /// <summary>
    /// Asynchronous command handler interface.
    /// </summary>
    /// <typeparam name="TCommand">[<see cref="ICommand">ICommand</see>] Command.</typeparam>
    public interface ICommandHandler<TCommand> : ICommandHandlerBase<TCommand> where TCommand : ICommand
    {
        /// <summary>
        /// Describes how command is handled.
        /// </summary>
        /// <param name="command">Command.</param>
        void Execute(TCommand command);
    }
}
