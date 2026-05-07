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
    private readonly IHubContext<ChatHub> _chatHub;

    public CommunicationServices(
        FitnessDbContext context,
        SessionResolver sessionResolver,
        IHubContext<ChatHub> chatHub)
    {
        _context = context;
        _sessionResolver = sessionResolver;
        _chatHub = chatHub;
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
                trainerId = _context.Plans.FirstOrDefault(x => x.ClientId == clientId)?.TrainerId
                    ?? _context.Trainers.OrderBy(x => x.Id).FirstOrDefault()?.Id
                    ?? 0;
            }
        }

        if (trainerId == 0 || clientId == 0)
        {
            return new BadRequestObjectResult(new { status = false, message = "Невозможно определить участников чата" });
        }

        if (!_context.Trainers.Any(x => x.Id == trainerId) || !_context.Clients.Any(x => x.Id == clientId))
        {
            return new BadRequestObjectResult(new { status = false, message = "Клиент или тренер не найден" });
        }

        if (string.IsNullOrWhiteSpace(request.Text))
        {
            return new BadRequestObjectResult(new { status = false, message = "Введите сообщение" });
        }

        var message = new ChatMessage
        {
            TrainerId = trainerId,
            ClientId = clientId,
            SenderRole = senderRole,
            Text = request.Text.Trim(),
            SentAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified)
        };

        _context.ChatMessages.Add(message);
        _context.SaveChanges();
        await _chatHub.Clients.All.SendAsync("ReceiveChatMessage", message);

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
        if (!_context.Clients.Any(x => x.Id == request.ClientId))
        {
            return new BadRequestObjectResult(new { status = false, message = "Клиент не найден" });
        }

        if (string.IsNullOrWhiteSpace(request.Text))
        {
            return new BadRequestObjectResult(new { status = false, message = "Введите текст уведомления" });
        }

        var push = new PushNotification
        {
            ClientId = request.ClientId,
            Text = request.Text.Trim(),
            SentAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified)
        };

        _context.Notifications.Add(push);
        _context.SaveChanges();
        await _chatHub.Clients.All.SendAsync("PushNotification", push);

        return new OkObjectResult(push);
    }
}
