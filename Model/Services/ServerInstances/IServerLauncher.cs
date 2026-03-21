namespace ServerInstancingService.Model.Services.ServerInstances;

/// <summary>
/// Отвечает за физический запуск сервера в целевой среде.
/// </summary>
public interface IServerLauncher
{
    /// <summary>
    /// Запускает новый экземпляр сервера на указанном порту.
    /// </summary>
    Task<IServerInstance> LaunchAsync(int port);
}