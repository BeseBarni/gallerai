namespace Gallerai.Workers.InferenceWorker.Services;

using Gallerai.SignalR.Shared.Hubs;
using Microsoft.AspNetCore.SignalR;

public class RedisPublisher(IHubContext<ImageNotificationsHub> hubContext)
{
    public async Task PublishMessageAsync<T>(string key, T message)
    {
        await hubContext.Clients.All.SendAsync(key, message);
    }
}
