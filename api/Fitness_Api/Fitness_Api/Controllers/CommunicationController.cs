using Fitness_Api.CustomAtributes;
using Fitness_Api.Interfaces;
using Fitness_Api.Requests;
using Fitness_Api.UniversalMethods;
using Microsoft.AspNetCore.Mvc;

namespace Fitness_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommunicationController : ControllerBase
{
    private readonly ICommunicationServices _communicationServices;

    public CommunicationController(ICommunicationServices communicationServices)
    {
        _communicationServices = communicationServices;
    }

    [HttpGet("chat")]
    [RoleAuthorize([RoleIds.Administrator, RoleIds.Client, RoleIds.Trainer])]
    public async Task<IActionResult> Chat()
    {
        var token = Request.Headers["Authorization"].ToString();
        return await _communicationServices.GetChat(token);
    }

    [HttpPost("chat")]
    [RoleAuthorize([RoleIds.Client, RoleIds.Trainer])]
    public async Task<IActionResult> SendMessage([FromBody] SendChatRequest request)
    {
        var token = Request.Headers["Authorization"].ToString();
        return await _communicationServices.SendChat(request, token);
    }

    [HttpGet("notifications")]
    [RoleAuthorize([RoleIds.Administrator, RoleIds.Client, RoleIds.Trainer, RoleIds.SalesManager])]
    public async Task<IActionResult> Notifications()
    {
        var token = Request.Headers["Authorization"].ToString();
        return await _communicationServices.GetNotifications(token);
    }

    [HttpPost("notifications")]
    [RoleAuthorize([RoleIds.Administrator, RoleIds.Trainer, RoleIds.SalesManager])]
    public async Task<IActionResult> Push([FromBody] SendPushRequest request)
    {
        return await _communicationServices.SendPush(request);
    }
}
