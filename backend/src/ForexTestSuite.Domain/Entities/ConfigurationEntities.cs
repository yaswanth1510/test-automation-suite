using ForexTestSuite.Domain.Common;
using ForexTestSuite.Domain.Enums;

namespace ForexTestSuite.Domain.Entities;

public class TestReport : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ReportFormat Format { get; set; } = ReportFormat.Html;
    public string FilePath { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
    public Dictionary<string, object> Statistics { get; set; } = new();
    
    // Foreign keys
    public Guid? TestSessionId { get; set; }
    
    // Navigation properties
    public TestSession? TestSession { get; set; }
}

public class TestEnvironment : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public EnvironmentType Type { get; set; } = EnvironmentType.Development;
    public string BaseUrl { get; set; } = string.Empty;
    public Dictionary<string, string> Variables { get; set; } = new();
    public Dictionary<string, object> Configuration { get; set; } = new();
    public bool IsActive { get; set; } = true;
}

public class TestConfiguration : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, object> Settings { get; set; } = new();
    public Dictionary<string, string> ConnectionStrings { get; set; } = new();
    public Dictionary<string, object> BrowserSettings { get; set; } = new();
    public Dictionary<string, object> ApiSettings { get; set; } = new();
    public Dictionary<string, object> DatabaseSettings { get; set; } = new();
    public bool IsDefault { get; set; } = false;
}

public class TestAsset : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
    
    // Foreign keys
    public Guid? TestExecutionId { get; set; }
    public Guid? TestStepExecutionId { get; set; }
    
    // Navigation properties
    public TestExecution? TestExecution { get; set; }
    public TestStepExecution? TestStepExecution { get; set; }
}