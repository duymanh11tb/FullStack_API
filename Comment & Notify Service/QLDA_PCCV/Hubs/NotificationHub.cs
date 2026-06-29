using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace QLDA_PCCV.Hubs;

public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        // Client specifies their UserId when connecting (or it is read from claims)
        var httpContext = Context.GetHttpContext();
        var userIdQuery = httpContext?.Request.Query["userId"].ToString();
        
        if (!string.IsNullOrEmpty(userIdQuery))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userIdQuery.ToLower());
        }
        
        // Also map standard NameIdentifier claim
        var userClaimId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userClaimId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userClaimId.ToLower());
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var httpContext = Context.GetHttpContext();
        var userIdQuery = httpContext?.Request.Query["userId"].ToString();
        
        if (!string.IsNullOrEmpty(userIdQuery))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userIdQuery.ToLower());
        }
        
        var userClaimId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userClaimId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userClaimId.ToLower());
        }

        await base.OnDisconnectedAsync(exception);
    }
}
