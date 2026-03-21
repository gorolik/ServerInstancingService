using Docker.DotNet;
using Docker.DotNet.Models;

namespace ServerInstancingService.Model.Services.ServerInstances.AsDocker;

public class DockerInstance : IServerInstance
{
    private readonly DockerClient _client;
    private readonly string _containerId;
    
    private LoggerService _loggerService;

    public int Port { get; }
    public event Action<IServerInstance, int>? OnClosed;

    public DockerInstance(DockerClient client, string containerId, int port, LoggerService loggerService)
    {
        _client = client;
        _containerId = containerId;
        Port = port;
        
        _loggerService = loggerService;

        // Запускаем фоновое отслеживание статуса контейнера
        _ = MonitorContainerAsync();
    }

    private async Task MonitorContainerAsync()
    {
        int exitCode = -1;
        
        try
        {
            ContainerWaitResponse response = await _client.Containers.WaitContainerAsync(_containerId);
            exitCode = (int)response.StatusCode;
        }
        catch (Exception ex)
        {
            _loggerService.Log(LogType.Error, $"Ошибка при мониторинге контейнера {_containerId}: {ex.Message}");
        }
        finally
        {
            OnClosed?.Invoke(this, exitCode);
        }
    }

    public async Task StopAsync()
    {
        await _client.Containers.StopContainerAsync(
            _containerId, 
            new ContainerStopParameters { WaitBeforeKillSeconds = 60 });
    }
}