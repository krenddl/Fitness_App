using System.Security.Claims;
using Fitness_Client.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace Fitness_Client.Services;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly SessionStorageService _storage;

    public CustomAuthStateProvider(SessionStorageService storage)
    {
        _storage = storage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var session = await _storage.GetSessionAsync();
        return BuildState(session);
    }

    public void NotifyUserAuthentication(StoredSessionModel session)
    {
        NotifyAuthenticationStateChanged(Task.FromResult(BuildState(session)));
    }

    public void NotifyUserLogout()
    {
        NotifyAuthenticationStateChanged(Task.FromResult(BuildState(null)));
    }

    private AuthenticationState BuildState(StoredSessionModel? session)
    {
        if (session?.User is null || string.IsNullOrWhiteSpace(session.Token))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, session.User.Name),
            new(ClaimTypes.Email, session.User.Email),
            new(ClaimTypes.Role, session.User.Role),
            new("roleId", session.User.Role_Id.ToString()),
            new("userId", session.User.Id_User.ToString())
        };

        if (session.User.Client_Id.HasValue)
        {
            claims.Add(new Claim("clientId", session.User.Client_Id.Value.ToString()));
        }

        if (session.User.Trainer_Id.HasValue)
        {
            claims.Add(new Claim("trainerId", session.User.Trainer_Id.Value.ToString()));
        }

        var identity = new ClaimsIdentity(claims, "jwt");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }
}
