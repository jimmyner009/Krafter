using Backend.Api;
using Backend.Features.Auth.Token;

namespace Backend.Api.Configuration;

/// <summary>
/// VSA route discovery and registration
/// </summary>
public static class RouteConfiguration
{
    public static IServiceCollection AddRouteDiscovery(this IServiceCollection services)
    {
        var assembly = typeof(GetToken.TokenRoute).Assembly;

        var routeRegistrars = assembly.GetTypes()
            .Where(t => typeof(IRouteRegistrar).IsAssignableFrom(t) &&
                        t is { IsInterface: false, IsAbstract: false } &&
                        t != typeof(IRouteRegistrar))
            .ToList();

        foreach (var routeType in routeRegistrars)
        {
            services.AddSingleton(typeof(IRouteRegistrar), routeType);
        }

        return services;
    }

    public static IApplicationBuilder MapDiscoveredRoutes(this IApplicationBuilder app)
    {
        foreach (var registrar in app.ApplicationServices.GetServices<IRouteRegistrar>())
        {
            registrar.MapRoute((IEndpointRouteBuilder)app);
        }

        return app;
    }
}
