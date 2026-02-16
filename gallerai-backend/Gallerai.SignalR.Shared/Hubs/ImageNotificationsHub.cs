using Microsoft.AspNetCore.SignalR;

namespace Gallerai.SignalR.Shared.Hubs;

public class ImageNotificationsHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var signalRId = Context.UserIdentifier;
        Console.WriteLine($"[SignalR] New Connection: {Context.ConnectionId}");
        Console.WriteLine($"[SignalR] Identity Name: {Context.User?.Identity?.Name}");
        Console.WriteLine($"[SignalR] UserIdentifier: '{signalRId}'"); // Look for quotes!
        await base.OnConnectedAsync();
    }
}
