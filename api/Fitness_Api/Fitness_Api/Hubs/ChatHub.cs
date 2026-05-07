using Fitness_Api.Data;
using Fitness_Api.Models;
using Fitness_Api.Requests;
using Fitness_Api.UniversalMethods;
using Microsoft.AspNetCore.SignalR;

namespace Fitness_Api.Hubs;

public class ChatHub : Hub
{
    private readonly FitnessDbContext _context;
    private readonly SessionResolver _sessionResolver;

    public ChatHub(FitnessDbContext context, SessionResolver sessionResolver)
    {
        _context = context;
        _sessionResolver = sessionResolver;
    }

    public override async Task OnConnectedAsync()
    {
        var user = GetCurrentUser();
        if (user is null)
        {
            Context.Abort();
            return;
        }

        if (user.Role_Id == RoleIds.Administrator)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "admins");
        }

        if (user.Trainer_Id.HasValue)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, TrainerGroup(user.Trainer_Id.Value));
        }

        if (user.Client_Id.HasValue)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, ClientGroup(user.Client_Id.Value));
        }

        await Clients.Caller.SendAsync("ChatConnected", new
        {
            user.Name,
            user.Role_Id
        });

        await base.OnConnectedAsync();
    }

    public async Task SendChatMessage(SendChatRequest request)
    {
        var user = GetCurrentUser();
        if (user is null)
        {
            throw new HubException("Сессия не найдена.");
        }

        if (string.IsNullOrWhiteSpace(request.Text))
        {
            throw new HubException("Введите сообщение.");
        }

        var trainerId = request.TrainerId;
        var clientId = request.ClientId;
        var senderRole = "User";

        if (user.Role_Id == RoleIds.Trainer)
        {
            trainerId = user.Trainer_Id ?? 0;
            senderRole = "Trainer";
        }

        if (user.Role_Id == RoleIds.Client)
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
            throw new HubException("Не удалось определить клиента или тренера.");
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
        await _context.SaveChangesAsync();

        await Clients.Group(TrainerGroup(trainerId)).SendAsync("ReceiveChatMessage", message);
        await Clients.Group(ClientGroup(clientId)).SendAsync("ReceiveChatMessage", message);
        await Clients.Group("admins").SendAsync("ReceiveChatMessage", message);
    }

    private User? GetCurrentUser()
    {
        var httpContext = Context.GetHttpContext();
        var token = httpContext?.Request.Query["access_token"].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(token))
        {
            token = httpContext?.Request.Headers["Authorization"].FirstOrDefault();
        }

        return _sessionResolver.GetUser(token);
    }

    private static string TrainerGroup(int trainerId)
    {
        return $"trainer-{trainerId}";
    }

    private static string ClientGroup(int clientId)
    {
        return $"client-{clientId}";
    }
}
