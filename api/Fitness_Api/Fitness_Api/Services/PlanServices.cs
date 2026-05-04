using Fitness_Api.Data;
using Fitness_Api.Interfaces;
using Fitness_Api.Models;
using Fitness_Api.UniversalMethods;
using Microsoft.AspNetCore.Mvc;

namespace Fitness_Api.Services;

public class PlanServices : IPlanServices
{
    private readonly InMemoryStore _context;
    private readonly SessionResolver _sessionResolver;

    public PlanServices(InMemoryStore context, SessionResolver sessionResolver)
    {
        _context = context;
        _sessionResolver = sessionResolver;
    }

    public Task<IActionResult> GetPlans(string token)
    {
        var user = _sessionResolver.GetUser(token);
        if (user is null)
        {
            return Task.FromResult<IActionResult>(new UnauthorizedObjectResult(new { status = false, message = "Сессия не найдена" }));
        }

        var plans = user.Role_Id switch
        {
            RoleIds.Administrator => _context.Plans.OrderByDescending(x => x.Id).ToList(),
            RoleIds.Trainer when user.Trainer_Id.HasValue => _context.Plans.Where(x => x.TrainerId == user.Trainer_Id.Value).OrderByDescending(x => x.Id).ToList(),
            RoleIds.Client when user.Client_Id.HasValue => _context.Plans.Where(x => x.ClientId == user.Client_Id.Value).OrderByDescending(x => x.Id).ToList(),
            _ => new List<PersonalPlan>()
        };

        return Task.FromResult<IActionResult>(new OkObjectResult(plans));
    }

    public Task<IActionResult> CreatePlan(PersonalPlan plan, string token)
    {
        var user = _sessionResolver.GetUser(token);
        if (user is null)
        {
            return Task.FromResult<IActionResult>(new UnauthorizedObjectResult(new { status = false, message = "Сессия не найдена" }));
        }

        plan.Id = _context.NextPlanId();
        if (user.Role_Id == RoleIds.Trainer && user.Trainer_Id.HasValue)
        {
            plan.TrainerId = user.Trainer_Id.Value;
        }
        _context.Plans.Add(plan);

        return Task.FromResult<IActionResult>(new OkObjectResult(plan));
    }

    public Task<IActionResult> UpdateProgress(int id, int progress, string token)
    {
        var user = _sessionResolver.GetUser(token);
        if (user is null)
        {
            return Task.FromResult<IActionResult>(new UnauthorizedObjectResult(new { status = false, message = "Сессия не найдена" }));
        }

        var plan = _context.Plans.FirstOrDefault(x => x.Id == id);
        if (plan is null)
        {
            return Task.FromResult<IActionResult>(new NotFoundObjectResult(new { status = false, message = "План не найден" }));
        }

        if (user.Role_Id == RoleIds.Trainer && user.Trainer_Id != plan.TrainerId)
        {
            return Task.FromResult<IActionResult>(new ObjectResult(new { status = false, message = "Недостаточно прав" }) { StatusCode = 403 });
        }

        plan.ProgressPercent = Math.Clamp(progress, 0, 100);
        return Task.FromResult<IActionResult>(new OkObjectResult(plan));
    }
}
