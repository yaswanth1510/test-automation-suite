using Microsoft.EntityFrameworkCore;
using ForexTestSuite.Domain.Entities;
using ForexTestSuite.Domain.Interfaces;
using ForexTestSuite.Infrastructure.Data.Context;

namespace ForexTestSuite.Infrastructure.Data.Repositories;

public class TestSuiteRepository : Repository<TestSuite>, ITestSuiteRepository
{
    public TestSuiteRepository(ForexTestSuiteDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TestSuite>> GetActiveTestSuitesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(ts => ts.IsActive)
            .OrderBy(ts => ts.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<TestSuite?> GetWithTestCasesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(ts => ts.TestCases)
            .FirstOrDefaultAsync(ts => ts.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<TestSuite>> SearchByTagsAsync(IEnumerable<string> tags, CancellationToken cancellationToken = default)
    {
        var tagsList = tags.ToList();
        return await _dbSet
            .Where(ts => ts.Tags.Any(tag => tagsList.Contains(tag)))
            .ToListAsync(cancellationToken);
    }
}

public class TestCaseRepository : Repository<TestCase>, ITestCaseRepository
{
    public TestCaseRepository(ForexTestSuiteDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TestCase>> GetByTestSuiteIdAsync(Guid testSuiteId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(tc => tc.TestSuiteId == testSuiteId)
            .OrderBy(tc => tc.Priority)
            .ThenBy(tc => tc.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TestCase>> GetByTypeAsync(Domain.Enums.TestType testType, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(tc => tc.TestType == testType)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TestCase>> GetParallelizableTestsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(tc => tc.IsParallelizable)
            .ToListAsync(cancellationToken);
    }

    public async Task<TestCase?> GetWithStepsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(tc => tc.TestSteps.OrderBy(ts => ts.Order))
            .FirstOrDefaultAsync(tc => tc.Id == id, cancellationToken);
    }
}

public class TestExecutionRepository : Repository<TestExecution>, ITestExecutionRepository
{
    public TestExecutionRepository(ForexTestSuiteDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TestExecution>> GetByTestCaseIdAsync(Guid testCaseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(te => te.TestCaseId == testCaseId)
            .OrderByDescending(te => te.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TestExecution>> GetByStatusAsync(Domain.Enums.TestStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(te => te.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TestExecution>> GetRunningExecutionsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(te => te.Status == Domain.Enums.TestStatus.Running)
            .ToListAsync(cancellationToken);
    }

    public async Task<TestExecution?> GetWithStepExecutionsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(te => te.TestStepExecutions)
                .ThenInclude(tse => tse.TestStep)
            .FirstOrDefaultAsync(te => te.Id == id, cancellationToken);
    }
}

public class TestSessionRepository : Repository<TestSession>, ITestSessionRepository
{
    public TestSessionRepository(ForexTestSuiteDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TestSession>> GetActiveSessionsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(ts => ts.Status == Domain.Enums.TestStatus.Running)
            .ToListAsync(cancellationToken);
    }

    public async Task<TestSession?> GetWithExecutionsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(ts => ts.TestExecutions)
                .ThenInclude(te => te.TestCase)
            .FirstOrDefaultAsync(ts => ts.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<TestSession>> GetRecentSessionsAsync(int count, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .OrderByDescending(ts => ts.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}

public class TestReportRepository : Repository<TestReport>, ITestReportRepository
{
    public TestReportRepository(ForexTestSuiteDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TestReport>> GetByFormatAsync(Domain.Enums.ReportFormat format, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(tr => tr.Format == format)
            .OrderByDescending(tr => tr.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TestReport>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(tr => tr.TestSessionId == sessionId)
            .OrderByDescending(tr => tr.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}