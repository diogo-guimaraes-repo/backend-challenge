using System;
using System.Collections.Generic;

namespace Backend.Challenge.Models
{
  public record CommentAccess
  {
    public string Id { get; init; }
    public string EntityId { get; init; }
    public string UserId { get; init; }
    public DateTimeOffset AccessDate { get; init; }

    public static implicit operator CommentAccess(List<CommentAccess> v)
    {
      throw new NotImplementedException();
    }
  }
}