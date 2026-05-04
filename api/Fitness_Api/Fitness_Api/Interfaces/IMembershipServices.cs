using Fitness_Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fitness_Api.Interfaces;

public interface IMembershipServices
{
    Task<IActionResult> GetMemberships(string token);
    Task<IActionResult> CreateMembership(Membership membership);
    Task<IActionResult> FreezeMembership(int id);
    Task<IActionResult> ExtendMembership(int id, int days);
    Task<IActionResult> GetReminders();
}
