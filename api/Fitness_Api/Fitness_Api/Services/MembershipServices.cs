using Fitness_Api.Data;
using Fitness_Api.Interfaces;
using Fitness_Api.Models;
using Fitness_Api.UniversalMethods;
using Microsoft.AspNetCore.Mvc;

namespace Fitness_Api.Services;

public class MembershipServices : IMembershipServices
{
    private readonly InMemoryStore _context;
    private readonly SessionResolver _sessionResolver;

    public MembershipServices(InMemoryStore context, SessionResolver sessionResolver)
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
        membership.Id = _context.NextMembershipId();
        membership.Status = membership.EndDate < DateTime.UtcNow ? MembershipStatus.Expired : MembershipStatus.Active;
        _context.Memberships.Add(membership);

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
        return Task.FromResult<IActionResult>(new OkObjectResult(membership));
    }

    public Task<IActionResult> ExtendMembership(int id, int days)
    {
        var membership = _context.Memberships.FirstOrDefault(x => x.Id == id);
        if (membership is null)
        {
            return Task.FromResult<IActionResult>(new NotFoundObjectResult(new { status = false, message = "Абонемент не найден" }));
        }

        membership.EndDate = membership.EndDate.AddDays(days);
        membership.Status = MembershipStatus.Active;
        return Task.FromResult<IActionResult>(new OkObjectResult(membership));
    }

    public Task<IActionResult> GetReminders()
    {
        var due = _context.Memberships
            .Where(x => !x.ReminderSent && x.EndDate <= DateTime.UtcNow.AddDays(3))
            .Select(x =>
            {
                x.ReminderSent = true;
                return new { x.Id, x.ClientId, x.EndDate };
            })
            .ToList();

        return Task.FromResult<IActionResult>(new OkObjectResult(due));
    }
}
