namespace WWT.NetTest.Blazor.Helper;

public class Helpers
{
    public static double SlowFunction(int baseNumber)
    {
        double result = 0;
        for (var i = Math.Pow(baseNumber, 7); i >= 0; i--) {		
            result += Math.Atan(i) * Math.Tan(i);
        };

        return result;
    }

    public static void CpuLoadGenerator(int seconds)
    {
        Enumerable
            .Range(1, Environment.ProcessorCount) // replace with lesser number if 100% usage is not what you are after.
            .AsParallel()
            .Select(i =>
            {
                var end = DateTime.Now + TimeSpan.FromSeconds(seconds);
                while (DateTime.Now < end)
                    /*nothing here */ ;
                return i;
            })
            .ToList();
    }
    
    public static string RandomString(int length)
    {

        var random = new Random();
        
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_+-=";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
    
    
}