using ForexTestSuite.Domain.Common;
using ForexTestSuite.Domain.Enums;

namespace ForexTestSuite.Domain.Entities;

public class TestSuite : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0.0";
    public bool IsActive { get; set; } = true;
    public List<string> Tags { get; set; } = new();
    public Dictionary<string, object> Configuration { get; set; } = new();
    
    // Navigation properties
    public ICollection<TestCase> TestCases { get; set; } = new List<TestCase>();
    public ICollection<TestExecution> TestExecutions { get; set; } = new List<TestExecution>();
}

public class TestCase : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TestType TestType { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string MethodName { get; set; } = string.Empty;
    public int Priority { get; set; } = 1;
    public int TimeoutSeconds { get; set; } = 300;
    public bool IsParallelizable { get; set; } = true;
    public List<string> Tags { get; set; } = new();
    public Dictionary<string, object> Parameters { get; set; } = new();
    public Dictionary<string, string> Environment { get; set; } = new();
    
    // Foreign keys
    public Guid TestSuiteId { get; set; }
    
    // Navigation properties
    public TestSuite TestSuite { get; set; } = null!;
    public ICollection<TestExecution> TestExecutions { get; set; } = new List<TestExecution>();
    public ICollection<TestStep> TestSteps { get; set; } = new List<TestStep>();
}

public class TestExecution : BaseEntity
{
    public TestStatus Status { get; set; } = TestStatus.Pending;
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public TimeSpan? Duration { get; set; }
    public string? ErrorMessage { get; set; }
    public string? StackTrace { get; set; }
    public Dictionary<string, object> Results { get; set; } = new();
    public List<string> Screenshots { get; set; } = new();
    public List<string> Artifacts { get; set; } = new();
    public ExecutionMode ExecutionMode { get; set; } = ExecutionMode.Sequential;
    
    // Foreign keys
    public Guid TestCaseId { get; set; }
    public Guid? TestSuiteId { get; set; }
    public Guid? TestSessionId { get; set; }
    
    // Navigation properties
    public TestCase TestCase { get; set; } = null!;
    public TestSuite? TestSuite { get; set; }
    public TestSession? TestSession { get; set; }
    public ICollection<TestStepExecution> TestStepExecutions { get; set; } = new List<TestStepExecution>();
}

public class TestStep : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; }
    public string Action { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
    public Dictionary<string, object> ExpectedResults { get; set; } = new();
    
    // Foreign keys
    public Guid TestCaseId { get; set; }
    
    // Navigation properties
    public TestCase TestCase { get; set; } = null!;
    public ICollection<TestStepExecution> TestStepExecutions { get; set; } = new List<TestStepExecution>();
}

public class TestStepExecution : BaseEntity
{
    public TestStatus Status { get; set; } = TestStatus.Pending;
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public TimeSpan? Duration { get; set; }
    public Dictionary<string, object> ActualResults { get; set; } = new();
    public string? ErrorMessage { get; set; }
    public List<string> Screenshots { get; set; } = new();
    public List<string> Logs { get; set; } = new();
    
    // Foreign keys
    public Guid TestStepId { get; set; }
    public Guid TestExecutionId { get; set; }
    
    // Navigation properties
    public TestStep TestStep { get; set; } = null!;
    public TestExecution TestExecution { get; set; } = null!;
}

public class TestSession : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TestStatus Status { get; set; } = TestStatus.Pending;
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public TimeSpan? Duration { get; set; }
    public ExecutionMode ExecutionMode { get; set; } = ExecutionMode.Sequential;
    public int MaxParallelTests { get; set; } = 4;
    public Dictionary<string, string> Environment { get; set; } = new();
    public Dictionary<string, object> Configuration { get; set; } = new();
    
    // Navigation properties
    public ICollection<TestExecution> TestExecutions { get; set; } = new List<TestExecution>();
    public ICollection<TestReport> TestReports { get; set; } = new List<TestReport>();
}