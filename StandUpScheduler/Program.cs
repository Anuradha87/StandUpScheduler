using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StandUpScheduler;

namespace StandUpScheduler;

public class Program
{
    
    public static async Task  Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appSettings.json")
            .Build();
        
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddHttpClient();
        serviceCollection.AddSingleton<IConfiguration>(configuration);
        serviceCollection.AddSingleton<IStandUpScheduleFinder,StandUpScheduleFinder>();
     
    
        var serviceProvider = serviceCollection.BuildServiceProvider();
        
        var scheduleFinder = serviceProvider.GetService<IStandUpScheduleFinder>(); 
        await scheduleFinder!.FindStandUpSchedules();
    }
    

  
}