using AirWarsDatabase.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ServerInstancingService.Model;
using ServerInstancingService.Model.Data;
using ServerInstancingService.Model.Services;

namespace ServerInstancingService.Controllers;

/// <summary>
/// Принимает запросы на запуск сервера и возвращает данные для подключения к запущенному экземпляру сервера.
/// </summary>
[ApiController]
[Route("api/allocation_request")]
[EnableRateLimiting("AllocationLimit")]
[ApiKeyRequired(ApiKeyType.Client)]
public class InstanceAllocationController : ControllerBase
{
    private readonly InstanceAllocatorService _allocatorService;
    private LoggerService _loggerService;

    public InstanceAllocationController(InstanceAllocatorService allocatorService, IConfiguration configuration,
        LoggerService loggerService)
    {
        _loggerService = loggerService;
        _allocatorService = allocatorService;
    }

    [HttpPost]
    public async Task<IActionResult> Allocate([FromBody]AllocationRequest allocationRequest)
    {
        try
        {
            AllocationData allocationData = await _allocatorService.AllocateServerInstanceAsync();
            _loggerService.Log(LogType.Info, "Allocation successfully: " + allocationData);
            return Ok(allocationData);
        }
        catch (Exception e)
        {
            _loggerService.Log(LogType.Error, "Allocation unsuccessfully: " + e.Message);
            return Problem(e.Message);
        }
    }
}