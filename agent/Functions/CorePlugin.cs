using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace Agent.Functions;

public class CorePlugin
{
    [KernelFunction, Description("Process data with error handling")]
    public async Task<string> ProcessDataAsync(
        [Description("The data to process")] string data)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(data))
                throw new ArgumentException("Data cannot be null or empty", nameof(data));

            // Simulate data processing
            await Task.Delay(50);
            return $"Processed: {data}";
        }
        catch (Exception ex)
        {
            return $"Error processing data: {ex.Message}";
        }
    }

    [KernelFunction, Description("Validate input data")]
    public string ValidateInput(
        [Description("The input to validate")] string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "Invalid: Input is null or empty";
        
        if (input.Length > 1000)
            return "Invalid: Input exceeds maximum length";
        
        return "Valid input";
    }
}