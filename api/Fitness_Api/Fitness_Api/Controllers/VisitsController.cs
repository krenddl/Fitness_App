using Fitness_Api.CustomAtributes;
using Fitness_Api.Interfaces;
using Fitness_Api.Requests;
using Fitness_Api.UniversalMethods;
using Microsoft.AspNetCore.Mvc;

namespace Fitness_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VisitsController : ControllerBase
{
    private readonly IVisitServices _visitServices;

    public VisitsController(IVisitServices visitServices)
    {
        _visitServices = visitServices;
    }

    [HttpGet]
    [RoleAuthorize([RoleIds.Administrator, RoleIds.Client, RoleIds.SalesManager])]
    public async Task<IActionResult> GetAll()
    {
        var token = Request.Headers["Authorization"].ToString();
        return await _visitServices.GetVisits(token);
    }

    [HttpPost("enter")]
    [RoleAuthorize([RoleIds.Administrator, RoleIds.Client, RoleIds.SalesManager])]
    public async Task<IActionResult> Enter([FromBody] EnterVisitRequest request)
    {
        var token = Request.Headers["Authorization"].ToString();
        return await _visitServices.Enter(request, token);
    }

    [HttpPost("exit/{id:int}")]
    [RoleAuthorize([RoleIds.Administrator, RoleIds.Client, RoleIds.SalesManager])]
    public async Task<IActionResult> Exit(int id)
    {
        var token = Request.Headers["Authorization"].ToString();
        return await _visitServices.Exit(id, token);
    }
}
