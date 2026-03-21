using System.Diagnostics;
using ServerInstancingService.Model.Data;
using ServerInstancingService.Model.Services.ServerInstances;

namespace ServerInstancingService.Model.Services;

public class InstanceAllocatorService
{
    private readonly IPortService _portService;
    private AllocatorService _allocatorService;
    private IServerLauncher _serverLauncher;

    public InstanceAllocatorService(IPortService portService, 
        AllocatorService allocatorService, IServerLauncher serverLauncher)
    {
        _portService = portService;
        _allocatorService = allocatorService;
        _serverLauncher = serverLauncher;
    }

    /// <summary>
    /// Общий метод выделения порта и создания экземпляра сервера.
    /// </summary>
    /// <exception cref="Exception">В случае, когда нет свободного порта
    /// или неудачи запуска процесса</exception>
    public async Task<AllocationData> AllocateServerInstanceAsync()
    {
        AllocationData allocationData = _allocatorService.GetAllocationData();
        int port = Convert.ToInt32(allocationData.Port);
        
        IServerInstance instance = await _serverLauncher.LaunchAsync(port);
        
        instance.OnClosed += OnServerInstanceClosed;

        return allocationData;
    }

    private void OnServerInstanceClosed(IServerInstance instance)
    {
        instance.OnClosed -= OnServerInstanceClosed;
        
        _portService.ReturnPort(instance.Port);
    }
}