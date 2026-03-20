using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using ServerInstancingService.Model.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IPortService, PortService>();
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

builder.Configuration.AddJsonFile("config.json");

app.UseHttpsRedirection();
app.UseRateLimiter();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();