using Fitness_Api.Data;
using Fitness_Api.Hubs;
using Fitness_Api.Interfaces;
using Fitness_Api.Models;
using Fitness_Api.Requests;
using Fitness_Api.UniversalMethods;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Fitness_Api.Services;

public class CommunicationServices : ICommunicationServices
{
    private readonly FitnessDbContext _context;
    private readonly SessionResolver _sessionResolver;
    private readonly IHubContext<NotificationHub> _hub;

    public CommunicationServices(FitnessDbContext context, SessionResolver sessionResolver, IHubContext<NotificationHub> hub)
    {
        _context = context;
        _sessionResolver = sessionResolver;
        _hub = hub;
    }

    public Task<IActionResult> GetChat(string token)
    {
        var user = _sessionResolver.GetUser(token);
        if (user is null)
        {
            return Task.FromResult<IActionResult>(new UnauthorizedObjectResult(new { status = false, message = "Сессия не найдена" }));
        }

        var chat = user.Role_Id switch
        {
            RoleIds.Administrator => _context.ChatMessages.OrderBy(x => x.SentAt).ToList(),
            RoleIds.Trainer when user.Trainer_Id.HasValue => _context.ChatMessages.Where(x => x.TrainerId == user.Trainer_Id.Value).OrderBy(x => x.SentAt).ToList(),
            RoleIds.Client when user.Client_Id.HasValue => _context.ChatMessages.Where(x => x.ClientId == user.Client_Id.Value).OrderBy(x => x.SentAt).ToList(),
            _ => new List<ChatMessage>()
        };

        return Task.FromResult<IActionResult>(new OkObjectResult(chat));
    }

    public async Task<IActionResult> SendChat(SendChatRequest request, string token)
    {
        var user = _sessionResolver.GetUser(token);
        if (user is null)
        {
            return new UnauthorizedObjectResult(new { status = false, message = "Сессия не найдена" });
        }

        var trainerId = request.TrainerId;
        var clientId = request.ClientId;
        var senderRole = "User";

        if (user.Role_Id == RoleIds.Trainer)
        {
            trainerId = user.Trainer_Id ?? 0;
            senderRole = "Trainer";
        }
        else if (user.Role_Id == RoleIds.Client)
        {
            clientId = user.Client_Id ?? 0;
            senderRole = "Client";
            if (trainerId == 0)
            {
                trainerId = _context.Plans.FirstOrDefault(x => x.ClientId == clientId)?.TrainerId ?? 0;
            }
        }

        if (trainerId == 0 || clientId == 0)
        {
            return new BadRequestObjectResult(new { status = false, message = "Невозможно определить участников чата" });
        }

        var message = new ChatMessage
        {
            TrainerId = trainerId,
            ClientId = clientId,
            SenderRole = senderRole,
            Text = request.Text,
            SentAt = DateTime.UtcNow
        };

        _context.ChatMessages.Add(message);
        _context.SaveChanges();
        await _hub.Clients.All.SendAsync("ChatMessage", message);

        return new OkObjectResult(message);
    }

    public Task<IActionResult> GetNotifications(string token)
    {
        var user = _sessionResolver.GetUser(token);
        if (user is null)
        {
            return Task.FromResult<IActionResult>(new UnauthorizedObjectResult(new { status = false, message = "Сессия не найдена" }));
        }

        var notifications = user.Role_Id == RoleIds.Client && user.Client_Id.HasValue
            ? _context.Notifications.Where(x => x.ClientId == user.Client_Id.Value).OrderByDescending(x => x.SentAt).ToList()
            : _context.Notifications.OrderByDescending(x => x.SentAt).ToList();

        return Task.FromResult<IActionResult>(new OkObjectResult(notifications));
    }

    public async Task<IActionResult> SendPush(SendPushRequest request)
    {
        var push = new PushNotification
        {
            ClientId = request.ClientId,
            Text = request.Text,
            SentAt = DateTime.UtcNow
        };

        _context.Notifications.Add(push);
        _context.SaveChanges();
        await _hub.Clients.All.SendAsync("PushNotification", push);

        return new OkObjectResult(push);
    }
}
