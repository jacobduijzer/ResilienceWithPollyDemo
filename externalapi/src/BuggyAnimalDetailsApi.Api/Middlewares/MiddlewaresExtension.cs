namespace BuggyAnimalDetailsApi.Api.Middlewares;

public static class MiddlewaresExtension
{
    public static IApplicationBuilder UseFaultyApiMiddleware(this IApplicationBuilder builder) =>
        builder.UseMiddleware<FaultyApiMiddleware>();

    public static IApplicationBuilder UseRateLimiterMiddleWare(this IApplicationBuilder builder) =>
        builder.UseMiddleware<RateLimiterMiddleWare>();
}