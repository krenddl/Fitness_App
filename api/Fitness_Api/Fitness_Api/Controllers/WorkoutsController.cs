using Fitness_Api.CustomAtributes;
using Fitness_Api.Interfaces;
using Fitness_Api.Models;
using Fitness_Api.Requests;
using Fitness_Api.UniversalMethods;
using Microsoft.AspNetCore.Mvc;

namespace Fitness_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkoutsController : ControllerBase
{
    private readonly IWorkoutServices _workoutServices;

    public WorkoutsController(IWorkoutServices workoutServices)
    {
        _workoutServices = workoutServices;
    }

    [HttpGet]
    [RoleAuthorize([RoleIds.Administrator, RoleIds.Client, RoleIds.Trainer, RoleIds.SalesManager])]
    public async Task<IActionResult> GetAll()
    {
        var token = Request.Headers["Authorization"].ToString();
        return await _workoutServices.GetWorkouts(token);
    }

    [HttpPost]
    [RoleAuthorize([RoleIds.Administrator, RoleIds.Trainer])]
    public async Task<IActionResult> Create([FromBody] WorkoutSession workout)
    {
        var token = Request.Headers["Authorization"].ToString();
        return await _workoutServices.CreateWorkout(workout, token);
    }

    [HttpPost("{id:int}/enroll")]
    [RoleAuthorize([RoleIds.Administrator, RoleIds.Client, RoleIds.SalesManager])]
    public async Task<IActionResult> Enroll(int id, [FromBody] EnrollWorkout request)
    {
        var token = Request.Headers["Authorization"].ToString();
        return await _workoutServices.Enroll(id, request, token);
    }
}
