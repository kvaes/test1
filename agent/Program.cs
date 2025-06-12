using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Agent.Functions;
using Agent.Services;

namespace Agent;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            var host = CreateHostBuilder(args).Build();
            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            
            logger.LogInformation("Starting Semantic Kernel Agent...");
            
            var agentService = host.Services.GetRequiredService<IAgentService>();
            await agentService.RunAsync();
            
            logger.LogInformation("Agent completed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Application terminated unexpectedly: {ex.Message}");
            Environment.Exit(1);
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // Add Semantic Kernel
                var kernelBuilder = Kernel.CreateBuilder();
                var kernel = kernelBuilder.Build();
                
                services.AddSingleton(kernel);
                services.AddScoped<IAgentService, AgentService>();
                services.AddScoped<IBicsApiService, BicsApiService>();
                services.AddHttpClient<BicsApiService>();
            });
}