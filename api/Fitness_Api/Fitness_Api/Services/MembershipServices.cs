using Fitness_Api.Data;
using Fitness_Api.Interfaces;
using Fitness_Api.Models;
using Fitness_Api.UniversalMethods;
using Microsoft.AspNetCore.Mvc;

namespace Fitness_Api.Services;

public class MembershipServices : IMembershipServices
{
    private readonly FitnessDbContext _context;
    private readonly SessionResolver _sessionResolver;

    public MembershipServices(FitnessDbContext context, SessionResolver sessionResolver)
    {
        _context = context;
        _sessionResolver = sessionResolver;
    }

    public Task<IActionResult> GetMemberships(string token)
    {
        var user = _sessionResolver.GetUser(token);
        if (user is null)
        {
            return Task.FromResult<IActionResult>(new UnauthorizedObjectResult(new { status = false, message = "Сессия не найдена" }));
        }

        ExpireOldMemberships();

        var memberships = user.Role_Id switch
        {
            RoleIds.Administrator or RoleIds.SalesManager => _context.Memberships.OrderByDescending(x => x.EndDate).ToList(),
            RoleIds.Client when user.Client_Id.HasValue => _context.Memberships.Where(x => x.ClientId == user.Client_Id.Value).OrderByDescending(x => x.EndDate).ToList(),
            _ => new List<Membership>()
        };

        return Task.FromResult<IActionResult>(new OkObjectResult(memberships));
    }

    public Task<IActionResult> CreateMembership(Membership membership)
    {
        if (!_context.Clients.Any(x => x.Id == membership.ClientId))
        {
            return Task.FromResult<IActionResult>(new BadRequestObjectResult(new
            {
                status = false,
                message = "Выберите существующего клиента"
            }));
        }

        if (string.IsNullOrWhiteSpace(membership.Type))
        {
            return Task.FromResult<IActionResult>(new BadRequestObjectResult(new
            {
                status = false,
                message = "Выберите тип абонемента"
            }));
        }

        membership.Type = NormalizeMembershipType(membership.Type);
        var days = GetMembershipDays(membership.Type);
        if (days == 0)
        {
            return Task.FromResult<IActionResult>(new BadRequestObjectResult(new
            {
                status = false,
                message = "Выберите существующий тариф"
            }));
        }

        membership.StartDate = DateTime.SpecifyKind(membership.StartDate.Date, DateTimeKind.Unspecified);
        membership.EndDate = DateTime.SpecifyKind(membership.StartDate.AddDays(days - 1), DateTimeKind.Unspecified);

        var overlap = FindOverlap(membership.ClientId, membership.StartDate, membership.EndDate);

        if (overlap is not null)
        {
            return Task.FromResult<IActionResult>(new BadRequestObjectResult(new
            {
                status = false,
                message = $"У клиента уже есть абонемент на этот период: {overlap.Type}, до {overlap.EndDate:dd.MM.yyyy}"
            }));
        }

        var now = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
        membership.Status = membership.EndDate.Date < now.Date ? MembershipStatus.Expired : MembershipStatus.Active;
        _context.Memberships.Add(membership);
        _context.SaveChanges();

        return Task.FromResult<IActionResult>(new OkObjectResult(membership));
    }

    public Task<IActionResult> FreezeMembership(int id)
    {
        var membership = _context.Memberships.FirstOrDefault(x => x.Id == id);
        if (membership is null)
        {
            return Task.FromResult<IActionResult>(new NotFoundObjectResult(new { status = false, message = "Абонемент не найден" }));
        }

        membership.Status = MembershipStatus.Frozen;
        _context.SaveChanges();
        return Task.FromResult<IActionResult>(new OkObjectResult(membership));
    }

    public Task<IActionResult> UnfreezeMembership(int id)
    {
        var membership = _context.Memberships.FirstOrDefault(x => x.Id == id);
        if (membership is null)
        {
            return Task.FromResult<IActionResult>(new NotFoundObjectResult(new { status = false, message = "Абонемент не найден" }));
        }

        membership.Status = membership.EndDate.Date < DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified).Date
            ? MembershipStatus.Expired
            : MembershipStatus.Active;

        _context.SaveChanges();
        return Task.FromResult<IActionResult>(new OkObjectResult(membership));
    }

    public Task<IActionResult> ExtendMembership(int id, int days)
    {
        var membership = _context.Memberships.FirstOrDefault(x => x.Id == id);
        if (membership is null)
        {
            return Task.FromResult<IActionResult>(new NotFoundObjectResult(new { status = false, message = "Абонемент не найден" }));
        }

        if (days <= 0)
        {
            return Task.FromResult<IActionResult>(new BadRequestObjectResult(new { status = false, message = "Количество дней должно быть больше нуля" }));
        }

        var newEndDate = DateTime.SpecifyKind(membership.EndDate.AddDays(days).Date, DateTimeKind.Unspecified);
        var overlap = FindOverlap(membership.ClientId, membership.StartDate, newEndDate, membership.Id);
        if (overlap is not null)
        {
            return Task.FromResult<IActionResult>(new BadRequestObjectResult(new
            {
                status = false,
                message = $"Продление пересекается с другим абонементом: {overlap.Type}, до {overlap.EndDate:dd.MM.yyyy}"
            }));
        }

        membership.EndDate = newEndDate;
        if (membership.Status != MembershipStatus.Frozen)
        {
            membership.Status = membership.EndDate.Date < DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified).Date
                ? MembershipStatus.Expired
                : MembershipStatus.Active;
        }

        _context.SaveChanges();
        return Task.FromResult<IActionResult>(new OkObjectResult(membership));
    }

    public Task<IActionResult> GetReminders()
    {
        ExpireOldMemberships();
        var now = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
        var due = _context.Memberships
            .Where(x => x.Status == MembershipStatus.Active
                && x.EndDate.Date >= now.Date
                && x.EndDate.Date <= now.AddDays(3).Date)
            .ToList();

        var result = due.Select(x => new { x.Id, x.ClientId, x.EndDate }).ToList();
        return Task.FromResult<IActionResult>(new OkObjectResult(result));
    }

    private string NormalizeMembershipType(string type)
    {
        type = type.Trim();

        return type switch
        {
            "Разовый" => "Разовое посещение",
            "Разовое" => "Разовое посещение",
            _ => type
        };
    }

    private int GetMembershipDays(string type)
    {
        return type switch
        {
            "Разовое посещение" => 1,
            "Дневной" => 30,
            "Стандарт" => 30,
            "Премиум" => 30,
            "Безлимит" => 90,
            _ => 0
        };
    }

    private Membership? FindOverlap(int clientId, DateTime startDate, DateTime endDate, int? ignoreId = null)
    {
        return _context.Memberships.FirstOrDefault(x =>
            x.ClientId == clientId
            && x.Id != ignoreId
            && x.Status != MembershipStatus.Expired
            && x.StartDate.Date <= endDate.Date
            && startDate.Date <= x.EndDate.Date);
    }

    private void ExpireOldMemberships()
    {
        var today = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified).Date;
        var oldMemberships = _context.Memberships
            .Where(x => x.Status == MembershipStatus.Active && x.EndDate.Date < today)
            .ToList();

        if (!oldMemberships.Any())
        {
            return;
        }

        foreach (var membership in oldMemberships)
        {
            membership.Status = MembershipStatus.Expired;
        }

        _context.SaveChanges();
    }
}
