using Fitness_Api.Data;
using Fitness_Api.Hubs;
using Fitness_Api.Interfaces;
using Fitness_Api.Models;
using Fitness_Api.Requests;
using Fitness_Api.UniversalMethods;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Fitness_Api.Services;

public class WorkoutServices : IWorkoutServices
{
    private readonly FitnessDbContext _context;
    private readonly SessionResolver _sessionResolver;
    private readonly IHubContext<ChatHub> _hub;

    public WorkoutServices(FitnessDbContext context, SessionResolver sessionResolver, IHubContext<ChatHub> hub)
    {
        _context = context;
        _sessionResolver = sessionResolver;
        _hub = hub;
    }

    public Task<IActionResult> GetWorkouts(string token)
    {
        var user = _sessionResolver.GetUser(token);
        if (user is null)
        {
            return Task.FromResult<IActionResult>(new UnauthorizedObjectResult(new { status = false, message = "Сессия не найдена" }));
        }

        var workouts = user.Role_Id switch
        {
            RoleIds.Trainer when user.Trainer_Id.HasValue => _context.Workouts.Where(x => x.TrainerId == user.Trainer_Id.Value).OrderBy(x => x.StartAt).ToList(),
            _ => _context.Workouts.OrderBy(x => x.StartAt).ToList()
        };

        return Task.FromResult<IActionResult>(new OkObjectResult(workouts));
    }

    public Task<IActionResult> CreateWorkout(WorkoutSession workout, string token)
    {
        var user = _sessionResolver.GetUser(token);
        if (user is null)
        {
            return Task.FromResult<IActionResult>(new UnauthorizedObjectResult(new { status = false, message = "Сессия не найдена" }));
        }

        if (user.Role_Id == RoleIds.Trainer && user.Trainer_Id.HasValue)
        {
            workout.TrainerId = user.Trainer_Id.Value;
        }

        if (string.IsNullOrWhiteSpace(workout.Title))
        {
            return Task.FromResult<IActionResult>(new BadRequestObjectResult(new { status = false, message = "Введите название занятия" }));
        }

        if (!_context.Trainers.Any(x => x.Id == workout.TrainerId))
        {
            return Task.FromResult<IActionResult>(new BadRequestObjectResult(new { status = false, message = "Выберите тренера" }));
        }

        if (workout.Capacity <= 0)
        {
            return Task.FromResult<IActionResult>(new BadRequestObjectResult(new { status = false, message = "Количество мест должно быть больше нуля" }));
        }

        var now = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
        workout.StartAt = DateTime.SpecifyKind(workout.StartAt, DateTimeKind.Unspecified);
        if (workout.StartAt < now.AddMinutes(-5))
        {
            return Task.FromResult<IActionResult>(new BadRequestObjectResult(new { status = false, message = "Дата занятия уже прошла" }));
        }

        workout.Title = workout.Title.Trim();
        workout.ClientIds ??= new List<int>();
        _context.Workouts.Add(workout);
        _context.SaveChanges();

        return Task.FromResult<IActionResult>(new OkObjectResult(workout));
    }

    public async Task<IActionResult> Enroll(int id, EnrollWorkout request, string token)
    {
        var user = _sessionResolver.GetUser(token);
        if (user is null)
        {
            return new UnauthorizedObjectResult(new { status = false, message = "Сессия не найдена" });
        }

        var clientId = user.Role_Id == RoleIds.Client ? user.Client_Id ?? 0 : request.ClientId;
        var workout = _context.Workouts.FirstOrDefault(x => x.Id == id);

        if (workout is null)
        {
            return new NotFoundObjectResult(new { status = false, message = "Занятие не найдено" });
        }

        var now = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
        if (!_context.Clients.Any(x => x.Id == clientId))
        {
            return new BadRequestObjectResult(new { status = false, message = "Клиент не найден" });
        }

        if (workout.StartAt < now)
        {
            return new BadRequestObjectResult(new { status = false, message = "Занятие уже прошло" });
        }

        var hasMembership = _context.Memberships.Any(x =>
            x.ClientId == clientId &&
            x.Status == MembershipStatus.Active &&
            x.StartDate.Date <= now.Date &&
            x.EndDate.Date >= now.Date);

        if (!hasMembership)
        {
            return new BadRequestObjectResult(new { status = false, message = "Нет активного абонемента" });
        }

        workout.ClientIds ??= new List<int>();
        if (workout.ClientIds.Contains(clientId))
        {
            return new OkObjectResult(workout);
        }

        if (workout.ClientIds.Count >= workout.Capacity)
        {
            return new BadRequestObjectResult(new { status = false, message = "Лимит мест заполнен" });
        }

        workout.ClientIds.Add(clientId);
        _context.SaveChanges();
        await _hub.Clients.All.SendAsync("WorkoutChanged", new { WorkoutId = workout.Id, Occupied = workout.ClientIds.Count, workout.Capacity });

        return new OkObjectResult(workout);
    }
}
