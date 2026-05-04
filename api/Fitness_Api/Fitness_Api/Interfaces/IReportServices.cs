using Microsoft.AspNetCore.Mvc;

namespace Fitness_Api.Interfaces;

public interface IReportServices
{
    Task<IActionResult> GetSummary();
}
