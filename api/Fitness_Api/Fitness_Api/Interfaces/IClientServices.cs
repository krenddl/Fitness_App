using Microsoft.AspNetCore.Mvc;
using Fitness_Api.Models;

namespace Fitness_Api.Interfaces;

public interface IClientServices
{
    Task<IActionResult> GetAllClients();
    Task<IActionResult> GetMyClient(string token);
    Task<IActionResult> CreateClient(Client client);
}
