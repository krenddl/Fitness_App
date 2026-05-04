using Fitness_Api.Data;
using Fitness_Api.Interfaces;
using Fitness_Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fitness_Api.Services;

public class ReportServices : IReportServices
{
    private readonly InMemoryStore _context;

    public ReportServices(InMemoryStore context)
    {
        _context = context;
    }

    public Task<IActionResult> GetSummary()
    {
        var active = _context.Memberships.Count(x => x.Status == MembershipStatus.Active);
        var frozen = _context.Memberships.Count(x => x.Status == MembershipStatus.Frozen);
        var attendance = _context.Visits.Count;
        var totalCapacity = _context.Workouts.Sum(x => x.Capacity);
        var occupied = _context.Workouts.Sum(x => x.ClientIds.Count);
        var occupancy = totalCapacity == 0 ? 0 : Math.Round((double)occupied / totalCapacity * 100, 1);
        var revenue = _context.Memberships.Count * 2500;

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
}
