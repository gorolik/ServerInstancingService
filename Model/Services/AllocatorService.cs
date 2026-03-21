using ServerInstancingService.Model.Data;

namespace ServerInstancingService.Model.Services;

/// <summary>
/// Выделяет адрес и порт для подключения
/// </summary>
public class AllocatorService
{
    private readonly IPortService _portService;
    private readonly string _serverIp;
    private readonly int _maxAllocations;

    public AllocatorService(IPortService portService, IConfiguration config)
    {
        _portService = portService;
        _serverIp = config["GameServer:IpAddress"] ?? "127.0.0.1";
        _maxAllocations = int.Parse(config["GameServer:MaxConcurrentServers"] ?? string.Empty);
    }
    
    /// <summary>
    /// Выделяет доступный порт и возвращает данные для подключения
    /// </summary>
    /// <returns>Данные подключения к серверу</returns>
    public AllocationData GetAllocationData(int currentAllocationsCount)
    {
        if (currentAllocationsCount >= _maxAllocations)
            throw new Exception("No available allocations, limit reached");
        
        int port = _portService.TryGetPort();

        AllocationData allocationData = new AllocationData
        {
            Ip = _serverIp,
            Port = port.ToString(),
        };

        return allocationData;
    }
}