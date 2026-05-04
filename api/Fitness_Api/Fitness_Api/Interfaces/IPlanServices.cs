using Fitness_Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fitness_Api.Interfaces;

public interface IPlanServices
{
    Task<IActionResult> GetPlans(string token);
    Task<IActionResult> CreatePlan(PersonalPlan plan, string token);
    Task<IActionResult> UpdateProgress(int id, int progress, string token);
}
