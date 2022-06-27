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

var redisSuccessFull = false;
WebApplication? app = null;
builder.Services.AddSingleton<IConnectionMultiplexer>(option =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetValue<string>("REDIS_CONNECTION_STRING")));
builder.Services.AddSingleton<IServerCountCache, RedisService>();
        
        
app = builder.Build();


while (!redisSuccessFull)
{
    try
    {
    
        app.Services.GetService<IServerCountCache>(); //Initiate instance of singleton to ensure bootstrapping works
        
        
        redisSuccessFull = true;

    }
    catch (Exception e)
    {
        //Prevents app from starting if REDIS is not accessible 
        
        Console.WriteLine("Exception connecting to Redis at startup. Will retry in 5 seconds");
        Thread.Sleep(5000);
    }
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