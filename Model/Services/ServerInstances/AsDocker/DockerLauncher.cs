using Docker.DotNet;
using Docker.DotNet.Models;

namespace ServerInstancingService.Model.Services.ServerInstances.AsDocker;

public class DockerLauncher : IServerLauncher
{
    private readonly DockerClient _client;
    
    private LoggerService _loggerService;

    public DockerLauncher(LoggerService loggerService)
    {
        _loggerService = loggerService;
        _client = new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock")).CreateClient();
    }

    public async Task<IServerInstance> LaunchAsync(int port)
    {
        string portStr = port.ToString();
        string containerName = $"aw-match-{port}-{Guid.NewGuid().ToString("N")[..8]}";

        var parameters = new CreateContainerParameters
        {
            Image = "airwars-game-server:latest",
            Name = containerName,
            ExposedPorts = new Dictionary<string, EmptyStruct> { { $"{portStr}/udp", default } },
            HostConfig = new HostConfig
            {
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    { $"{portStr}/udp", new List<PortBinding> { new() { HostPort = portStr } } }
                },
                // Ограничения ресурсов
                Memory = 1024L * 1024 * 512,      // Лимит 512 МБ предотвратят падение сервера по OOM
                MemorySwap = 1024L * 1024 * 512,  // Диск не будет использоваться вместо RAM (стабильный FPS)
                CPUShares = 256,                   // Гарантирует ~25% мощности ядра при пиковых перегрузках
                PidsLimit = 150,                   // Защита от утечки потоков Unity
                
                // Изоляция и безопасность ядра
                SecurityOpt = new List<string> { "no-new-privileges:true" },
                CapDrop = new List<string> { "ALL" },
                
                // Ротация логов (защита диска хоста от заполнения при "-logfile -")
                LogConfig = new LogConfig
                {
                    Type = "json-file",
                    Config = new Dictionary<string, string>
                    {
                        { "max-size", "50m" },
                        { "max-file", "2" }
                    }
                },
                
                AutoRemove = true,
                Init = true
            },
            Cmd = new List<string>
            {
                "-logfile", "-",
                "-server_port", portStr,
            }
        };

        var response = await _client.Containers.CreateContainerAsync(parameters);
        await _client.Containers.StartContainerAsync(response.ID, new ContainerStartParameters());

        return new DockerInstance(_client, response.ID, port, _loggerService);
    }
}