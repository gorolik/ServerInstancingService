using System.Collections.Concurrent;

namespace ServerInstancingService.Model.Services;

/// <summary>
/// Управляет свободными портами.
/// </summary>
public class PortService : IPortService
{
    private readonly ConcurrentQueue<int> _availablePorts;
    private LoggerService _loggerService;

    public PortService(LoggerService loggerService)
    {
        _loggerService = loggerService;
        _availablePorts = new ConcurrentQueue<int>();

        for (int i = 7001; i < 7017; i++) 
            _availablePorts.Enqueue(i);
    }

    public int TryGetPort()
    {
        if (_availablePorts.TryDequeue(out int port))
        {
            _loggerService.Log(LogType.Info, "Port " + port + " taken");

            return port;
        }

        _loggerService.Log(LogType.Error, "No available ports");
        throw new Exception("No available ports");
    }

    public void ReturnPort(int port)
    {
        _availablePorts.Enqueue(port);
        _loggerService.Log(LogType.Info, "Port " + port + " returned");
    }
}