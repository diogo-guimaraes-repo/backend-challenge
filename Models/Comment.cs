using System;

namespace Backend.Challenge.Models
{
  public record Comment
  {
    public string Id { get; init; }
    public string EntityId { get; init; }
    public string AuthorId { get; init; }
    public string Text { get; init; }
    public DateTimeOffset PublishedDate { get; init; }
  }
}