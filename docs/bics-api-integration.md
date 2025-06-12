# BICS API Integration Guide

This guide covers the integration with the BICS API Developer Portal APIs as outlined in the project requirements.

## Overview

The BICS API Developer Portal (https://developer.bics.com/portal/apis) provides various telecommunications and connectivity APIs. This project integrates with these APIs through dedicated Semantic Kernel plugins.

## Available BICS API Categories

Based on typical telecommunications API portals, the BICS API likely includes:

### 1. Network APIs
- **Network Status**: Check network connectivity and status
- **Network Topology**: Retrieve network topology information
- **Route Management**: Manage and query routing information

### 2. Pricing APIs
- **Rate Lookup**: Get pricing for various services
- **Cost Calculation**: Calculate costs for specific usage patterns
- **Billing Information**: Access billing and invoice data

### 3. Service Management APIs
- **Service Catalog**: Browse available services
- **Service Provisioning**: Provision new services
- **Service Monitoring**: Monitor service performance

### 4. Customer APIs
- **Account Management**: Manage customer accounts
- **Usage Reporting**: Report and track usage
- **Support Operations**: Handle customer support requests

## Plugin Implementation

### Current BICS API Plugin

The `BicsApiPlugin` class provides the foundation for BICS API integration:

```csharp
[KernelFunction, Description("Get pricing information from BICS API")]
public async Task<string> GetPricingAsync(string endpoint)

[KernelFunction, Description("Get network information from BICS API")]
public async Task<string> GetNetworkInfoAsync(string endpoint)

[KernelFunction, Description("Get service catalog from BICS API")]
public async Task<string> GetServiceCatalogAsync()
```

### Extending the Plugin

To add support for additional BICS API endpoints:

#### 1. Add New Kernel Functions

```csharp
[KernelFunction, Description("Get customer account information")]
public async Task<string> GetCustomerAccountAsync(
    [Description("Customer ID")] string customerId)
{
    var endpoint = $"customers/{customerId}";
    return await _bicsApiService.GetApiDataAsync(endpoint);
}

[KernelFunction, Description("Report usage data to BICS")]
public async Task<string> ReportUsageAsync(
    [Description("Usage data in JSON format")] string usageData)
{
    // Implementation for reporting usage
    return await _bicsApiService.PostApiDataAsync("usage/report", usageData);
}
```

#### 2. Extend the Service Interface

```csharp
public interface IBicsApiService
{
    Task InitializeAsync();
    Task<string> GetApiDataAsync(string endpoint);
    Task<string> PostApiDataAsync(string endpoint, string data);
    Task<string> PutApiDataAsync(string endpoint, string data);
    Task<string> DeleteApiDataAsync(string endpoint);
}
```

## Configuration

### API Configuration

Add BICS API configuration to `appsettings.json`:

```json
{
  "BicsApi": {
    "BaseUrl": "https://developer.bics.com/api/v1",
    "ApiKey": "your-api-key-here",
    "Timeout": "00:00:30",
    "RetryCount": 3,
    "RateLimitPerMinute": 100
  }
}
```

### Environment Variables

For secure deployment, use environment variables:

```bash
export BICS_API_KEY=your-actual-api-key
export BICS_API_BASE_URL=https://developer.bics.com/api/v1
```

## Authentication

### API Key Authentication

Most APIs use API key authentication:

```csharp
public class BicsApiService : IBicsApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public BicsApiService(IConfiguration configuration)
    {
        _apiKey = configuration["BicsApi:ApiKey"];
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("X-API-Key", _apiKey);
    }
}
```

### OAuth 2.0 (if required)

For OAuth 2.0 authentication:

```csharp
public async Task<string> GetAccessTokenAsync()
{
    var tokenRequest = new
    {
        grant_type = "client_credentials",
        client_id = _clientId,
        client_secret = _clientSecret
    };

    var response = await _httpClient.PostAsJsonAsync("oauth/token", tokenRequest);
    var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
    return tokenResponse.AccessToken;
}
```

## Error Handling

### API-Specific Error Handling

```csharp
public async Task<string> GetApiDataAsync(string endpoint)
{
    try
    {
        var response = await _httpClient.GetAsync(endpoint);
        
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new BicsApiException("API authentication failed");
        }
        
        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            // Handle rate limiting
            await Task.Delay(TimeSpan.FromSeconds(60));
            return await GetApiDataAsync(endpoint); // Retry
        }
        
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
    catch (HttpRequestException ex)
    {
        throw new BicsApiException($"Network error calling BICS API: {ex.Message}", ex);
    }
}
```

### Rate Limiting

Implement rate limiting to respect API limits:

```csharp
public class RateLimiter
{
    private readonly Queue<DateTime> _requests = new();
    private readonly int _maxRequests;
    private readonly TimeSpan _timeWindow;

    public async Task WaitIfNeeded()
    {
        var now = DateTime.UtcNow;
        
        // Remove old requests outside the time window
        while (_requests.Count > 0 && now - _requests.Peek() > _timeWindow)
        {
            _requests.Dequeue();
        }

        if (_requests.Count >= _maxRequests)
        {
            var waitTime = _timeWindow - (now - _requests.Peek());
            await Task.Delay(waitTime);
        }

        _requests.Enqueue(now);
    }
}
```

## Data Models

### Response Models

Create models for API responses:

```csharp
public class BicsApiResponse<T>
{
    public bool Success { get; set; }
    public T Data { get; set; }
    public string Message { get; set; }
    public int StatusCode { get; set; }
}

public class PricingInfo
{
    public string ServiceId { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; }
    public string PricingModel { get; set; }
}

public class NetworkInfo
{
    public string NetworkId { get; set; }
    public string Status { get; set; }
    public double Latency { get; set; }
    public double Bandwidth { get; set; }
}
```

## Testing

### Unit Testing API Integration

```csharp
[Test]
public async Task GetPricingAsync_ValidEndpoint_ReturnsData()
{
    // Arrange
    var mockApiService = new Mock<IBicsApiService>();
    mockApiService.Setup(x => x.GetApiDataAsync("pricing/voice"))
              .ReturnsAsync("mock pricing data");
    
    var plugin = new BicsApiPlugin(mockApiService.Object);

    // Act
    var result = await plugin.GetPricingAsync("pricing/voice");

    // Assert
    Assert.That(result, Is.EqualTo("Pricing data from endpoint: pricing/voice"));
}
```

### Integration Testing

```csharp
[Test]
public async Task BicsApiService_RealEndpoint_ReturnsValidData()
{
    // Arrange
    var configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string>
        {
            ["BicsApi:BaseUrl"] = "https://api-sandbox.bics.com",
            ["BicsApi:ApiKey"] = "test-key"
        })
        .Build();

    var service = new BicsApiService(Mock.Of<ILogger<BicsApiService>>(), configuration);

    // Act & Assert
    await service.InitializeAsync();
    // Additional assertions based on expected behavior
}
```

## Monitoring and Logging

### Request Logging

```csharp
public async Task<string> GetApiDataAsync(string endpoint)
{
    _logger.LogInformation("Calling BICS API endpoint: {Endpoint}", endpoint);
    
    var stopwatch = Stopwatch.StartNew();
    try
    {
        var result = await _httpClient.GetStringAsync(endpoint);
        _logger.LogInformation("BICS API call successful. Endpoint: {Endpoint}, Duration: {Duration}ms", 
                             endpoint, stopwatch.ElapsedMilliseconds);
        return result;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "BICS API call failed. Endpoint: {Endpoint}, Duration: {Duration}ms", 
                        endpoint, stopwatch.ElapsedMilliseconds);
        throw;
    }
}
```

### Performance Metrics

Track API performance:

```csharp
public class BicsApiMetrics
{
    public int TotalRequests { get; set; }
    public int SuccessfulRequests { get; set; }
    public int FailedRequests { get; set; }
    public double AverageResponseTime { get; set; }
    public DateTime LastRequestTime { get; set; }
}
```

## Best Practices

### 1. Connection Management
- Use `HttpClientFactory` for efficient connection pooling
- Configure appropriate timeouts
- Implement proper disposal patterns

### 2. Security
- Never hard-code API keys
- Use secure configuration providers
- Implement proper authentication handling

### 3. Reliability
- Implement retry logic with exponential backoff
- Handle rate limiting gracefully
- Use circuit breaker patterns for resilience

### 4. Performance
- Cache frequently accessed data
- Use async/await patterns throughout
- Monitor and optimize API usage

## Troubleshooting

### Common Issues

1. **Authentication Failures**
   - Verify API key configuration
   - Check token expiration
   - Validate API permissions

2. **Rate Limiting**
   - Implement proper rate limiting
   - Monitor API usage patterns
   - Use caching to reduce calls

3. **Network Issues**
   - Configure appropriate timeouts
   - Implement retry logic
   - Handle transient failures

### Debugging

Enable detailed logging for troubleshooting:

```json
{
  "Logging": {
    "LogLevel": {
      "Agent.Services.BicsApiService": "Debug",
      "Agent.Functions.BicsApiPlugin": "Debug"
    }
  }
}
```

## Future Enhancements

1. **Additional API Support**: Add support for new BICS API endpoints as they become available
2. **Caching Layer**: Implement caching for frequently accessed data
3. **Event-Driven Integration**: Use webhooks for real-time updates
4. **Batch Processing**: Support batch operations for improved efficiency