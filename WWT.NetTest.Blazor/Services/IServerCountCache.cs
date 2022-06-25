namespace WWT.NetTest.Blazor.Services;

public interface IServerCountCache
{
    int GetIndividualServerCounter(string serverName);
    int GetAllServersCounter();
    void IncreaseIndividualConnectionCounter(string serverName);
}