using ForexTestSuite.Application.Common;

namespace ForexTestSuite.Application.Features.TestSuites.Commands;

public class CreateTestSuiteCommand : ICommand<Result<Guid>>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0.0";
    public bool IsActive { get; set; } = true;
    public List<string> Tags { get; set; } = new();
    public Dictionary<string, object> Configuration { get; set; } = new();
}

public class UpdateTestSuiteCommand : ICommand<Result<bool>>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<string> Tags { get; set; } = new();
    public Dictionary<string, object> Configuration { get; set; } = new();
}

public class DeleteTestSuiteCommand : ICommand<Result<bool>>
{
    public Guid Id { get; set; }
}