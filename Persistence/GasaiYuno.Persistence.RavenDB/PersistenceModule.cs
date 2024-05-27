using System.Security.Cryptography.X509Certificates;
using Autofac;
using GasaiYuno.Persistence.Data;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace GasaiYuno.Persistence;

public class PersistenceModule : Module
{
    public string Url { get; init; }
    public string Database { get; init; }
    public string Certificate { get; init; }

    protected override void Load(ContainerBuilder builder)
    {
        builder.Register(_ =>
        {
            var store = new DocumentStore
            {
                Database = Database,
                Certificate = new X509Certificate2(Certificate),
                Urls = new[] { Url }
            }.Initialize();
            return store;
        }).As<IDocumentStore>().InstancePerLifetimeScope();
        
        builder.Register(x =>
        {
            var documentStore = x.Resolve<IDocumentStore>();
            var session = documentStore.OpenAsyncSession();
            session.Advanced.UseOptimisticConcurrency = true;
            return session;
        }).As<IAsyncDocumentSession>().InstancePerDependency();

        builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerDependency();
        builder.RegisterGeneric(typeof(UnitOfWork<>)).As(typeof(IUnitOfWork<>)).InstancePerDependency();
    }
}