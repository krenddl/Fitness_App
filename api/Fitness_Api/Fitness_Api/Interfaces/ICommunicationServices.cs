using Fitness_Api.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Fitness_Api.Interfaces;

public interface ICommunicationServices
{
    Task<IActionResult> GetChat(string token);
    Task<IActionResult> SendChat(SendChatRequest request, string token);
    Task<IActionResult> GetNotifications(string token);
    Task<IActionResult> SendPush(SendPushRequest request);
}
