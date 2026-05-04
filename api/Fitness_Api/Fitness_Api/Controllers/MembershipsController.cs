using Fitness_Api.CustomAtributes;
using Fitness_Api.Interfaces;
using Fitness_Api.Models;
using Fitness_Api.UniversalMethods;
using Microsoft.AspNetCore.Mvc;

namespace Fitness_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MembershipsController : ControllerBase
{
    private readonly IMembershipServices _membershipServices;

    public MembershipsController(IMembershipServices membershipServices)
    {
        _membershipServices = membershipServices;
    }

    [HttpGet]
    [RoleAuthorize([RoleIds.Administrator, RoleIds.Client, RoleIds.SalesManager])]
    public async Task<IActionResult> GetAll()
    {
        var token = Request.Headers["Authorization"].ToString();
        return await _membershipServices.GetMemberships(token);
    }

    [HttpPost]
    [RoleAuthorize([RoleIds.Administrator, RoleIds.SalesManager])]
    public async Task<IActionResult> Create([FromBody] Membership membership)
    {
        return await _membershipServices.CreateMembership(membership);
    }

    [HttpPost("{id:int}/freeze")]
    [RoleAuthorize([RoleIds.Administrator, RoleIds.SalesManager])]
    public async Task<IActionResult> Freeze(int id)
    {
        return await _membershipServices.FreezeMembership(id);
    }

    [HttpPost("{id:int}/extend/{days:int}")]
    [RoleAuthorize([RoleIds.Administrator, RoleIds.SalesManager])]
    public async Task<IActionResult> Extend(int id, int days)
    {
        return await _membershipServices.ExtendMembership(id, days);
    }

    [HttpGet("reminders")]
    [RoleAuthorize([RoleIds.Administrator, RoleIds.SalesManager])]
    public async Task<IActionResult> Reminders()
    {
        return await _membershipServices.GetReminders();
    }
}
