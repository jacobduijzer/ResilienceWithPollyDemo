using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;

namespace AnimalDetailsWorker.TestApplication;

public class RetryService
{
    private readonly ILogger<RetryService> _logger;

    private AsyncCircuitBreakerPolicy _circuitBreakerPolicy;
    
    private const int ExceptionsAllowedBeforeBreakingCircuit = 3;
    private const int CircuitBreakingTimeInSeconds = 30;
    
    public RetryService(ILogger<RetryService> logger)
    {
        _logger = logger;

        _circuitBreakerPolicy = Policy.Handle<Exception>()
        .CircuitBreakerAsync(1, TimeSpan.FromMinutes(1),
        (ex, t) => logger.LogInformation("Circuit broken!"),
        () => logger.LogInformation("Circuit reset!"));
    }

    public async Task RetryForever(Func<Task> apiCall, TimeSpan waitTimeBetweenRetries) =>
        await Policy
            .Handle<Exception>()
            .WaitAndRetryForeverAsync((count, context) =>
            {
                _logger.LogInformation($"Retry {count}, waiting {waitTimeBetweenRetries.TotalSeconds} seconds.");
                return waitTimeBetweenRetries;
            })
            .ExecuteAsync(apiCall)
            .ConfigureAwait(false);

    public async Task Retry(Func<Task> apiCall, int numberOfRetries, TimeSpan waitTimeBetweenRetries) =>
        await Policy
            .Handle<Exception>()
            .RetryAsync(numberOfRetries, (exception, retryCount, context) => _logger.LogInformation($"Retry {retryCount}/{numberOfRetries}, exception: {exception.Message}"))
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
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                numberOfRetries,
                retryAttempt => waitTimeBetweenRetries,
                (exception, timeSpan, retryCount, context) =>
                    _logger.LogInformation($"Wait and retry ({retryCount}/{numberOfRetries}: {exception.Message}"));
        
        // var circuitBreakerPolicy = Policy
        // .Handle<BrokenCircuitException>()
        // .WaitAndRetryAsync(
        //     numberOfRetries,
        //     retryAttept => TimeSpan.FromSeconds(CircuitBreakingTimeInSeconds),
        //     (exception, timeSpan, retryCount, context) =>
        //         _logger.LogInformation($"Wait and retry ({retryCount}/{numberOfRetries}: {exception.Message}")
        // );
        
        // var circuitPolicy = CreateCircuitBreakerPolicy(numberOfRetries, waitTime);
    
        await retryPolicy.WrapAsync(_circuitBreakerPolicy).ExecuteAsync(apiCall);
        // await _circuitBreakerPolicy.WrapAsync(retryPolicy).ExecuteAsync(apiCall).ConfigureAwait(false);
    }
}