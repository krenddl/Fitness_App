using Fitness_Api.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Fitness_Api.Interfaces;

public interface IVisitServices
{
    Task<IActionResult> GetVisits(string token);
    Task<IActionResult> Enter(EnterVisitRequest request, string token);
    Task<IActionResult> Exit(int id, string token);
}
