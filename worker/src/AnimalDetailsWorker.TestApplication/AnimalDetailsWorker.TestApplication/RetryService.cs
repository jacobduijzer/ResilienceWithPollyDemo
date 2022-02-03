using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;

namespace AnimalDetailsWorker.TestApplication;

public class RetryService
{
    private const int ExceptionsAllowedBeforeBreakingCircuit = 3;
    private const int CircuitBreakingTimeInSeconds = 30;
    
    private readonly ILogger<RetryService> _logger;
    private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;
    
    public RetryService(ILogger<RetryService> logger)
    {
        _logger = logger;
        _circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(ExceptionsAllowedBeforeBreakingCircuit, TimeSpan.FromSeconds(CircuitBreakingTimeInSeconds),
        (ex, t) => logger.LogInformation("Circuit broken!"),
        () => logger.LogInformation("Circuit reset!"));
    }

    public async Task RetryForever(Func<Task> apiCall, TimeSpan waitTimeBetweenRetries) =>
        await Policy
            .Handle<Exception>()
            .WaitAndRetryForeverAsync((count, context) =>
            {
                _logger.LogInformation($"Retry {count}, waiting {waitTimeBetweenRetries.TotalSeconds} seconds.");
                // MAYBE DO SOMETHING HERE TO PREVENT MORE EXCEPTIONS, LIKE AUTHENTICATING
                return waitTimeBetweenRetries;
            })
            .ExecuteAsync(apiCall)
            .ConfigureAwait(false);

    public async Task Retry(Func<Task> apiCall, int numberOfRetries, TimeSpan waitTimeBetweenRetries) =>
        await Policy
            .Handle<Exception>()
            .RetryAsync(numberOfRetries,
                (exception, retryCount, context) =>
                {
                    _logger.LogInformation($"Retry {retryCount}/{numberOfRetries}, exception: {exception.Message}");
                    // MAYBE DO SOMETHING HERE TO PREVENT MORE EXCEPTIONS, LIKE AUTHENTICATING
                })
            .ExecuteAsync(apiCall)
            .ConfigureAwait(false);

    public async Task RetryWithFallBack(
        Func<Task> apiCall,
        Func<Task> fallback,
        int numberOfRetries,
        TimeSpan waitTimeBetweenRetries)
    {
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                numberOfRetries,
                retryAttempt => waitTimeBetweenRetries,
                (exception, timeSpan, retryCount, context) =>
                    _logger.LogInformation($"Wait and retry ({retryCount}/{numberOfRetries}: {exception.Message}"));

        var fallbackPolicy = Policy
            .Handle<Exception>()
            .FallbackAsync((cancellationToken) => fallback.Invoke());

        await fallbackPolicy
            .WrapAsync(retryPolicy)
            .ExecuteAsync(apiCall)
            .ConfigureAwait(false);
    }

    public async Task WaitAndRetryWithCircuitBreaker(Func<Task> apiCall, int numberOfRetries, TimeSpan waitTimeBetweenRetries)
    {
        var brokenPolicy = Policy
            .Handle<BrokenCircuitException>()
            .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(CircuitBreakingTimeInSeconds));
            
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                numberOfRetries,
                retryAttempt => waitTimeBetweenRetries,
                (exception, timeSpan, retryCount, context) =>
                    _logger.LogInformation($"Wait and retry ({retryCount}/{numberOfRetries}: {exception.Message}"));
    
        await brokenPolicy.WrapAsync(retryPolicy).WrapAsync(_circuitBreakerPolicy).ExecuteAsync(apiCall);
    }
}