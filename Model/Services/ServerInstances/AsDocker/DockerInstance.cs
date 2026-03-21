using Docker.DotNet;
using Docker.DotNet.Models;

namespace ServerInstancingService.Model.Services.ServerInstances.AsDocker;

public class DockerInstance : IServerInstance
{
    private readonly DockerClient _client;
    private readonly string _containerId;
    
    public int Port { get; }
    public event Action<IServerInstance>? OnClosed;

    public DockerInstance(DockerClient client, string containerId, int port)
    {
        _client = client;
        _containerId = containerId;
        Port = port;
        
        // Запускаем фоновое отслеживание статуса контейнера
        _ = MonitorContainerAsync();
    }

    private async Task MonitorContainerAsync()
    {
        try
        {
            // Метод заблокирует выполнение до тех пор, пока контейнер не остановится
            await _client.Containers.WaitContainerAsync(_containerId);
        }
        finally
        {
            OnClosed?.Invoke(this);
        }
    }

    public async Task StopAsync()
    {
        await _client.Containers.StopContainerAsync(
            _containerId, 
            new ContainerStopParameters { WaitBeforeKillSeconds = 5 });
    }
}