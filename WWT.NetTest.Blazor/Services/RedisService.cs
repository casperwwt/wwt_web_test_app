using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;


namespace WWT.NetTest.Blazor.Services;



public class RedisService : IServerCountCache
{
    public IConnectionMultiplexer Connection { get; }

    private readonly ILogger<RedisService> _logger;
    private readonly IConnectionMultiplexer _connection;
    private readonly IDatabase _db;
    
    
    public RedisService(ILogger<RedisService> logger, IConnectionMultiplexer connection)
    {
        _logger = logger;
        _connection = connection;
        _db = _connection.GetDatabase(); // Will fail if Redis is inaccessible at startup.
   }
    
    public int GetIndividualServerCounter(string serverName)
    {

        try
        {
            bool found = int.TryParse(_db.StringGet(serverName), out int counter);

            if (!found) counter = 0;

            return counter;
        }
        catch (Exception e)
        {
            
            return -1;
        }
        
    }

    public int GetAllServersCounter()
    {

        var keys = _connection.GetServer(_connection.GetEndPoints().First()).Keys(_db.Database).ToArray();
        var total = 0;

        foreach (var key in keys)
        {
            total += GetIndividualServerCounter(key);
        }

        return total;
    }


    public  void IncreaseIndividualConnectionCounter(string serverName)
    {
        

        bool found = int.TryParse( _db.StringGet($"{serverName}"), out int counter);

        if (!found) counter = 0;
        counter++;
        _db.StringSet(serverName, counter.ToString());
    }

    public string GetRedisConfigurationString()
    {
        if (_connection.Configuration is not null)
        {
            return _connection.Configuration.ToString();
        }
        
        return "Not Configured";
    }


}