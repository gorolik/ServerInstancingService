using System.Collections.Concurrent;

namespace ServerInstancingService.Model.Services;

/// <summary>
/// Управляет свободными портами.
/// </summary>
public class PortService : IPortService
{
    private readonly ConcurrentQueue<int> _availablePorts;

    public PortService()
    {
        _availablePorts = new ConcurrentQueue<int>();

        for (int i = 7000; i < 7016; i++) 
            _availablePorts.Enqueue(i);
    }

    public int TryGetPort()
    {
        if (_availablePorts.TryDequeue(out int port))
        {
            Console.WriteLine("[API] Port " + port + " taken");
            return port;
        }

        throw new Exception("No available ports");
    }

    public void ReturnPort(int port)
    {
        _availablePorts.Enqueue(port);
        Console.WriteLine("[API] Port " + port + " returned");
    }
}