using System.Diagnostics;

namespace ServerInstancingService.Model.Services.ServerInstances.AsProcess;

public class LocalProcessLauncher : IServerLauncher
{
    public Task<IServerInstance> LaunchAsync(int port)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = @"C:\Users\Maksim\Desktop\Air Wars server\Air Wars.exe",
            Arguments = $"-batchmode -nographics -headless -server_port {port}",
            UseShellExecute = false,
            CreateNoWindow = false
        };

        var process = new Process { StartInfo = startInfo };
        process.Start();

        IServerInstance instance = new ProcessInstance(process, port);

        return Task.FromResult(instance); 
    }
}