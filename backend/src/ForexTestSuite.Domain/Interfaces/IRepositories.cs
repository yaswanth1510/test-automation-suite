using ForexTestSuite.Domain.Entities;
using System.Linq.Expressions;

namespace ForexTestSuite.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
}

public interface ITestSuiteRepository : IRepository<TestSuite>
{
    Task<IEnumerable<TestSuite>> GetActiveTestSuitesAsync(CancellationToken cancellationToken = default);
    Task<TestSuite?> GetWithTestCasesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TestSuite>> SearchByTagsAsync(IEnumerable<string> tags, CancellationToken cancellationToken = default);
}

public interface ITestCaseRepository : IRepository<TestCase>
{
    Task<IEnumerable<TestCase>> GetByTestSuiteIdAsync(Guid testSuiteId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TestCase>> GetByTypeAsync(TestType testType, CancellationToken cancellationToken = default);
    Task<IEnumerable<TestCase>> GetParallelizableTestsAsync(CancellationToken cancellationToken = default);
    Task<TestCase?> GetWithStepsAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ITestExecutionRepository : IRepository<TestExecution>
{
    Task<IEnumerable<TestExecution>> GetByTestCaseIdAsync(Guid testCaseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TestExecution>> GetByStatusAsync(TestStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<TestExecution>> GetRunningExecutionsAsync(CancellationToken cancellationToken = default);
    Task<TestExecution?> GetWithStepExecutionsAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ITestSessionRepository : IRepository<TestSession>
{
    Task<IEnumerable<TestSession>> GetActiveSessionsAsync(CancellationToken cancellationToken = default);
    Task<TestSession?> GetWithExecutionsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TestSession>> GetRecentSessionsAsync(int count, CancellationToken cancellationToken = default);
}

public interface ITestReportRepository : IRepository<TestReport>
{
    Task<IEnumerable<TestReport>> GetByFormatAsync(ReportFormat format, CancellationToken cancellationToken = default);
    Task<IEnumerable<TestReport>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
}

public interface IUnitOfWork : IDisposable
{
    ITestSuiteRepository TestSuites { get; }
    ITestCaseRepository TestCases { get; }
    ITestExecutionRepository TestExecutions { get; }
    ITestSessionRepository TestSessions { get; }
    ITestReportRepository TestReports { get; }
    IRepository<TestEnvironment> TestEnvironments { get; }
    IRepository<TestConfiguration> TestConfigurations { get; }
    IRepository<TestAsset> TestAssets { get; }
    IRepository<ForexTradingTest> ForexTradingTests { get; }
    IRepository<MarketDataSimulation> MarketDataSimulations { get; }
    IRepository<TradingOrderTest> TradingOrderTests { get; }
    IRepository<RiskManagementTest> RiskManagementTests { get; }
    IRepository<ComplianceTest> ComplianceTests { get; }
    IRepository<FinancialCalculationTest> FinancialCalculationTests { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}