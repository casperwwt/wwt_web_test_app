
using WWT.NetTest.Blazor.Helper;
using WWT.NetTest.Blazor.Services;

namespace WWT.NetTest.Blazor.ApiEndpoints;


public static class ApiEndpointMapper
{

    public static void MapApiEndPoints(this WebApplication app)
    {
        app.MapPost("/load/{seconds}", async (int seconds) =>
        {
            Helpers.CpuLoadGenerator(seconds);
        });


        app.MapPost("/kill", (IHostApplicationLifetime app) =>
        {
            
            app.StopApplication();
            //throw new ApplicationException("User initiated kill");
            return Results.Ok();
        });

        app.MapGet("counter/server", (IServerCountCache cacheService) =>
        {
            try
            {
                return Results.Ok(cacheService.GetIndividualServerCounter(Environment.MachineName));
            }
            catch (Exception e)
            {
                return Results.Problem();
            }
        });
        
        app.MapGet("counter/all", (IServerCountCache cacheService) =>
        {
            try
            {
                return Results.Ok(cacheService.GetAllServersCounter());
            }
            catch (Exception e)
            {
                return Results.Problem();
            }
        });
        
        app.MapGet("random", () =>
        {
            return Results.Ok($"{Environment.MachineName} : {Helpers.RandomString(32)}");
        });

    }
}