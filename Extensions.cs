using Backend.Challenge.Dtos;
using Backend.Challenge.Models;

namespace Backend.Challenge
{
  public static class Extensions
  {
    public static CommentDto asDto(this Comment comment)
    {
      return new CommentDto
      {
        Id = comment.Id,
        EntityId = comment.EntityId,
        AuthorId = comment.AuthorId,
        Text = comment.Text,
        PublishedDate = comment.PublishedDate
      };
    }
  }
}