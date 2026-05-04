using Fitness_Api.CustomAtributes;
using Fitness_Api.Interfaces;
using Fitness_Api.UniversalMethods;
using Microsoft.AspNetCore.Mvc;

namespace Fitness_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportServices _reportServices;

    public ReportsController(IReportServices reportServices)
    {
        _reportServices = reportServices;
    }

    [HttpGet("summary")]
    [RoleAuthorize([RoleIds.Administrator, RoleIds.SalesManager])]
    public async Task<IActionResult> Summary()
    {
        return await _reportServices.GetSummary();
    }
}
