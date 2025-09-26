using Microsoft.EntityFrameworkCore.Storage;
using ForexTestSuite.Domain.Interfaces;
using ForexTestSuite.Domain.Entities;
using ForexTestSuite.Infrastructure.Data.Context;
using ForexTestSuite.Infrastructure.Data.Repositories;

namespace ForexTestSuite.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly ForexTestSuiteDbContext _context;
    private IDbContextTransaction? _transaction;

    // Repositories
    private ITestSuiteRepository? _testSuites;
    private ITestCaseRepository? _testCases;
    private ITestExecutionRepository? _testExecutions;
    private ITestSessionRepository? _testSessions;
    private ITestReportRepository? _testReports;
    private IRepository<TestEnvironment>? _testEnvironments;
    private IRepository<TestConfiguration>? _testConfigurations;
    private IRepository<TestAsset>? _testAssets;
    private IRepository<ForexTradingTest>? _forexTradingTests;
    private IRepository<MarketDataSimulation>? _marketDataSimulations;
    private IRepository<TradingOrderTest>? _tradingOrderTests;
    private IRepository<RiskManagementTest>? _riskManagementTests;
    private IRepository<ComplianceTest>? _complianceTests;
    private IRepository<FinancialCalculationTest>? _financialCalculationTests;

    public UnitOfWork(ForexTestSuiteDbContext context)
    {
        _context = context;
    }

    public ITestSuiteRepository TestSuites =>
        _testSuites ??= new TestSuiteRepository(_context);

    public ITestCaseRepository TestCases =>
        _testCases ??= new TestCaseRepository(_context);

    public ITestExecutionRepository TestExecutions =>
        _testExecutions ??= new TestExecutionRepository(_context);

    public ITestSessionRepository TestSessions =>
        _testSessions ??= new TestSessionRepository(_context);

    public ITestReportRepository TestReports =>
        _testReports ??= new TestReportRepository(_context);

    public IRepository<TestEnvironment> TestEnvironments =>
        _testEnvironments ??= new Repository<TestEnvironment>(_context);

    public IRepository<TestConfiguration> TestConfigurations =>
        _testConfigurations ??= new Repository<TestConfiguration>(_context);

    public IRepository<TestAsset> TestAssets =>
        _testAssets ??= new Repository<TestAsset>(_context);

    public IRepository<ForexTradingTest> ForexTradingTests =>
        _forexTradingTests ??= new Repository<ForexTradingTest>(_context);

    public IRepository<MarketDataSimulation> MarketDataSimulations =>
        _marketDataSimulations ??= new Repository<MarketDataSimulation>(_context);

    public IRepository<TradingOrderTest> TradingOrderTests =>
        _tradingOrderTests ??= new Repository<TradingOrderTest>(_context);

    public IRepository<RiskManagementTest> RiskManagementTests =>
        _riskManagementTests ??= new Repository<RiskManagementTest>(_context);

    public IRepository<ComplianceTest> ComplianceTests =>
        _complianceTests ??= new Repository<ComplianceTest>(_context);

    public IRepository<FinancialCalculationTest> FinancialCalculationTests =>
        _financialCalculationTests ??= new Repository<FinancialCalculationTest>(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}