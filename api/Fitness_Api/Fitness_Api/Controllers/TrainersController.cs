using Fitness_Api.CustomAtributes;
using Fitness_Api.Interfaces;
using Fitness_Api.UniversalMethods;
using Microsoft.AspNetCore.Mvc;

namespace Fitness_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TrainersController : ControllerBase
{
    private readonly ITrainerServices _trainerServices;

    public TrainersController(ITrainerServices trainerServices)
    {
        _trainerServices = trainerServices;
    }

    [HttpGet]
    [RoleAuthorize([RoleIds.Administrator, RoleIds.Client, RoleIds.Trainer, RoleIds.SalesManager])]
    public async Task<IActionResult> GetAll()
    {
        return await _trainerServices.GetAllTrainers();
    }

    [HttpGet("Me")]
    [RoleAuthorize([RoleIds.Trainer])]
    public async Task<IActionResult> Me()
    {
        var token = Request.Headers["Authorization"].ToString();
        return await _trainerServices.GetMyTrainer(token);
    }
}
