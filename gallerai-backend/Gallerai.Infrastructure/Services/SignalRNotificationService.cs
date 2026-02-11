using Gallerai.Application.Interfaces;
using Gallerai.Infrastructure.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace Gallerai.Infrastructure.Services;

internal sealed class SignalRNotificationService(IHubContext<ImageNotificationsHub> hubContext) : INotificationService
{
    public async Task NotifyUserUpdate<T>(string userId, T message)
    {
        await hubContext.Clients.All.SendAsync("ReceiveImageNotification", message);
    }
}
