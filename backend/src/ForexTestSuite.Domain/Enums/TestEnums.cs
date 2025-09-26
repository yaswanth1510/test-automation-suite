namespace ForexTestSuite.Domain.Enums;

public enum TestStatus
{
    Pending,
    Running,
    Passed,
    Failed,
    Skipped,
    Cancelled
}

public enum TestType
{
    Unit,
    Integration,
    EndToEnd,
    Api,
    UI,
    Performance,
    Load,
    Security,
    ForexTrading,
    MarketDataValidation,
    RiskManagement,
    ComplianceValidation
}

public enum ExecutionMode
{
    Sequential,
    Parallel,
    Distributed
}

public enum EnvironmentType
{
    Development,
    Testing,
    Staging,
    Production,
    Sandbox
}

public enum ReportFormat
{
    Html,
    Json,
    Xml,
    Pdf,
    Excel
}

public enum LogLevel
{
    Trace,
    Debug,
    Information,
    Warning,
    Error,
    Critical
}