using Raven.Client.Documents;

namespace Backend.Challenge.Persistence.Context
{
  public interface IRavenDbContext
  {
    public IDocumentStore store { get; }
  }
}