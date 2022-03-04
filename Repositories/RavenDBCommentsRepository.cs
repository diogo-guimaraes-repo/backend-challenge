using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Challenge.Models;
using Backend.Challenge.Persistence.Context;
using Raven.Client.Documents;

namespace Backend.Challenge.Repositories
{
  public class RavenDBCommentsRepository : ICommentsRepository
  {
    private readonly IRavenDbContext context;

    public RavenDBCommentsRepository(IRavenDbContext dbContext)
    {
      context = dbContext;
    }
    public async Task CreateCommentAsync(Comment comment)
    {
      try
      {
        using var session = context.store.OpenAsyncSession();
        await session.StoreAsync(comment);
        await session.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw new RepositoryException(e.Message, e.InnerException);
      }
    }

    public async Task<Comment> GetCommentAsync(string id)
    {
      try
      {
        using var session = context.store.OpenAsyncSession();
        return await session.LoadAsync<Comment>(id);
      }
      catch (Exception e)
      {
        throw new RepositoryException(e.Message, e.InnerException);
      }
    }

    public async Task<IEnumerable<Comment>> GetCommentsAsync(string entityId, int pageSize, int pageNumber)
    {
      try
      {
        int skip = pageSize * (pageNumber - 1);
        int take = pageSize;

        using var session = context.store.OpenAsyncSession();

        return await session
                        .Query<Comment>()
                        .Where(comment => comment.EntityId == entityId)
                        .Skip(skip).Take(take)
                        .ToListAsync();
      }
      catch (Exception e)
      {
        throw new RepositoryException(e.Message, e.InnerException);
      }
    }

    public async Task<IEnumerable<Comment>> GetNewCommentsAsync(string entityId, int pageSize, int pageNumber, CommentAccess commentAccess)
    {
      try
      {
        int skip = pageSize * (pageNumber - 1);
        int take = pageSize;

        using var session = context.store.OpenAsyncSession();

        if (commentAccess is null)
        {
          return await GetCommentsAsync(entityId, pageSize, pageNumber);
        }
        return await session
                        .Query<Comment>()
                        .Where(comment => (comment.EntityId == entityId && comment.PublishedDate >= commentAccess.AccessDate))
                        .Skip(skip).Take(take)
                        .ToListAsync();
      }
      catch (Exception e)
      {
        throw new RepositoryException(e.Message, e.InnerException);
      }
    }

    public async Task<CommentAccess> GetCommentAccessAsync(string entityId, string userId)
    {
      try
      {
        using var session = context.store.OpenAsyncSession();

        IEnumerable<CommentAccess> commentAccessList = await session.Query<CommentAccess>()
                        .Where(access => access.EntityId == entityId)
                        .ToListAsync();

        CommentAccess commentAccess = null;

        if (commentAccessList.Count() > 0)
        {
          commentAccess = commentAccessList.First();
        }

        return commentAccess;
      }
      catch (Exception e)
      {
        throw new RepositoryException(e.Message, e.InnerException);
      }
    }

    public async Task CreateOrUpdateCommentAccess(CommentAccess commentAccess)
    {
      try
      {
        using var session = context.store.OpenAsyncSession();
        await session.StoreAsync(commentAccess);
        await session.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw new RepositoryException(e.Message, e.InnerException);
      }
    }
  }
  public class RepositoryException : Exception
  {
    public RepositoryException(string message, Exception exception) : base(message, exception)
    {

    }
  }
}