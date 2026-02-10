namespace Gallerai.Application.Interfaces;

public interface INotificationService
{
    Task NotifyUserUpdate<T>(string userId, T message);
}
