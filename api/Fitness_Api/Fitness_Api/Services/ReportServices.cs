using Fitness_Api.Data;
using Fitness_Api.Interfaces;
using Fitness_Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fitness_Api.Services;

public class ReportServices : IReportServices
{
    private readonly FitnessDbContext _context;

    public ReportServices(FitnessDbContext context)
    {
        _context = context;
    }

    public Task<IActionResult> GetSummary()
    {
        var today = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified).Date;
        var active = _context.Memberships.Count(x => x.Status == MembershipStatus.Active && x.StartDate.Date <= today && x.EndDate.Date >= today);
        var frozen = _context.Memberships.Count(x => x.Status == MembershipStatus.Frozen && x.EndDate.Date >= today);
        var attendance = _context.Visits.Count();
        var totalCapacity = _context.Workouts.Sum(x => x.Capacity);
        var occupied = _context.Workouts.ToList().Sum(x => x.ClientIds.Count);
        var occupancy = totalCapacity == 0 ? 0 : Math.Round((double)occupied / totalCapacity * 100, 1);
        var revenue = _context.Memberships.ToList().Sum(x => GetMembershipPrice(x.Type));

        return Task.FromResult<IActionResult>(new OkObjectResult(new
        {
            Active = active,
            Frozen = frozen,
            Attendance = attendance,
            Occupancy = occupancy,
            Revenue = revenue,
            PlansInProgress = _context.Plans.Count(x => x.ProgressPercent < 100)
        }));
    }

    private int GetMembershipPrice(string type)
    {
        return type switch
        {
            "Разовое посещение" => 500,
            "Дневной" => 1800,
            "Стандарт" => 2500,
            "Премиум" => 3500,
            "Безлимит" => 6500,
            _ => 2500
        };
    }
}
