using Autofac;
using GasaiYuno.Discord.Infrastructure;
using GasaiYuno.Discord.Infrastructure.Repositories;
using GasaiYuno.Discord.Infrastructure.UnitOfWorks;
using GasaiYuno.Discord.Persistence.Repositories;
using GasaiYuno.Discord.Persistence.UnitOfWork;

namespace GasaiYuno.Discord.Modules
{
    internal class PersistenceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DataContext>().InstancePerDependency();

            builder.RegisterType<BanUnitOfWork>().As<IUnitOfWork<IBanRepository>>().InstancePerDependency();
            builder.RegisterType<CommandUnitOfWork>().As<IUnitOfWork<ICommandRepository>>().InstancePerDependency();
            builder.RegisterType<DynamicChannelUnitOfWork>().As<IUnitOfWork<IDynamicChannelRepository>>().InstancePerDependency();
            //builder.RegisterType<DynamicRoleUnitOfWork>().As<IUnitOfWork<IDynamicRoleRepository>>().InstancePerDependency();
            builder.RegisterType<PollUnitOfWork>().As<IUnitOfWork<IPollRepository>>().InstancePerDependency();
            builder.RegisterType<LanguageUnitOfWork>().As<IUnitOfWork<ILanguageRepository>>().InstancePerDependency();
            builder.RegisterType<NotificationUnitOfWork>().As<IUnitOfWork<INotificationRepository>>().InstancePerDependency();
            builder.RegisterType<ServerUnitOfWork>().As<IUnitOfWork<IServerRepository>>().InstancePerDependency();

            builder.RegisterType<BanRepository>().As<IBanRepository>().InstancePerDependency();
            builder.RegisterType<CommandRepository>().As<ICommandRepository>().InstancePerDependency();
            builder.RegisterType<DynamicChannelRepository>().As<IDynamicChannelRepository>().InstancePerDependency();
            //builder.RegisterType<DynamicRoleRepository>().As<IDynamicRoleRepository>().InstancePerDependency();
            builder.RegisterType<PollRepository>().As<IPollRepository>().InstancePerDependency();
            builder.RegisterType<LanguageRepository>().As<ILanguageRepository>().InstancePerDependency();
            builder.RegisterType<NotificationRepository>().As<INotificationRepository>().InstancePerDependency();
            builder.RegisterType<ServerRepository>().As<IServerRepository>().InstancePerDependency();
        }
    }
}