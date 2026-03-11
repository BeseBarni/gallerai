using Gallerai.Application.Interfaces;
using Gallerai.SignalR.Shared.Consts;
using Gallerai.SignalR.Shared.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Gallerai.Infrastructure.Services;

internal sealed class SignalRNotificationService(IHubContext<ImageNotificationsHub> hubContext) : INotificationService
{
    public async ValueTask NotifyUserUpdate<T>(string userId, T message)
    {
        await hubContext.Clients.User(userId).SendAsync(MessageChannelConsts.ImageUpdate, message);
    }
}
