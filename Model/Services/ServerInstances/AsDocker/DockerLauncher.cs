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
        string containerName = $"aw-match-{port}-{Guid.NewGuid().ToString()[..4]}";

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
                AutoRemove = true
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