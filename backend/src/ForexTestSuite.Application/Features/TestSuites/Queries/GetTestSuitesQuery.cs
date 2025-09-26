using ForexTestSuite.Application.Common;
using ForexTestSuite.Domain.Entities;

namespace ForexTestSuite.Application.Features.TestSuites.Queries;

public class GetTestSuitesQuery : IQuery<Result<IEnumerable<TestSuiteDto>>>
{
    public bool ActiveOnly { get; set; } = true;
    public List<string>? Tags { get; set; }
}

public class GetTestSuiteByIdQuery : IQuery<Result<TestSuiteDto>>
{
    public Guid Id { get; set; }
    public bool IncludeTestCases { get; set; } = true;
}

public class TestSuiteDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<string> Tags { get; set; } = new();
    public Dictionary<string, object> Configuration { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public List<TestCaseDto>? TestCases { get; set; }
}

public class TestCaseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TestType { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string MethodName { get; set; } = string.Empty;
    public int Priority { get; set; }
    public int TimeoutSeconds { get; set; }
    public bool IsParallelizable { get; set; }
    public List<string> Tags { get; set; } = new();
    public Dictionary<string, object> Parameters { get; set; } = new();
    public Dictionary<string, string> Environment { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}