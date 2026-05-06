using Fitness_Api.Data;
using Fitness_Api.Interfaces;
using Fitness_Api.Models;
using Fitness_Api.Requests;
using Fitness_Api.UniversalMethods;
using Microsoft.AspNetCore.Mvc;

namespace Fitness_Api.Services;

public class VisitServices : IVisitServices
{
    private readonly FitnessDbContext _context;
    private readonly SessionResolver _sessionResolver;

    public VisitServices(FitnessDbContext context, SessionResolver sessionResolver)
    {
        _context = context;
        _sessionResolver = sessionResolver;
    }

    public Task<IActionResult> GetVisits(string token)
    {
        var user = _sessionResolver.GetUser(token);
        if (user is null)
        {
            return Task.FromResult<IActionResult>(new UnauthorizedObjectResult(new { status = false, message = "Сессия не найдена" }));
        }

        var visits = user.Role_Id switch
        {
            RoleIds.Administrator or RoleIds.SalesManager => _context.Visits.OrderByDescending(x => x.EnteredAt).ToList(),
            RoleIds.Client when user.Client_Id.HasValue => _context.Visits.Where(x => x.ClientId == user.Client_Id.Value).OrderByDescending(x => x.EnteredAt).ToList(),
            _ => new List<Visit>()
        };

        return Task.FromResult<IActionResult>(new OkObjectResult(visits));
    }

    public Task<IActionResult> Enter(EnterVisitRequest request, string token)
    {
        var user = _sessionResolver.GetUser(token);
        if (user is null)
        {
            return Task.FromResult<IActionResult>(new UnauthorizedObjectResult(new { status = false, message = "Сессия не найдена" }));
        }

        var clientId = user.Role_Id == RoleIds.Client ? user.Client_Id ?? 0 : request.ClientId;
        var now = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);

        var activeVisit = _context.Visits.FirstOrDefault(x => x.ClientId == clientId && x.ExitedAt == null);
        if (activeVisit is not null)
        {
            return Task.FromResult<IActionResult>(new BadRequestObjectResult(new
            {
                status = false,
                message = "Вход уже отмечен. Сначала отметьте выход."
            }));
        }

        var hasMembership = _context.Memberships.Any(x =>
            x.ClientId == clientId &&
            x.Status == MembershipStatus.Active &&
            x.EndDate >= now);

        if (!hasMembership)
        {
            return Task.FromResult<IActionResult>(new BadRequestObjectResult(new { status = false, message = "Нет активного абонемента" }));
        }

        var visit = new Visit
        {
            ClientId = clientId,
            EnteredAt = now,
            AccessType = request.AccessType
        };

        _context.Visits.Add(visit);
        _context.SaveChanges();
        return Task.FromResult<IActionResult>(new OkObjectResult(visit));
    }

    public Task<IActionResult> Exit(int id, string token)
    {
        var user = _sessionResolver.GetUser(token);
        if (user is null)
        {
            return Task.FromResult<IActionResult>(new UnauthorizedObjectResult(new { status = false, message = "Сессия не найдена" }));
        }

        var visit = _context.Visits.FirstOrDefault(x => x.Id == id);
        if (visit is null)
        {
            return Task.FromResult<IActionResult>(new NotFoundObjectResult(new { status = false, message = "Посещение не найдено" }));
        }

        if (user.Role_Id == RoleIds.Client && user.Client_Id != visit.ClientId)
        {
            return Task.FromResult<IActionResult>(new ObjectResult(new { status = false, message = "Недостаточно прав" }) { StatusCode = 403 });
        }

        visit.ExitedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
        _context.SaveChanges();
        return Task.FromResult<IActionResult>(new OkObjectResult(visit));
    }
}
