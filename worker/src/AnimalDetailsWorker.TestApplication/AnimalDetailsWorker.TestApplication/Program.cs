using AnimalDetailsWorker.TestApplication;
using Microsoft.Extensions.Logging;
using Refit;

const int retryCount = 6;
const int millisecondsToWaitBetweenExceptions = 500;

Console.WriteLine("Getting animal details from external API");

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .AddFilter("Microsoft", LogLevel.Warning)
        .AddFilter("System", LogLevel.Warning)
        .AddFilter("NonHostConsoleApp.Program", LogLevel.Debug)
        .AddConsole();
});

var logger = loggerFactory.CreateLogger<Program>();
var api = RestService.For<IAnimalDetailsApi>("http://localhost");
var retryService = new RetryService(loggerFactory.CreateLogger<RetryService>());

const int numberOfRequests = 1000;
foreach (var fakeAnimalId in Enumerable.Range(0, numberOfRequests))
{
    Func<Task> apiCall = async () =>
    {
        var animalDetails = await api.GetAnimalDetails(fakeAnimalId);
        logger.LogInformation($"Animal details for animal {animalDetails.AnimalId} => Name: {animalDetails.Name}, Date of Birth: {animalDetails.DateOfBirth}");
    };

    Func<Task> fallback = async () =>
    {
        logger.LogInformation($"Handling fallback for animal {fakeAnimalId}");
        // store to local json file for later processing
    };

    try
    {
        // 1 => no policies, ok when API is healthy, losing data when API is faulty
        // await apiCall.Invoke().ConfigureAwait(false);

        // 2 => simple retry policy, retry forever
        // await retryService
        //     .RetryForever(apiCall: apiCall, waitTimeBetweenRetries: TimeSpan.FromSeconds(3))
        //     .ConfigureAwait(false);
        
        // 2a =>? retry 3 times. throw
        // await retryService.Retry(apiCall, 3, TimeSpan.FromSeconds(3)).ConfigureAwait(false);

        // 3 => retry with fallback
        // await retryService.RetryWithFallBack(
        //         apiCall: apiCall,
        //         fallback: fallback,
        //         numberOfRetries: 3,
        //         TimeSpan.FromSeconds(3))
        //     .ConfigureAwait(false);

        // 4 => retry with circuit breaker
        await retryService.WaitAndRetryWithCircuitBreaker(apiCall, 3, TimeSpan.FromSeconds(3));
    }
    catch (Exception exception)
    {
        Console.WriteLine($"Unhandled exception: {exception.Message}");
    }
}

Console.WriteLine("Done, bye!");