using System;
using System.Linq;
using System.Threading.Tasks;
using Backend.Challenge.Dtos;
using Backend.Challenge.Models;
using Backend.Challenge.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Backend.Challenge.Controllers
{
  [ApiController]
  [Route("comments")]
  public class CommentsController : ControllerBase
  {
    private readonly ICommentsRepository repository;
    private readonly ILogger<CommentsController> logger;

    public CommentsController(ICommentsRepository repository, ILogger<CommentsController> logger)
    {
      this.repository = repository;
      this.logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<CommentDto>> GetCommentsAsync(string entityId, string userId, int pageSize, int pageNumber)
    {
      var comments = (await repository.GetCommentsAsync(entityId, pageSize, pageNumber))
                      .Select(comment => comment.asDto());

      var commentsAccess = await repository.GetCommentAccessAsync(entityId, userId);

      if (commentsAccess is null)
      {
        try
        {
          await CreateCommentsAccess(entityId, userId);
        }
        catch (Exception e)
        {
          logger.LogError(e.Message, e.InnerException);
          return StatusCode(500);
        }
      }
      else
      {
        try
        {
          await UpdateCommentsAccess(commentsAccess);
        }
        catch (Exception e)
        {
          logger.LogError(e.Message, e.InnerException);
          return StatusCode(500);
        }
      }

      return Ok(comments);
    }

    [HttpGet("new")]
    public async Task<ActionResult<CommentDto>> GetNewCommentsAsync(string entityId, string userId, int pageSize, int pageNumber)
    {
      var commentsAccess = await repository.GetCommentAccessAsync(entityId, userId);

      var comments = (await repository.GetNewCommentsAsync(entityId, pageSize, pageNumber, commentsAccess))
                              .Select(comment => comment.asDto());

      try
      {
        await CreateOrUpdateAccessDetails(commentsAccess, entityId, userId);
      }
      catch (Exception e)
      {
        logger.LogError(e.Message, e.InnerException);
        return StatusCode(500);
      }

      return Ok(comments);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CommentDto>> GetCommentAsync(string id)
    {
      var comment = await repository.GetCommentAsync(id);

      if (comment is null)
      {
        return NotFound();
      }
      return comment.asDto();
    }

    [HttpPost]
    public async Task<ActionResult<CommentDto>> CreateComment(CreateCommentDto commentDto, string entityId, string authorId)
    {
      Comment comment = new()
      {
        Id = string.Empty,
        EntityId = entityId,
        AuthorId = authorId,
        Text = commentDto.Text,
        PublishedDate = DateTimeOffset.UtcNow
      };

      try
      {
        await repository.CreateCommentAsync(comment);
      }
      catch (RepositoryException e)
      {
        logger.LogError(e.Message, e.InnerException);
        return StatusCode(500);
      }

      return CreatedAtAction(nameof(GetCommentAsync), new { id = comment.Id }, comment.asDto());
    }

    private async Task CreateOrUpdateAccessDetails(CommentAccess commentsAccess, string entityId, string userId)
    {
      if (commentsAccess is null)
      {
        try
        {
          await CreateCommentsAccess(entityId, userId);
        }
        catch (Exception e)
        {
          logger.LogError(e.Message, e.InnerException);
        }
      }
      else
      {
        try
        {
          await UpdateCommentsAccess(commentsAccess);
        }
        catch (Exception e)
        {
          logger.LogError(e.Message, e.InnerException);
        }
      }
    }

    private async Task CreateCommentsAccess(string entityId, string userId)
    {
      CommentAccess commentAccess = new()
      {
        Id = string.Empty,
        EntityId = entityId,
        UserId = userId,
        AccessDate = DateTimeOffset.UtcNow
      };

      try
      {
        await repository.CreateOrUpdateCommentAccess(commentAccess);
      }
      catch (RepositoryException e)
      {
        logger.LogError(e.Message, e.InnerException);
      }
    }

    private async Task UpdateCommentsAccess(CommentAccess commentAccess)
    {
      CommentAccess updatedCommentAccess = commentAccess with
      {
        AccessDate = DateTimeOffset.UtcNow
      };

      try
      {
        await repository.CreateOrUpdateCommentAccess(updatedCommentAccess);
      }
      catch (RepositoryException e)
      {
        logger.LogError(e.Message, e.InnerException);
      }
    }
  }
}
