using System;
using System.Reflection;
using Autofac;
using Microsoft.AspNetCore.Http;
using Rebar.Core.Cancellation;
using Rebar.Core.Command;
using Rebar.Core.Query;

namespace Rebar.Core
{
    public static class Extensions
    {
        private const string CommandHandlerAssemblyTypeSuffix = "CommandHandler";
        private const string QueryHandlerAssemblyTypeSuffix = "QueryHandler";

        /// <summary>
        /// Registers command handlers and adds them to DI scope.
        /// </summary>
        /// <param name="executingAssembly">Commands executing assembly.</param>
        public static void RegisterCommandHandlers(this ContainerBuilder self, Assembly executingAssembly)
        {
            self.RegisterTypesInstancePerLifetimeScope(CommandHandlerAssemblyTypeSuffix, executingAssembly);
        }

        /// <summary>
        /// Registers query handlers and adds them to DI scope.
        /// </summary>
        /// <param name="executingAssembly">Queries executing assemblt.</param>
        public static void RegisterQueryHandlers(this ContainerBuilder self, Assembly executingAssembly)
        {
            self.RegisterTypesInstancePerLifetimeScope(QueryHandlerAssemblyTypeSuffix, executingAssembly);
        }

        /// <summary>
        /// Allows to register both commands and queries at once and adds them to DI scope.
        /// </summary>
        /// <param name="executingAssembly">Commands and queries executing assembly.</param>
        public static void RegisterAll(this ContainerBuilder self, Assembly executingAssembly)
        {
            RegisterCommandHandlers(self, executingAssembly);
            RegisterQueryHandlers(self, executingAssembly);
        }

        /// <summary>
        /// Adds Rebar.Core to DI scope.
        /// </summary>
        /// <param name="builder"></param>
        public static void AddRebar(this ContainerBuilder builder)
        {
            builder.RegisterType<CommandDispatcher>().As<ICommandDispatcher>().InstancePerLifetimeScope();
            builder.RegisterType<CancellationTokenProvider>().As<ICancellationTokenProvider>().InstancePerLifetimeScope();
            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().InstancePerLifetimeScope();
            builder.RegisterType<QueryDispatcher>().As<IQueryDispatcher>().InstancePerLifetimeScope();
        }

        /// <summary>
        /// Sets lifetime scope for handlers
        /// </summary>
        /// <param name="self"></param>
        /// <param name="typeSuffix">Type of suffix</param>
        /// <param name="executingAssembly"></param>
        private static void RegisterTypesInstancePerLifetimeScope(this ContainerBuilder self, string typeSuffix, Assembly executingAssembly)
        {
            self.RegisterAssemblyTypes(executingAssembly)
                .Where(t => t.Name.EndsWith(typeSuffix, System.StringComparison.Ordinal) && !t.IsAbstract)
                .AsImplementedInterfaces()
                .AsSelf()
                .InstancePerLifetimeScope();
        }
    }
}
