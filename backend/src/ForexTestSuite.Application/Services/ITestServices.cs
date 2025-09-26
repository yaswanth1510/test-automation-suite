namespace ForexTestSuite.Application.Services;

public interface ITestDiscoveryService
{
    Task<IEnumerable<DiscoveredTest>> DiscoverTestsAsync(string rootPath, CancellationToken cancellationToken = default);
    Task<IEnumerable<DiscoveredTest>> DiscoverTestsInAssemblyAsync(string assemblyPath, CancellationToken cancellationToken = default);
    Task<bool> ValidateTestAsync(DiscoveredTest test, CancellationToken cancellationToken = default);
}

public interface ITestExecutionService
{
    Task<TestExecutionResult> ExecuteTestAsync(TestExecutionRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<TestExecutionResult>> ExecuteTestSuiteAsync(TestSuiteExecutionRequest request, CancellationToken cancellationToken = default);
    Task<TestExecutionResult> GetExecutionStatusAsync(Guid executionId, CancellationToken cancellationToken = default);
    Task<bool> CancelExecutionAsync(Guid executionId, CancellationToken cancellationToken = default);
}

public interface IReportingService
{
    Task<ReportResult> GenerateReportAsync(ReportRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<ReportSummary>> GetAvailableReportsAsync(CancellationToken cancellationToken = default);
    Task<byte[]> GetReportDataAsync(Guid reportId, CancellationToken cancellationToken = default);
}

public interface IParameterGeneratorService
{
    Task<Dictionary<string, object>> GenerateParametersAsync(ParameterGenerationRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<Dictionary<string, object>>> GenerateParameterSetsAsync(ParameterSetGenerationRequest request, CancellationToken cancellationToken = default);
}

public interface IAssetManagementService
{
    Task<string> StoreAssetAsync(AssetStoreRequest request, CancellationToken cancellationToken = default);
    Task<byte[]> RetrieveAssetAsync(string assetPath, CancellationToken cancellationToken = default);
    Task<bool> DeleteAssetAsync(string assetPath, CancellationToken cancellationToken = default);
    Task<IEnumerable<AssetInfo>> ListAssetsAsync(string? filterPath = null, CancellationToken cancellationToken = default);
}

public interface IComparisonService
{
    Task<ComparisonResult> CompareObjectsAsync<T>(T expected, T actual, ComparisonOptions? options = null);
    Task<ComparisonResult> CompareJsonAsync(string expectedJson, string actualJson, ComparisonOptions? options = null);
    Task<ComparisonResult> CompareXmlAsync(string expectedXml, string actualXml, ComparisonOptions? options = null);
}

// DTOs
public class DiscoveredTest
{
    public string Name { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string MethodName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string TestType { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public Dictionary<string, object> Attributes { get; set; } = new();
}

public class TestExecutionRequest
{
    public Guid TestCaseId { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
    public Dictionary<string, string> Environment { get; set; } = new();
    public int TimeoutSeconds { get; set; } = 300;
}

public class TestSuiteExecutionRequest
{
    public Guid TestSuiteId { get; set; }
    public List<Guid>? TestCaseIds { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
    public Dictionary<string, string> Environment { get; set; } = new();
    public bool RunInParallel { get; set; } = true;
    public int MaxParallelTests { get; set; } = 4;
}

public class TestExecutionResult
{
    public Guid ExecutionId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public TimeSpan? Duration { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<string, object> Results { get; set; } = new();
    public List<string> Screenshots { get; set; } = new();
    public List<string> Artifacts { get; set; } = new();
}

public class ReportRequest
{
    public Guid? TestSessionId { get; set; }
    public List<Guid>? TestExecutionIds { get; set; }
    public string Format { get; set; } = "Html";
    public Dictionary<string, object> Options { get; set; } = new();
}

public class ReportResult
{
    public Guid ReportId { get; set; }
    public string Format { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class ReportSummary
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public Dictionary<string, object> Statistics { get; set; } = new();
}

public class ParameterGenerationRequest
{
    public string ParameterType { get; set; } = string.Empty;
    public Dictionary<string, object> Configuration { get; set; } = new();
}

public class ParameterSetGenerationRequest
{
    public List<ParameterGenerationRequest> Parameters { get; set; } = new();
    public int Count { get; set; } = 1;
}

public class AssetStoreRequest
{
    public string Name { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public byte[] Data { get; set; } = Array.Empty<byte>();
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class AssetInfo
{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ComparisonResult
{
    public bool AreEqual { get; set; }
    public List<string> Differences { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class ComparisonOptions
{
    public bool IgnoreCase { get; set; } = false;
    public bool IgnoreWhitespace { get; set; } = false;
    public List<string> IgnoreProperties { get; set; } = new();
    public double NumericTolerance { get; set; } = 0.0;
}