using Microsoft.AspNetCore.Mvc;
using ForexTestSuite.Application.Services;

namespace ForexTestSuite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestExecutionController : ControllerBase
{
    private readonly ITestExecutionService _testExecutionService;

    public TestExecutionController(ITestExecutionService testExecutionService)
    {
        _testExecutionService = testExecutionService;
    }

    /// <summary>
    /// Execute a single test
    /// </summary>
    [HttpPost("execute-test")]
    public async Task<ActionResult<TestExecutionResult>> ExecuteTest(
        TestExecutionRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _testExecutionService.ExecuteTestAsync(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Execute a test suite
    /// </summary>
    [HttpPost("execute-suite")]
    public async Task<ActionResult<IEnumerable<TestExecutionResult>>> ExecuteTestSuite(
        TestSuiteExecutionRequest request,
        CancellationToken cancellationToken = default)
    {
        var results = await _testExecutionService.ExecuteTestSuiteAsync(request, cancellationToken);
        return Ok(results);
    }

    /// <summary>
    /// Get execution status
    /// </summary>
    [HttpGet("status/{executionId:guid}")]
    public async Task<ActionResult<TestExecutionResult>> GetExecutionStatus(
        Guid executionId,
        CancellationToken cancellationToken = default)
    {
        var result = await _testExecutionService.GetExecutionStatusAsync(executionId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Cancel test execution
    /// </summary>
    [HttpPost("cancel/{executionId:guid}")]
    public async Task<ActionResult<bool>> CancelExecution(
        Guid executionId,
        CancellationToken cancellationToken = default)
    {
        var result = await _testExecutionService.CancelExecutionAsync(executionId, cancellationToken);
        return Ok(result);
    }
}

[ApiController]
[Route("api/[controller]")]
public class TestDiscoveryController : ControllerBase
{
    private readonly ITestDiscoveryService _testDiscoveryService;

    public TestDiscoveryController(ITestDiscoveryService testDiscoveryService)
    {
        _testDiscoveryService = testDiscoveryService;
    }

    /// <summary>
    /// Discover tests in a directory
    /// </summary>
    [HttpPost("discover")]
    public async Task<ActionResult<IEnumerable<DiscoveredTest>>> DiscoverTests(
        [FromBody] DiscoverTestsRequest request,
        CancellationToken cancellationToken = default)
    {
        var tests = await _testDiscoveryService.DiscoverTestsAsync(request.RootPath, cancellationToken);
        return Ok(tests);
    }

    /// <summary>
    /// Discover tests in an assembly
    /// </summary>
    [HttpPost("discover-assembly")]
    public async Task<ActionResult<IEnumerable<DiscoveredTest>>> DiscoverTestsInAssembly(
        [FromBody] DiscoverAssemblyTestsRequest request,
        CancellationToken cancellationToken = default)
    {
        var tests = await _testDiscoveryService.DiscoverTestsInAssemblyAsync(request.AssemblyPath, cancellationToken);
        return Ok(tests);
    }
}

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportingService _reportingService;

    public ReportsController(IReportingService reportingService)
    {
        _reportingService = reportingService;
    }

    /// <summary>
    /// Generate a test report
    /// </summary>
    [HttpPost("generate")]
    public async Task<ActionResult<ReportResult>> GenerateReport(
        ReportRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _reportingService.GenerateReportAsync(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get available reports
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReportSummary>>> GetAvailableReports(
        CancellationToken cancellationToken = default)
    {
        var reports = await _reportingService.GetAvailableReportsAsync(cancellationToken);
        return Ok(reports);
    }

    /// <summary>
    /// Download report data
    /// </summary>
    [HttpGet("{reportId:guid}/download")]
    public async Task<ActionResult> DownloadReport(
        Guid reportId,
        CancellationToken cancellationToken = default)
    {
        var reportData = await _reportingService.GetReportDataAsync(reportId, cancellationToken);
        return File(reportData, "application/octet-stream", $"report-{reportId}.zip");
    }
}

// Request DTOs
public class DiscoverTestsRequest
{
    public string RootPath { get; set; } = string.Empty;
}

public class DiscoverAssemblyTestsRequest
{
    public string AssemblyPath { get; set; } = string.Empty;
}