# Plugin Development Guide

This guide explains how to develop plugins for the Semantic Kernel agent in the Test1 project.

## Overview

The Test1 project uses Microsoft Semantic Kernel's native plugin system to provide extensible functionality. Plugins are implemented as C# classes with methods decorated with the `KernelFunction` attribute.

## Plugin Architecture

### Core Concepts

1. **Kernel Functions**: Methods that can be called by the Semantic Kernel
2. **Plugins**: Classes that group related kernel functions
3. **Dependency Injection**: Plugins can use services through DI
4. **Error Handling**: Consistent error handling patterns across plugins

### Plugin Structure

```csharp
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace Agent.Functions;

public class ExamplePlugin
{
    private readonly IExampleService _service;
    private readonly ILogger<ExamplePlugin> _logger;

    public ExamplePlugin(IExampleService service, ILogger<ExamplePlugin> logger)
    {
        _service = service;
        _logger = logger;
    }

    [KernelFunction, Description("Example function description")]
    public async Task<string> ExampleFunctionAsync(
        [Description("Parameter description")] string input)
    {
        try
        {
            _logger.LogInformation("Executing example function with input: {Input}", input);
            var result = await _service.ProcessAsync(input);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in example function");
            throw new PluginException("Example function failed", ex);
        }
    }
}
```

## Creating New Plugins

### Step 1: Define the Plugin Class

Create a new class in the `agent/Functions/` directory:

```csharp
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace Agent.Functions;

public class MyNewPlugin
{
    // Constructor for dependency injection
    public MyNewPlugin(ILogger<MyNewPlugin> logger)
    {
        _logger = logger;
    }
}
```

### Step 2: Add Kernel Functions

Decorate methods with the `KernelFunction` attribute:

```csharp
[KernelFunction, Description("Processes user data and returns formatted result")]
public async Task<string> ProcessUserDataAsync(
    [Description("User data in JSON format")] string userData,
    [Description("Processing options")] string options = "default")
{
    // Implementation here
    return "Processed result";
}
```

### Step 3: Register the Plugin

Add the plugin to the dependency injection container in `Program.cs`:

```csharp
services.AddScoped<MyNewPlugin>();

// Register with Semantic Kernel
kernel.Plugins.AddFromType<MyNewPlugin>();
```

## Plugin Types

### 1. Data Processing Plugins

For data transformation and validation:

```csharp
public class DataProcessingPlugin
{
    [KernelFunction, Description("Validates and transforms input data")]
    public string ValidateAndTransform(
        [Description("Input data to validate")] string data,
        [Description("Validation rules")] string rules)
    {
        // Validation logic
        if (string.IsNullOrWhiteSpace(data))
            return "Error: Input data is required";

        // Transformation logic
        return data.ToUpperCase();
    }

    [KernelFunction, Description("Parses CSV data into structured format")]
    public async Task<string> ParseCsvAsync(
        [Description("CSV data string")] string csvData)
    {
        // CSV parsing implementation
        await Task.CompletedTask;
        return "Parsed CSV data";
    }
}
```

### 2. API Integration Plugins

For external API interactions:

```csharp
public class ExternalApiPlugin
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ExternalApiPlugin(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    [KernelFunction, Description("Calls external API and returns response")]
    public async Task<string> CallExternalApiAsync(
        [Description("API endpoint")] string endpoint,
        [Description("Request payload")] string payload = "")
    {
        try
        {
            var baseUrl = _configuration["ExternalApi:BaseUrl"];
            var response = await _httpClient.GetAsync($"{baseUrl}/{endpoint}");
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            return $"Error calling API: {ex.Message}";
        }
    }
}
```

### 3. Utility Plugins

For common utility functions:

```csharp
public class UtilityPlugin
{
    [KernelFunction, Description("Generates a unique identifier")]
    public string GenerateId([Description("ID prefix")] string prefix = "")
    {
        return $"{prefix}{Guid.NewGuid():N}";
    }

    [KernelFunction, Description("Formats date and time")]
    public string FormatDateTime(
        [Description("Date time to format")] string dateTime,
        [Description("Format string")] string format = "yyyy-MM-dd HH:mm:ss")
    {
        if (DateTime.TryParse(dateTime, out var dt))
        {
            return dt.ToString(format);
        }
        return "Invalid date format";
    }

    [KernelFunction, Description("Calculates hash for input string")]
    public string CalculateHash([Description("Input string")] string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
```

## Advanced Plugin Features

### Stateful Plugins

Plugins can maintain state across function calls:

```csharp
public class StatefulPlugin
{
    private readonly Dictionary<string, object> _state = new();

    [KernelFunction, Description("Stores a value in plugin state")]
    public string StoreValue(
        [Description("Key for the value")] string key,
        [Description("Value to store")] string value)
    {
        _state[key] = value;
        return $"Stored {key} = {value}";
    }

    [KernelFunction, Description("Retrieves a value from plugin state")]
    public string GetValue([Description("Key to retrieve")] string key)
    {
        return _state.TryGetValue(key, out var value) 
            ? value.ToString() 
            : "Key not found";
    }
}
```

### Async Operations

Handle long-running operations properly:

```csharp
public class AsyncOperationsPlugin
{
    [KernelFunction, Description("Performs a long-running operation")]
    public async Task<string> LongRunningOperationAsync(
        [Description("Operation parameters")] string parameters)
    {
        // Simulate long-running work
        await Task.Delay(TimeSpan.FromSeconds(5));
        
        // Actual implementation would go here
        return $"Operation completed with parameters: {parameters}";
    }

    [KernelFunction, Description("Processes multiple items concurrently")]
    public async Task<string> ProcessConcurrentlyAsync(
        [Description("Items to process (comma-separated)")] string items)
    {
        var itemList = items.Split(',').Select(s => s.Trim());
        
        var tasks = itemList.Select(async item =>
        {
            await Task.Delay(100); // Simulate processing
            return $"Processed: {item}";
        });

        var results = await Task.WhenAll(tasks);
        return string.Join(", ", results);
    }
}
```

## Error Handling in Plugins

### Custom Exceptions

Create specific exception types for your plugins:

```csharp
public class PluginException : Exception
{
    public string PluginName { get; }
    public string FunctionName { get; }

    public PluginException(string pluginName, string functionName, string message) 
        : base(message)
    {
        PluginName = pluginName;
        FunctionName = functionName;
    }

    public PluginException(string pluginName, string functionName, string message, Exception innerException) 
        : base(message, innerException)
    {
        PluginName = pluginName;
        FunctionName = functionName;
    }
}
```

### Error Handling Patterns

```csharp
public class RobustPlugin
{
    private readonly ILogger<RobustPlugin> _logger;

    [KernelFunction, Description("Function with comprehensive error handling")]
    public async Task<string> RobustFunctionAsync(
        [Description("Input data")] string input)
    {
        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(input))
            {
                const string error = "Input cannot be null or empty";
                _logger.LogWarning(error);
                return $"Error: {error}";
            }

            // Process input
            var result = await ProcessInputAsync(input);
            _logger.LogInformation("Successfully processed input");
            return result;
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided");
            return $"Validation Error: {ex.Message}";
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error occurred");
            return "Error: Network connectivity issue";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in robust function");
            throw new PluginException("RobustPlugin", "RobustFunctionAsync", 
                "An unexpected error occurred", ex);
        }
    }

    private async Task<string> ProcessInputAsync(string input)
    {
        // Implementation details
        await Task.Delay(100);
        return $"Processed: {input}";
    }
}
```

## Testing Plugins

### Unit Testing

```csharp
[TestFixture]
public class MyPluginTests
{
    private MyPlugin _plugin;
    private Mock<ILogger<MyPlugin>> _mockLogger;

    [SetUp]
    public void Setup()
    {
        _mockLogger = new Mock<ILogger<MyPlugin>>();
        _plugin = new MyPlugin(_mockLogger.Object);
    }

    [Test]
    public async Task ProcessDataAsync_ValidInput_ReturnsExpectedResult()
    {
        // Arrange
        const string input = "test data";
        const string expected = "Processed: test data";

        // Act
        var result = await _plugin.ProcessDataAsync(input);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ValidateInput_NullInput_ReturnsErrorMessage()
    {
        // Arrange
        string input = null;

        // Act
        var result = _plugin.ValidateInput(input);

        // Assert
        Assert.That(result, Does.Contain("Invalid"));
    }
}
```

### Integration Testing

```csharp
[TestFixture]
public class PluginIntegrationTests
{
    private Kernel _kernel;
    private IServiceProvider _serviceProvider;

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddScoped<MyPlugin>();
        
        _serviceProvider = services.BuildServiceProvider();
        _kernel = Kernel.CreateBuilder()
            .AddFromType<MyPlugin>(_serviceProvider)
            .Build();
    }

    [Test]
    public async Task Kernel_CanExecutePluginFunction()
    {
        // Act
        var result = await _kernel.InvokeAsync("MyPlugin", "ProcessDataAsync", 
            new KernelArguments { ["input"] = "test" });

        // Assert
        Assert.That(result.GetValue<string>(), Is.Not.Null);
    }
}
```

## Plugin Configuration

### Plugin-Specific Settings

Add configuration sections for your plugins:

```json
{
  "Plugins": {
    "MyPlugin": {
      "Enabled": true,
      "MaxRetries": 3,
      "Timeout": "00:00:30",
      "CustomSettings": {
        "Feature1": true,
        "Feature2": false
      }
    }
  }
}
```

### Using Configuration in Plugins

```csharp
public class ConfigurablePlugin
{
    private readonly PluginSettings _settings;

    public ConfigurablePlugin(IConfiguration configuration)
    {
        _settings = configuration.GetSection("Plugins:MyPlugin").Get<PluginSettings>();
    }

    [KernelFunction, Description("Function that uses configuration")]
    public async Task<string> ConfigurableFunctionAsync(string input)
    {
        if (!_settings.Enabled)
        {
            return "Plugin is disabled";
        }

        // Use configuration values
        var timeout = _settings.Timeout;
        var maxRetries = _settings.MaxRetries;

        // Implementation using configuration
        return "Function executed with configuration";
    }
}

public class PluginSettings
{
    public bool Enabled { get; set; }
    public int MaxRetries { get; set; }
    public TimeSpan Timeout { get; set; }
    public Dictionary<string, object> CustomSettings { get; set; }
}
```

## Performance Considerations

### Caching

Implement caching for expensive operations:

```csharp
public class CachingPlugin
{
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheExpiry = TimeSpan.FromMinutes(10);

    public CachingPlugin(IMemoryCache cache)
    {
        _cache = cache;
    }

    [KernelFunction, Description("Function with caching")]
    public async Task<string> CachedFunctionAsync(string input)
    {
        var cacheKey = $"cached_function_{input}";
        
        if (_cache.TryGetValue(cacheKey, out string cachedResult))
        {
            return cachedResult;
        }

        // Expensive operation
        var result = await ExpensiveOperationAsync(input);
        
        _cache.Set(cacheKey, result, _cacheExpiry);
        return result;
    }
}
```

### Resource Management

Properly dispose of resources:

```csharp
public class ResourceManagementPlugin : IDisposable
{
    private readonly HttpClient _httpClient;
    private bool _disposed;

    public ResourceManagementPlugin()
    {
        _httpClient = new HttpClient();
    }

    [KernelFunction, Description("Function that uses resources")]
    public async Task<string> UseResourcesAsync(string input)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        
        // Use resources
        var response = await _httpClient.GetStringAsync("https://api.example.com");
        return response;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _httpClient?.Dispose();
            _disposed = true;
        }
    }
}
```

## Best Practices

### 1. Function Design
- Keep functions focused and single-purpose
- Use descriptive names and descriptions
- Validate inputs thoroughly
- Handle errors gracefully

### 2. Documentation
- Provide clear descriptions for functions and parameters
- Include examples in XML documentation
- Document any side effects or prerequisites

### 3. Performance
- Use async/await for I/O operations
- Implement appropriate caching strategies
- Consider resource pooling for expensive resources

### 4. Testing
- Write unit tests for all public functions
- Include integration tests for external dependencies
- Test error conditions and edge cases

### 5. Security
- Validate and sanitize all inputs
- Use secure communication for external APIs
- Never log sensitive information

## Common Patterns

### Request/Response Pattern

```csharp
[KernelFunction, Description("Processes a request and returns a response")]
public async Task<string> ProcessRequestAsync(
    [Description("Request data in JSON format")] string request)
{
    try
    {
        var requestObj = JsonSerializer.Deserialize<RequestModel>(request);
        var response = await ProcessAsync(requestObj);
        return JsonSerializer.Serialize(response);
    }
    catch (JsonException ex)
    {
        return JsonSerializer.Serialize(new { Error = "Invalid JSON format" });
    }
}
```

### Batch Processing Pattern

```csharp
[KernelFunction, Description("Processes multiple items in batch")]
public async Task<string> ProcessBatchAsync(
    [Description("Items to process (JSON array)")] string items)
{
    var itemArray = JsonSerializer.Deserialize<string[]>(items);
    var results = new List<string>();

    foreach (var item in itemArray)
    {
        try
        {
            var result = await ProcessSingleItemAsync(item);
            results.Add(result);
        }
        catch (Exception ex)
        {
            results.Add($"Error processing {item}: {ex.Message}");
        }
    }

    return JsonSerializer.Serialize(results);
}
```

## Plugin Discovery and Loading

### Dynamic Plugin Loading

For advanced scenarios, plugins can be loaded dynamically:

```csharp
public class PluginManager
{
    private readonly Kernel _kernel;

    public async Task LoadPluginsFromAssemblyAsync(Assembly assembly)
    {
        var pluginTypes = assembly.GetTypes()
            .Where(t => t.GetMethods().Any(m => m.GetCustomAttribute<KernelFunctionAttribute>() != null));

        foreach (var pluginType in pluginTypes)
        {
            _kernel.Plugins.AddFromType(pluginType);
        }
    }
}
```

This guide provides a comprehensive foundation for developing plugins in the Test1 project. Follow these patterns and best practices to create robust, maintainable, and efficient plugins.