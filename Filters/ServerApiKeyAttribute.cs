using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace AirWarsDatabase.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class ApiKeyRequired : Attribute, IAsyncAuthorizationFilter
{
    private const string _apiKeyHeaderName = "X-Api-Key";

    private static readonly IReadOnlyDictionary<ApiKeyType, string> _configKeyMappings = 
        new Dictionary<ApiKeyType, string>
        {
            [ApiKeyType.Server] = "SERVER_API_KEY",
            [ApiKeyType.Client] = "CLIENT_API_KEY"
        };

    private readonly ApiKeyType[] _allowedTypes;

    public ApiKeyRequired(params ApiKeyType[] allowedTypes)
    {
        _allowedTypes = allowedTypes.Length > 0
            ? allowedTypes
            : new[] { ApiKeyType.Server };
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        HttpRequest request = context.HttpContext.Request;

        if (!request.Headers.TryGetValue(_apiKeyHeaderName, out StringValues extractedApiKey))
        {
            context.Result = new UnauthorizedObjectResult("API key is missing.");
            return;
        }

        IConfiguration configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        byte[] actualBytes = Encoding.UTF8.GetBytes(extractedApiKey.ToString());
        bool isValid = false;

        foreach (ApiKeyType type in _allowedTypes)
        {
            string configKey = _configKeyMappings[type];
            string? expectedApiKey = configuration[configKey];

            if (string.IsNullOrWhiteSpace(expectedApiKey))
            {
                context.Result = new ObjectResult($"API key for '{type}' is not configured on the server.")
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
                return;
            }

            byte[] expectedBytes = Encoding.UTF8.GetBytes(expectedApiKey);

            if (expectedBytes.Length == actualBytes.Length &&
                CryptographicOperations.FixedTimeEquals(expectedBytes, actualBytes))
            {
                isValid = true;
                break;
            }
        }

        if (!isValid)
        {
            context.Result = new UnauthorizedObjectResult("Invalid API key.");
            return;
        }

        await Task.CompletedTask;
    }
}