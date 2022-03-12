using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Challenge.Models;

namespace Backend.Challenge.Repositories
{
  public interface ICommentsRepository
  {
    Task<IEnumerable<Comment>> GetCommentsAsync(string entityId, int pageSize, int pageNumber);
    Task<IEnumerable<Comment>> GetNewCommentsAsync(string entityId, int pageSize, int pageNumber, CommentAccess commentAccess);
    Task<Comment> GetCommentAsync(string id);
    Task CreateCommentAsync(Comment comment);
    Task<CommentAccess> GetCommentAccessAsync(string entityId, string userId);
    Task CreateOrUpdateCommentAccess(CommentAccess commentAccess);
  }
}