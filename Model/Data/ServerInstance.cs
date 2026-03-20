using System.Diagnostics;

namespace ServerInstancingService.Model.Data;

public class ServerInstance
{
    public Process Process { get; }
    public int Port { get; }

    public Action<ServerInstance> OnClosed;

    public ServerInstance(Process process, int port)
    {
        Process = process;
        Port = port;
        
        Process.Exited += ProcessOnExited;
    }

    ~ServerInstance() => Process.Exited -= ProcessOnExited;

    private void ProcessOnExited(object? sender, EventArgs e) => 
        OnClosed?.Invoke(this);
}