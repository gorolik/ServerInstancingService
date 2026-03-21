using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using ServerInstancingService.Model.Services;
using ServerInstancingService.Model.Services.ServerInstances;
using ServerInstancingService.Model.Services.ServerInstances.AsDocker;
using ServerInstancingService.Model.Services.ServerInstances.AsProcess;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("config.json")
    .AddEnvironmentVariables();

string launcherType = builder.Configuration["GameServer:LauncherType"] ?? "Local";

if (launcherType == "Docker")
    builder.Services.AddSingleton<IServerLauncher, DockerLauncher>();
else
    builder.Services.AddSingleton<IServerLauncher, LocalProcessLauncher>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<LoggerService>();
builder.Services.AddSingleton<IPortService, PortService>();
builder.Services.AddSingleton<AllocatorService>();
builder.Services.AddSingleton<InstanceAllocatorService>();

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("AllocationLimit", opt =>
    {
        opt.PermitLimit = 1; // Сколько запросов разрешено
        opt.Window = TimeSpan.FromSeconds(5); // За какой период времени
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0; // Не ставим запросы в очередь, сразу отбиваем лишние
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRateLimiter();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();