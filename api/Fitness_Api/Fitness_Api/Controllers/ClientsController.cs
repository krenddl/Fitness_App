using Fitness_Api.CustomAtributes;
using Fitness_Api.Interfaces;
using Fitness_Api.UniversalMethods;
using Microsoft.AspNetCore.Mvc;

namespace Fitness_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientServices _clientServices;

    public ClientsController(IClientServices clientServices)
    {
        _clientServices = clientServices;
    }

    [HttpGet]
    [RoleAuthorize([RoleIds.Administrator, RoleIds.SalesManager])]
    public async Task<IActionResult> GetAll()
    {
        return await _clientServices.GetAllClients();
    }

    [HttpGet("Me")]
    [RoleAuthorize([RoleIds.Client])]
    public async Task<IActionResult> Me()
    {
        var token = Request.Headers["Authorization"].ToString();
        return await _clientServices.GetMyClient(token);
    }
}
