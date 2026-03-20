using System.Diagnostics;
using ServerInstancingService.Model.Data;

namespace ServerInstancingService.Model.Services;

public class InstanceAllocatorService
{
    private readonly List<ServerInstance> _serverInstances;
    private readonly IPortService _portService;

    public InstanceAllocatorService(IPortService portService)
    {
        _serverInstances = new List<ServerInstance>();
        _portService = portService;
    }

    /// <summary>
    /// Общий метод выделения порта и создания экземпляра сервера.
    /// </summary>
    /// <exception cref="Exception">В случае, когда нет свободного порта
    /// или неудачи запуска процесса</exception>
    public AllocationData AllocateServerInstance()
    {
        AllocationData allocationData = GetAllocationData();
        CreateServerInstance(Convert.ToInt32(allocationData?.Port));

        return allocationData;
    }
    
    /// <summary>
    /// Выделяет доступный порт и возвращает данные для подключения
    /// </summary>
    /// <returns>Данные подключения к серверу</returns>
    private AllocationData GetAllocationData()
    {
        int port = _portService.TryGetPort();

        AllocationData allocationData = new AllocationData
        {
            Ip = "127.0.0.1",
            Port = port.ToString(),
        };

        return allocationData;
    }
    
    /// <summary>
    /// Запускает процесс сервера, формирует инстанс и добавляет его в коллекцию экземпляров.
    /// Подписывается на событие о закрытии экземпляра.
    /// </summary>
    /// <param name="port">Порт, который будет слушать экземпляр сервера</param>
    private void CreateServerInstance(int port)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo()
        {
            FileName = "C:\\Users\\Maksim\\Desktop\\Air Wars server\\Air Wars.exe",
            Arguments = $"-batchmode -nographics -headless -server_ip 0.0.0.0 -server_port {port}",
            UseShellExecute = false,

            // Для теста оставляем окно видимым
            CreateNoWindow = false
        };
        
        Process serverProcess = new Process()
        {
            StartInfo = startInfo,
            EnableRaisingEvents = true,
        };

        ServerInstance serverInstance = new ServerInstance(serverProcess, port);
        serverInstance.OnClosed += OnServerInstanceClosed;
        
        _serverInstances.Add(serverInstance);
        
        serverProcess.Start();
    }

    private void OnServerInstanceClosed(ServerInstance instance)
    {
        _portService.ReturnPort(instance.Port);
        instance.Process.Dispose();

        _serverInstances.Remove(instance);
    }
}