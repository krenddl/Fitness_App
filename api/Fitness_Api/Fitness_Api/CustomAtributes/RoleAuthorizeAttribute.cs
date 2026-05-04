using Fitness_Api.UniversalMethods;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Fitness_Api.CustomAtributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RoleAuthorizeAttribute : Attribute, IAsyncActionFilter
{
    private readonly int[] _roleIds;

    public RoleAuthorizeAttribute(int[] roleIds)
    {
        _roleIds = roleIds;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resolver = context.HttpContext.RequestServices.GetRequiredService<SessionResolver>();
        var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
        var user = resolver.GetUser(token);

        if (user is null)
        {
            context.Result = new JsonResult(new { status = false, message = "Сессия не найдена" }) { StatusCode = 401 };
            return;
        }

        if (!_roleIds.Contains(user.Role_Id))
        {
            context.Result = new JsonResult(new { status = false, message = "Недостаточно прав" }) { StatusCode = 403 };
            return;
        }

        await next();
    }
}
