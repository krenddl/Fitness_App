using Fitness_Api.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Fitness_Api.Interfaces;

public interface IUserServices
{
    Task<IActionResult> Registration(Registration regUser);
    Task<IActionResult> Authorize(Auth authUser);
    Task<IActionResult> Logout(string token);
    Task<IActionResult> Session(string token);
    Task<IActionResult> Profile(Profile profile, string token);
    Task<IActionResult> GetAllUsers();
    Task<IActionResult> CreateNewUser(CreateNewUser regUser);
    Task<IActionResult> GetDemoAccounts();
}
