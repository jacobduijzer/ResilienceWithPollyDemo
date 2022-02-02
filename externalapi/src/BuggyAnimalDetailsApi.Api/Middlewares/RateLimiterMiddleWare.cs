using System.Globalization;

namespace BuggyAnimalDetailsApi.Api.Middlewares;

public class RateLimiterMiddleWare
{
    private readonly RequestDelegate _next;
    private readonly CacheService _cacheService;
    private readonly ILogger<RateLimiterMiddleWare> _logger;

    public RateLimiterMiddleWare(RequestDelegate next, CacheService cacheService, ILogger<RateLimiterMiddleWare> logger)
    {
        _next = next;
        _cacheService = cacheService;
        _logger = logger;
    }
    
    public async Task Invoke(HttpContext httpContext)
    {
        _logger.LogInformation("new request coming in");
        
        if (!_cacheService.TryAddRequestToCache(httpContext.Request.Path))
        {
            _logger.LogInformation($"Returning StatusCode {StatusCodes.Status429TooManyRequests.ToString()}");
            httpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            httpContext.Response.Headers["Retry-After"] = DateTime.Now.AddSeconds(CacheService.ExpireItemsInSeconds).ToString(CultureInfo.InvariantCulture);
            await httpContext.Response.WriteAsync("Please, calm down!");
            return;
        }
        
        await _next(httpContext);
    }  
}