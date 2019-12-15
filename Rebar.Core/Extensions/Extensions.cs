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

        public static void RegisterCommandHandlers(this ContainerBuilder self, Assembly executingAssembly)
        {
            self.RegisterTypesInstancePerLifetimeScope(CommandHandlerAssemblyTypeSuffix, executingAssembly);
        }

        public static void RegisterQueryHandlers(this ContainerBuilder self, Assembly executingAssembly)
        {
            self.RegisterTypesInstancePerLifetimeScope(QueryHandlerAssemblyTypeSuffix, executingAssembly);
        }

        public static void RegisterAll(this ContainerBuilder self, Assembly executingAssembly)
        {
            RegisterCommandHandlers(self, executingAssembly);
            RegisterQueryHandlers(self, executingAssembly);
        }

        public static void AddRebar(this ContainerBuilder builder)
        {
            builder.RegisterType<CommandDispatcher>().As<ICommandDispatcher>().InstancePerLifetimeScope();
            builder.RegisterType<CancellationTokenProvider>().As<ICancellationTokenProvider>().InstancePerLifetimeScope();
            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().InstancePerLifetimeScope();
            builder.RegisterType<QueryDispatcher>().As<IQueryDispatcher>().InstancePerLifetimeScope();
        }

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
