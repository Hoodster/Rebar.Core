using System;
using System.Reflection;
using Autofac;
using Autofac.Builder;
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
        /// <param name="instanceType">Type of instance lifetime.</param>
        public static void RegisterCommandHandlers(this ContainerBuilder self, Assembly executingAssembly, InstanceTypes instanceType = InstanceTypes.LifetimeScope, object[] lifetimeScopeTags = null)
        {
            self.RegisterTypesPerInstanceLife(CommandHandlerAssemblyTypeSuffix, executingAssembly, instanceType, lifetimeScopeTags);
        }

        /// <summary>
        /// Registers query handlers and adds them to DI scope.
        /// </summary>
        /// <param name="executingAssembly">Queries executing assemblt.</param>
        /// <param name="instanceType">Type of instance lifetime.</param>
        /// <param name="lifetimeScopeTags">Tag applied to matching lifetime scopes. Optional for request scope, required for matching lifetime scope.</param>
        public static void RegisterQueryHandlers(this ContainerBuilder self, Assembly executingAssembly, InstanceTypes instanceType = InstanceTypes.LifetimeScope, object[] lifetimeScopeTags = null)
        {
            self.RegisterTypesPerInstanceLife(QueryHandlerAssemblyTypeSuffix, executingAssembly, instanceType, lifetimeScopeTags);
        }

        /// <summary>
        /// Allows to register both commands and queries at once and adds them to DI scope.
        /// </summary>
        /// <param name="executingAssembly">Commands and queries executing assembly.</param>
        public static void RegisterAll(this ContainerBuilder self, Assembly executingAssembly, InstanceTypes instanceType = InstanceTypes.LifetimeScope)
        {
            RegisterCommandHandlers(self, executingAssembly, instanceType);
            RegisterQueryHandlers(self, executingAssembly, instanceType);
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
        /// Sets life scope for handlers
        /// </summary>
        /// <param name="self"></param>
        /// <param name="typeSuffix">Type of suffix</param>
        /// <param name="executingAssembly"></param>
        private static void RegisterTypesPerInstanceLife(this ContainerBuilder self, string typeSuffix, Assembly executingAssembly, InstanceTypes instanceType, object[] lifetimeScopeTags)
        {
            var registeredType = self.RegisterTypesBase(typeSuffix, executingAssembly);

            switch(instanceType)
            {
                case InstanceTypes.LifetimeScope:
                    registeredType.InstancePerLifetimeScope();
                    break;
                case InstanceTypes.DependencyScope:
                    registeredType.InstancePerDependency();
                    break;
                case InstanceTypes.RequestScope:
                    registeredType.InstancePerRequest(lifetimeScopeTags);
                    break;
                case InstanceTypes.MatchingLifetimeScope:
                    registeredType.InstancePerMatchingLifetimeScope(lifetimeScopeTags);
                    break;
                default:
                    throw new Exception("Not supported dependency scope.");
            }

        }

        private static IRegistrationBuilder<object, Autofac.Features.Scanning.ScanningActivatorData, DynamicRegistrationStyle> RegisterTypesBase(this ContainerBuilder self, string typeSuffix, Assembly executingAssembly)
        {
            return self.RegisterAssemblyTypes(executingAssembly)
                .Where(t => t.Name.EndsWith(typeSuffix, StringComparison.Ordinal) && !t.IsAbstract)
                .AsImplementedInterfaces()
                .AsSelf();
        }
    }
}
