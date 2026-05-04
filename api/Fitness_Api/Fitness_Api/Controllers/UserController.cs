using Fitness_Api.CustomAtributes;
using Fitness_Api.Interfaces;
using Fitness_Api.Requests;
using Fitness_Api.UniversalMethods;
using Microsoft.AspNetCore.Mvc;

namespace Fitness_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : Controller
{
    private readonly IUserServices _userServices;

    public UserController(IUserServices userServices)
    {
        _userServices = userServices;
    }

    [HttpPost("Registration")]
    public async Task<IActionResult> Registration([FromBody] Registration regUser)
    {
        return await _userServices.Registration(regUser);
    }

    [HttpPost("Authorize")]
    public async Task<IActionResult> Authorize([FromBody] Auth auth)
    {
        return await _userServices.Authorize(auth);
    }

    [HttpPost("Logout")]
    public async Task<IActionResult> Logout()
    {
        var token = Request.Headers["Authorization"].ToString();
        return await _userServices.Logout(token);
    }

    [HttpGet("Session")]
    [RoleAuthorize([RoleIds.Administrator, RoleIds.Client, RoleIds.Trainer, RoleIds.SalesManager])]
    public async Task<IActionResult> Session()
    {
        var token = Request.Headers["Authorization"].ToString();
        return await _userServices.Session(token);
    }

    [HttpPut("Profile")]
    [RoleAuthorize([RoleIds.Administrator, RoleIds.Client, RoleIds.Trainer, RoleIds.SalesManager])]
    public async Task<IActionResult> Profile([FromBody] Profile profile)
    {
        var token = Request.Headers["Authorization"].ToString();
        return await _userServices.Profile(profile, token);
    }

    [HttpGet("GetAllUsers")]
    [RoleAuthorize([RoleIds.Administrator])]
    public async Task<IActionResult> GetAllUsers()
    {
        return await _userServices.GetAllUsers();
    }

    [HttpPost("CreateNewUser")]
    [RoleAuthorize([RoleIds.Administrator])]
    public async Task<IActionResult> CreateNewUser([FromBody] CreateNewUser createNewUser)
    {
        return await _userServices.CreateNewUser(createNewUser);
    }

    [HttpGet("DemoAccounts")]
    public async Task<IActionResult> DemoAccounts()
    {
        return await _userServices.GetDemoAccounts();
    }
}
