using Autofac;
using GasaiYuno.Discord.Domain.Persistence.UnitOfWork;
using GasaiYuno.Discord.RavenDB.Indexes;
using Raven.Client.Documents;
using System.Security.Cryptography.X509Certificates;

namespace GasaiYuno.Discord.RavenDB;

public class PersistenceModule : Module
{
    private readonly string _url;
    private readonly string _database;
    private readonly string _certificate;

    /// <inheritdoc />
    public PersistenceModule() { }

    /// <inheritdoc />
    public PersistenceModule(string url, string database, string certificate)
    {
        _url = url;
        _database = database;
        _certificate = certificate;
    }

    protected override void Load(ContainerBuilder builder)
    {
        builder.Register(_ =>
        {
            var store = new DocumentStore
            {
                Database = _database,
                Certificate = new X509Certificate2(_certificate),
                Urls = new[] { _url }
            }.Initialize();
            new CustomCommands_ByServerAndCommand().Execute(store);
            return store;
        }).As<IDocumentStore>().InstancePerLifetimeScope();

        builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerDependency();
    }
}