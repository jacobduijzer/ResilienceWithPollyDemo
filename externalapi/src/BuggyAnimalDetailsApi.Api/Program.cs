using BuggyAnimalDetailsApi.Api;
using BuggyAnimalDetailsApi.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOptions()
    .AddMemoryCache()
    .AddSingleton<FakeAnimalGenerator>()
    .AddSingleton<CacheService>()
    .AddControllers();

builder.Services
  .AddEndpointsApiExplorer()
  .AddSwaggerGen();

var faultyApiFlag = new FaultyApiFlag(builder.Configuration.GetValue<bool>("FeatureFlags:EnableFaultyApi"));
var rateLimitedFlag = new RateLimitedApiFlag(builder.Configuration.GetValue<bool>("FeatureFlags:EnableRateLimit"));
var coolingDownFlagFlag = new CoolingDownApiFlag(builder.Configuration.GetValue<bool>("FeatureFlags:EnableCoolingDown"));
Console.WriteLine($"FaultyApiFlag: {faultyApiFlag.Enabled}");
Console.WriteLine($"RateLimitFlag: {rateLimitedFlag.Enabled}");
Console.WriteLine($"CoolingDownFlag: {coolingDownFlagFlag.Enabled}");
builder.Services.AddSingleton<CoolingDownApiFlag>(coolingDownFlagFlag);

var app = builder.Build();
  app
        .UseSwagger()
        .UseSwaggerUI();

if (faultyApiFlag.Enabled)
    app.UseFaultyApiMiddleware();
else if (rateLimitedFlag.Enabled)
    app.UseRateLimiterMiddleWare();

app.UseAuthorization();
app.MapControllers();
app.Run();
