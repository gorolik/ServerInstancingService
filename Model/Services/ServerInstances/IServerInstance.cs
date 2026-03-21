namespace ServerInstancingService.Model.Services.ServerInstances;

/// <summary>
/// Представляет запущенный экземпляр игрового сервера.
/// </summary>
public interface IServerInstance
{
    int Port { get; }
    
    /// <summary>
    /// Событие, вызываемое при остановке сервера.
    /// </summary>
    event Action<IServerInstance, int> OnClosed;

    /// <summary>
    /// Принудительно останавливает сервер.
    /// </summary>
    Task StopAsync();
}