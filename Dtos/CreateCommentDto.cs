using System.ComponentModel.DataAnnotations;

namespace Backend.Challenge.Dtos
{
  public record CreateCommentDto
  {
    [Required]
    public string Text { get; init; }
  }
}