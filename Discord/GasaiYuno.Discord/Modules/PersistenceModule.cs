using Autofac;
using GasaiYuno.Discord.Domain.Persistence.UnitOfWork;
using GasaiYuno.Discord.Infrastructure;

namespace GasaiYuno.Discord.Modules
{
    internal class PersistenceModule : Module
    {
        private readonly string _connectionString;

        public PersistenceModule(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DataContext>().WithParameter("connectionString", _connectionString).InstancePerDependency();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerDependency();
        }
    }
}