namespace Gallerai.Application.Interfaces;

public interface INotificationService
{
    ValueTask NotifyUserUpdate<T>(string userId, T message);
}
