using Microsoft.Extensions.Options;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations;
using Raven.Client.Exceptions.Database;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;

namespace Backend.Challenge.Persistence.Context
{
  public class RavenDbContext : IRavenDbContext
  {
    private readonly DocumentStore localstore;
    public IDocumentStore store => localstore;

    private readonly PersistenceSettings persistenceSettings;

    public RavenDbContext(IOptionsMonitor<PersistenceSettings> settings)
    {
      persistenceSettings = settings.CurrentValue;

      localstore = new DocumentStore()
      {
        Database = persistenceSettings.DatabaseName,
        Urls = persistenceSettings.Urls
      };

      localstore.Initialize();
      this.EnsureDatabaseIsCreated();
    }

    public void EnsureDatabaseIsCreated()
    {
      try
      {
        localstore.Maintenance.ForDatabase(persistenceSettings.DatabaseName).Send(new GetStatisticsOperation());
      }
      catch (DatabaseDoesNotExistException)
      {
        localstore.Maintenance.Server.Send(new CreateDatabaseOperation(new DatabaseRecord(persistenceSettings.DatabaseName)));
      }
    }
  }
}