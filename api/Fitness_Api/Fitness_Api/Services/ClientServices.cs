using Fitness_Api.Data;
using Fitness_Api.Interfaces;
using Fitness_Api.UniversalMethods;
using Microsoft.AspNetCore.Mvc;

namespace Fitness_Api.Services;

public class ClientServices : IClientServices
{
    private readonly FitnessDbContext _context;
    private readonly SessionResolver _sessionResolver;

    public ClientServices(FitnessDbContext context, SessionResolver sessionResolver)
    {
        _context = context;
        _sessionResolver = sessionResolver;
    }

    public Task<IActionResult> GetAllClients()
    {
        return Task.FromResult<IActionResult>(new OkObjectResult(_context.Clients.OrderBy(x => x.FullName).ToList()));
    }

    public Task<IActionResult> GetMyClient(string token)
    {
        var clientId = _sessionResolver.GetClientId(token);
        var client = clientId.HasValue ? _context.Clients.FirstOrDefault(x => x.Id == clientId.Value) : null;

        if (client is null)
        {
            return Task.FromResult<IActionResult>(new NotFoundObjectResult(new
            {
                status = false,
                message = "Клиент не найден"
            }));
        }

        return Task.FromResult<IActionResult>(new OkObjectResult(client));
    }
}
