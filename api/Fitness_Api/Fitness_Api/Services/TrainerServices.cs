using Fitness_Api.Data;
using Fitness_Api.Interfaces;
using Fitness_Api.UniversalMethods;
using Microsoft.AspNetCore.Mvc;

namespace Fitness_Api.Services;

public class TrainerServices : ITrainerServices
{
    private readonly InMemoryStore _context;
    private readonly SessionResolver _sessionResolver;

    public TrainerServices(InMemoryStore context, SessionResolver sessionResolver)
    {
        _context = context;
        _sessionResolver = sessionResolver;
    }

    public Task<IActionResult> GetAllTrainers()
    {
        return Task.FromResult<IActionResult>(new OkObjectResult(_context.Trainers.OrderBy(x => x.FullName)));
    }

    public Task<IActionResult> GetMyTrainer(string token)
    {
        var trainerId = _sessionResolver.GetTrainerId(token);
        var trainer = trainerId.HasValue ? _context.Trainers.FirstOrDefault(x => x.Id == trainerId.Value) : null;

        if (trainer is null)
        {
            return Task.FromResult<IActionResult>(new NotFoundObjectResult(new
            {
                status = false,
                message = "Тренер не найден"
            }));
        }

        return Task.FromResult<IActionResult>(new OkObjectResult(trainer));
    }
}
