using Fitness_Api.CustomAtributes;
using Fitness_Api.Interfaces;
using Fitness_Api.Models;
using Fitness_Api.UniversalMethods;
using Microsoft.AspNetCore.Mvc;

namespace Fitness_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlansController : ControllerBase
{
    private readonly IPlanServices _planServices;

    public PlansController(IPlanServices planServices)
    {
        _planServices = planServices;
    }

    [HttpGet]
    [RoleAuthorize([RoleIds.Administrator, RoleIds.Client, RoleIds.Trainer])]
    public async Task<IActionResult> GetAll()
    {
        var token = Request.Headers["Authorization"].ToString();
        return await _planServices.GetPlans(token);
    }

    [HttpPost]
    [RoleAuthorize([RoleIds.Administrator, RoleIds.Trainer])]
    public async Task<IActionResult> Create([FromBody] PersonalPlan plan)
    {
        var token = Request.Headers["Authorization"].ToString();
        return await _planServices.CreatePlan(plan, token);
    }

    [HttpPost("{id:int}/progress/{progress:int}")]
    [RoleAuthorize([RoleIds.Administrator, RoleIds.Trainer])]
    public async Task<IActionResult> UpdateProgress(int id, int progress)
    {
        var token = Request.Headers["Authorization"].ToString();
        return await _planServices.UpdateProgress(id, progress, token);
    }
}
