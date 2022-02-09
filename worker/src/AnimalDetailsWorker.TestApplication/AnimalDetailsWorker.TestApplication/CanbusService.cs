using System.Net;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;

namespace AnimalDetailsWorker.TestApplication;

public class CanbusService
{
    private readonly CanbusDataApi _canbusDataApi;
    private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;

    public CanbusService(CanbusDataApi canbusDataApi, ILogger<CanbusService> logger)
    {
        _canbusDataApi = canbusDataApi;
        _circuitBreakerPolicy = Policy
            .Handle<HttpRequestException>(with => with.StatusCode == HttpStatusCode.TooManyRequests)
            .CircuitBreakerAsync(exceptionsAllowedBeforeBreaking: 3, durationOfBreak: TimeSpan.FromSeconds(30),
                (ex, t) => logger.LogInformation("Circuit broken!"),
                () => logger.LogInformation("Circuit reset!"));
    }
    
    public async Task<ScheduledEvent> PlanMaintenanceForCar(User user, Car car)
    {
        var brokenPolicy = Policy
            .Handle<BrokenCircuitException>()
            .WaitAndRetryAsync(retryCount: 3, sleepDurationProvider:duration => TimeSpan.FromSeconds(3));

        var serverErrorPolicy = Policy
            .Handle<HttpRequestException>(with => with.StatusCode == HttpStatusCode.InternalServerError)
            .FallbackAsync(async (cancellationToken) => await AddToQueue(user, car));
        
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(retryCount:3, sleepDurationProvider:duration => TimeSpan.FromSeconds(3));
        
        return await brokenPolicy
            .WrapAsync(retryPolicy)
            .WrapAsync(serverErrorPolicy)
            .WrapAsync(_circuitBreakerPolicy)
            .ExecuteAsync(async () => await _canbusDataApi.PlanMaintenance(user, car));
    }

    public async Task<CanbusData> Get(User user, Car car)
    {
        var unauthorizedApiPolicy = Policy
            .Handle<UnauthorizedAccessException>()
            .RetryAsync(async (exception, i, context) => await _canbusDataApi.AuthorizeCall());

        var tokenExpiredPolicy = Policy
            .Handle<TokenExpiredException>()
            .RetryAsync(async (exception, i, context) => await _canbusDataApi.AuthenticateUser(user));

        return await unauthorizedApiPolicy.WrapAsync(tokenExpiredPolicy)
            .ExecuteAsync(async () => await _canbusDataApi.GetCanbusData(user, car));
    }

    private async Task<ScheduledEvent> AddToQueue(User user, Car car)
    {
        // DUMMY CODE
        return await Task.FromResult(new ScheduledEvent());
    }
}

public class CanbusDataApi 
{
    public async Task AuthorizeCall()
    {
    }

    public async Task AuthenticateUser(User user)
    {
    }

    public async Task<CanbusData> GetCanbusData(User user, Car car)
    {
        return default;
    }

    public async Task<ScheduledEvent> PlanMaintenance(User user, Car car)
    {
        return default;
    }
}

public class ScheduledEvent
{
    
}

public class User
{
}

public class Car
{
}

public class CanbusData
{
}

public class TokenExpiredException : Exception
{
}