namespace BuggyAnimalDetailsApi.Api.Middlewares;

public class FaultyApiMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<FaultyApiMiddleware> _logger;
    private readonly Random _random;

    public FaultyApiMiddleware(RequestDelegate next, ILogger<FaultyApiMiddleware> logger)
    {
        _next = next;
        _logger = logger;
        _random = new Random();
    }
    
    public async Task Invoke(HttpContext httpContext)
    {
        _logger.LogInformation("new request coming in");
        if (_random.NextDouble() >= 0.5)
        {
            _logger.LogInformation($"Returning StatusCode {StatusCodes.Status500InternalServerError.ToString()}");
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsync("Something went wrong :(");
            return;
        }
        
        await _next(httpContext);
    } 
}