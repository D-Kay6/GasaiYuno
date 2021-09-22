using Autofac;
using GasaiYuno.Discord.Infrastructure;
using GasaiYuno.Discord.Persistence.UnitOfWork;

namespace GasaiYuno.Discord.Modules
{
    internal class PersistenceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DataContext>().InstancePerDependency();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerDependency();
        }
    }
}