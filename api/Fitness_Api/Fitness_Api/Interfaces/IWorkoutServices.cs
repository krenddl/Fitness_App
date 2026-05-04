using Fitness_Api.Models;
using Fitness_Api.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Fitness_Api.Interfaces;

public interface IWorkoutServices
{
    Task<IActionResult> GetWorkouts(string token);
    Task<IActionResult> CreateWorkout(WorkoutSession workout, string token);
    Task<IActionResult> Enroll(int id, EnrollWorkout request, string token);
}
