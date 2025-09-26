using Microsoft.AspNetCore.SignalR;

namespace ForexTestSuite.Api.Hubs;

public class TestExecutionHub : Hub
{
    /// <summary>
    /// Join a test execution group to receive real-time updates
    /// </summary>
    /// <param name="executionId">The ID of the test execution to monitor</param>
    public async Task JoinExecution(string executionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"execution-{executionId}");
    }

    /// <summary>
    /// Leave a test execution group
    /// </summary>
    /// <param name="executionId">The ID of the test execution to stop monitoring</param>
    public async Task LeaveExecution(string executionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"execution-{executionId}");
    }

    /// <summary>
    /// Join a test session group to receive updates for all tests in the session
    /// </summary>
    /// <param name="sessionId">The ID of the test session to monitor</param>
    public async Task JoinSession(string sessionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"session-{sessionId}");
    }

    /// <summary>
    /// Leave a test session group
    /// </summary>
    /// <param name="sessionId">The ID of the test session to stop monitoring</param>
    public async Task LeaveSession(string sessionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"session-{sessionId}");
    }

    /// <summary>
    /// Join the global notifications group for system-wide updates
    /// </summary>
    public async Task JoinGlobalNotifications()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "global-notifications");
    }

    /// <summary>
    /// Leave the global notifications group
    /// </summary>
    public async Task LeaveGlobalNotifications()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "global-notifications");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // Clean up group memberships when client disconnects
        await base.OnDisconnectedAsync(exception);
    }
}

/// <summary>
/// Service for sending SignalR notifications
/// </summary>
public interface ITestExecutionNotificationService
{
    Task SendTestStartedAsync(Guid executionId, string testName);
    Task SendTestCompletedAsync(Guid executionId, string testName, string status, TimeSpan duration);
    Task SendTestProgressAsync(Guid executionId, string stepName, string status);
    Task SendTestErrorAsync(Guid executionId, string errorMessage);
    Task SendSessionStartedAsync(Guid sessionId, string sessionName, int totalTests);
    Task SendSessionCompletedAsync(Guid sessionId, string sessionName, int passed, int failed, int skipped);
    Task SendSystemNotificationAsync(string message, string type = "info");
}

public class TestExecutionNotificationService : ITestExecutionNotificationService
{
    private readonly IHubContext<TestExecutionHub> _hubContext;

    public TestExecutionNotificationService(IHubContext<TestExecutionHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendTestStartedAsync(Guid executionId, string testName)
    {
        await _hubContext.Clients.Group($"execution-{executionId}")
            .SendAsync("TestStarted", new
            {
                ExecutionId = executionId,
                TestName = testName,
                Timestamp = DateTime.UtcNow
            });
    }

    public async Task SendTestCompletedAsync(Guid executionId, string testName, string status, TimeSpan duration)
    {
        await _hubContext.Clients.Group($"execution-{executionId}")
            .SendAsync("TestCompleted", new
            {
                ExecutionId = executionId,
                TestName = testName,
                Status = status,
                Duration = duration,
                Timestamp = DateTime.UtcNow
            });
    }

    public async Task SendTestProgressAsync(Guid executionId, string stepName, string status)
    {
        await _hubContext.Clients.Group($"execution-{executionId}")
            .SendAsync("TestProgress", new
            {
                ExecutionId = executionId,
                StepName = stepName,
                Status = status,
                Timestamp = DateTime.UtcNow
            });
    }

    public async Task SendTestErrorAsync(Guid executionId, string errorMessage)
    {
        await _hubContext.Clients.Group($"execution-{executionId}")
            .SendAsync("TestError", new
            {
                ExecutionId = executionId,
                ErrorMessage = errorMessage,
                Timestamp = DateTime.UtcNow
            });
    }

    public async Task SendSessionStartedAsync(Guid sessionId, string sessionName, int totalTests)
    {
        await _hubContext.Clients.Group($"session-{sessionId}")
            .SendAsync("SessionStarted", new
            {
                SessionId = sessionId,
                SessionName = sessionName,
                TotalTests = totalTests,
                Timestamp = DateTime.UtcNow
            });
    }

    public async Task SendSessionCompletedAsync(Guid sessionId, string sessionName, int passed, int failed, int skipped)
    {
        await _hubContext.Clients.Group($"session-{sessionId}")
            .SendAsync("SessionCompleted", new
            {
                SessionId = sessionId,
                SessionName = sessionName,
                Passed = passed,
                Failed = failed,
                Skipped = skipped,
                Timestamp = DateTime.UtcNow
            });
    }

    public async Task SendSystemNotificationAsync(string message, string type = "info")
    {
        await _hubContext.Clients.Group("global-notifications")
            .SendAsync("SystemNotification", new
            {
                Message = message,
                Type = type,
                Timestamp = DateTime.UtcNow
            });
    }
}