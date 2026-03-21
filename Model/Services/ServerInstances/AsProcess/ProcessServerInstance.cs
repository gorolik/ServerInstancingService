using System.Diagnostics;

namespace ServerInstancingService.Model.Services.ServerInstances.AsProcess;

public class ProcessInstance : IServerInstance
{
    private readonly Process _process;
    public int Port { get; }
    
    public event Action<IServerInstance, int>? OnClosed;

    public ProcessInstance(Process process, int port)
    {
        _process = process;
        Port = port;
        
        _process.EnableRaisingEvents = true;
        _process.Exited += OnProcessExited;
    }

    private void OnProcessExited(object? sender, EventArgs e)
    {
        _process.Exited -= OnProcessExited;
        
        int exitCode = _process.ExitCode; 
        
        _process.Dispose();
        OnClosed?.Invoke(this, exitCode);
    }

    public Task StopAsync()
    {
        if (!_process.HasExited)
        {
            _process.Kill();
        }
        return Task.CompletedTask;
    }
}