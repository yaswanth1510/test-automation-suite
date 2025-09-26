using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace ForexTestFramework.Core;

/// <summary>
/// Manages test steps and their execution flow
/// </summary>
public class StepManager
{
    private readonly ILogger<StepManager> _logger;
    private readonly ConcurrentDictionary<string, TestStep> _steps = new();
    private readonly List<StepExecution> _executionHistory = new();

    public StepManager(ILogger<StepManager> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Register a new test step
    /// </summary>
    public void RegisterStep(string stepId, string name, Func<Dictionary<string, object>, Task<StepResult>> action)
    {
        var step = new TestStep
        {
            Id = stepId,
            Name = name,
            Action = action,
            RegisteredAt = DateTime.UtcNow
        };

        _steps.TryAdd(stepId, step);
        _logger.LogDebug("Registered step: {StepName} ({StepId})", name, stepId);
    }

    /// <summary>
    /// Execute a step with parameters
    /// </summary>
    public async Task<StepResult> ExecuteStepAsync(string stepId, Dictionary<string, object> parameters)
    {
        if (!_steps.TryGetValue(stepId, out var step))
        {
            _logger.LogError("Step not found: {StepId}", stepId);
            return StepResult.Failed($"Step '{stepId}' not found");
        }

        var execution = new StepExecution
        {
            StepId = stepId,
            StepName = step.Name,
            Parameters = parameters,
            StartTime = DateTime.UtcNow
        };

        _logger.LogInformation("Executing step: {StepName}", step.Name);

        try
        {
            var result = await step.Action(parameters);
            execution.EndTime = DateTime.UtcNow;
            execution.Duration = execution.EndTime - execution.StartTime;
            execution.Result = result;
            execution.Success = result.IsSuccess;

            _executionHistory.Add(execution);

            _logger.LogInformation("Step completed: {StepName} - {Status} in {Duration}ms", 
                step.Name, result.IsSuccess ? "SUCCESS" : "FAILED", execution.Duration?.TotalMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            execution.EndTime = DateTime.UtcNow;
            execution.Duration = execution.EndTime - execution.StartTime;
            execution.Success = false;
            execution.Exception = ex;

            var result = StepResult.Failed(ex.Message);
            execution.Result = result;
            _executionHistory.Add(execution);

            _logger.LogError(ex, "Step failed: {StepName}", step.Name);
            return result;
        }
    }

    /// <summary>
    /// Execute multiple steps in sequence
    /// </summary>
    public async Task<List<StepResult>> ExecuteStepsAsync(List<string> stepIds, Dictionary<string, object> parameters)
    {
        var results = new List<StepResult>();

        foreach (var stepId in stepIds)
        {
            var result = await ExecuteStepAsync(stepId, parameters);
            results.Add(result);

            // Stop execution if step failed and configuration requires it
            if (!result.IsSuccess && result.StopOnFailure)
            {
                _logger.LogWarning("Stopping step execution due to failure in step: {StepId}", stepId);
                break;
            }
        }

        return results;
    }

    /// <summary>
    /// Get execution history
    /// </summary>
    public List<StepExecution> GetExecutionHistory() => _executionHistory.ToList();

    /// <summary>
    /// Clear execution history
    /// </summary>
    public void ClearHistory() => _executionHistory.Clear();

    /// <summary>
    /// Get registered steps
    /// </summary>
    public Dictionary<string, TestStep> GetRegisteredSteps() => _steps.ToDictionary(kv => kv.Key, kv => kv.Value);
}

public class TestStep
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Func<Dictionary<string, object>, Task<StepResult>> Action { get; set; } = null!;
    public DateTime RegisteredAt { get; set; }
    public List<string> Tags { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class StepExecution
{
    public string StepId { get; set; } = string.Empty;
    public string StepName { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public TimeSpan? Duration { get; set; }
    public bool Success { get; set; }
    public StepResult? Result { get; set; }
    public Exception? Exception { get; set; }
}

public class StepResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, object> Data { get; set; } = new();
    public List<string> Screenshots { get; set; } = new();
    public List<string> Artifacts { get; set; } = new();
    public bool StopOnFailure { get; set; } = true;

    public static StepResult Success(string message = "Step completed successfully", Dictionary<string, object>? data = null)
    {
        return new StepResult
        {
            IsSuccess = true,
            Message = message,
            Data = data ?? new Dictionary<string, object>()
        };
    }

    public static StepResult Failed(string message, Dictionary<string, object>? data = null, bool stopOnFailure = true)
    {
        return new StepResult
        {
            IsSuccess = false,
            Message = message,
            Data = data ?? new Dictionary<string, object>(),
            StopOnFailure = stopOnFailure
        };
    }
}