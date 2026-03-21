namespace ServerInstancingService.Model.Services.ServerInstances;

/// <summary>
/// Представляет запущенный экземпляр игрового сервера.
/// </summary>
public interface IServerInstance
{
    int Port { get; }
    
    /// <summary>
    /// Событие, вызываемое при остановке (падении/закрытии) сервера.
    /// </summary>
    event Action<IServerInstance> OnClosed;

    /// <summary>
    /// Принудительно останавливает сервер.
    /// </summary>
    Task StopAsync();
}