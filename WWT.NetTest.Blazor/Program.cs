using Serilog;
using StackExchange.Redis;
using WWT.NetTest.Blazor.ApiEndpoints;
using WWT.NetTest.Blazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((hostContext, services, configuration) => {
    configuration.WriteTo.Console();
});


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpContextAccessor();


if (builder.Configuration.GetValue<string>("REDIS_CONNECTION_STRING") is null)
{
    throw new ApplicationException(
        "Cannot find REDIS_CONNECTION_STRING environmental variable. This is a fatal error. Exiting"
        );
}

WebApplication? app = null;
try{

    builder.Services.AddSingleton<IConnectionMultiplexer>(option => ConnectionMultiplexer.Connect(builder.Configuration.GetValue<string>("REDIS_CONNECTION_STRING")));
    builder.Services.AddSingleton<IServerCountCache, RedisService>();
    
    app = builder.Build();

    app.Services.GetService<IServerCountCache>(); //Initiate instance of singleton to ensure bootstrapping works

}
catch (Exception e)
{
    
    
    //Prevents app from starting if REDIS is not accessible 
    
    var logger = app.Services.GetService<ILogger<Program>>();
    
    logger.LogCritical("Exception connecting to Redis at startup. Exiting {Error}", e.Message);
    throw;
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection(); // HTTPS Redirect Disabled for ease of use in containers

app.UseStaticFiles();

app.MapApiEndPoints();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();