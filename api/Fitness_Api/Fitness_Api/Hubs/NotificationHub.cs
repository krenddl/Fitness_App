using Fitness_Api.UniversalMethods;
using Microsoft.AspNetCore.SignalR;

namespace Fitness_Api.Hubs;

public class NotificationHub : Hub
{
    private readonly SessionResolver _sessionResolver;

    public NotificationHub(SessionResolver sessionResolver)
    {
        _sessionResolver = sessionResolver;
    }

    public override async Task OnConnectedAsync()
    {
        var token = Context.GetHttpContext()?.Request.Query["access_token"].FirstOrDefault();
        var session = _sessionResolver.GetSession(token);

        if (session is null)
        {
            Context.Abort();
            return;
        }

        await base.OnConnectedAsync();
    }
}
