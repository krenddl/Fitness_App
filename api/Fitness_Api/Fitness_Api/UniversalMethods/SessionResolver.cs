using Fitness_Api.Data;
using Fitness_Api.Models;

namespace Fitness_Api.UniversalMethods;

public class SessionResolver
{
    private readonly FitnessDbContext _context;

    public SessionResolver(FitnessDbContext context)
    {
        _context = context;
    }

    public string NormalizeToken(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return string.Empty;
        }

        token = token.Trim();

        return token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            ? token["Bearer ".Length..].Trim()
            : token;
    }

    public Session? GetSession(string? token)
    {
        var normalized = NormalizeToken(token);
        return _context.Sessions.FirstOrDefault(x => x.Token == normalized);
    }

    public User? GetUser(string? token)
    {
        var session = GetSession(token);
        return session is null
            ? null
            : _context.Users.FirstOrDefault(x => x.id_User == session.User_Id);
    }

    public int? GetClientId(string? token)
    {
        return GetUser(token)?.Client_Id;
    }

    public int? GetTrainerId(string? token)
    {
        return GetUser(token)?.Trainer_Id;
    }

    public string GetRoleName(int roleId)
    {
        return _context.Roles.FirstOrDefault(x => x.id_Role == roleId)?.Name ?? string.Empty;
    }
}
