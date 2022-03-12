using System.Linq;
using Backend.Challenge.Dtos;
using Backend.Challenge.ServiceModels;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Challenge.Controllers
{
  public class MainController : Controller
  {
    public MainController()
    {
    }

    // This action responds to the url /main/users/42 and /main/users?id=4&id=10
    public GetUserResponse Users(int[] id)
    {
      return new GetUserResponse
      {
        Users = id.ToDictionary(i => i, i => new UserDto
        {
          Id = i,
          Username = $"User {i}",
          Email = $"user-{i}@example.com"
        })
      };
    }

    // TODO: An action to return a paged list of comments

    // TODO: An action to add a comment
  }
}