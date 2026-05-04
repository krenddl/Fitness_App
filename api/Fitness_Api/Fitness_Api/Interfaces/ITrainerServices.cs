using Microsoft.AspNetCore.Mvc;

namespace Fitness_Api.Interfaces;

public interface ITrainerServices
{
    Task<IActionResult> GetAllTrainers();
    Task<IActionResult> GetMyTrainer(string token);
}
