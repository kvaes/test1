using Microsoft.Extensions.Logging;

namespace Agent.Services;

public interface IAgentService
{
    Task RunAsync();
}

public class AgentService : IAgentService
{
    private readonly ILogger<AgentService> _logger;
    private readonly IBicsApiService _bicsApiService;

    public AgentService(ILogger<AgentService> logger, IBicsApiService bicsApiService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _bicsApiService = bicsApiService ?? throw new ArgumentNullException(nameof(bicsApiService));
    }

    public async Task RunAsync()
    {
        try
        {
            _logger.LogInformation("Agent service starting...");
            
            // Example usage of BICS API service
            await _bicsApiService.InitializeAsync();
            
            _logger.LogInformation("Agent service completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in agent service");
            throw new AgentServiceException("Agent service failed", ex);
        }
    }
}

public class AgentServiceException : Exception
{
    public AgentServiceException(string message) : base(message) { }
    public AgentServiceException(string message, Exception innerException) : base(message, innerException) { }
}