using Microsoft.EntityFrameworkCore;
using ForexTestSuite.Domain.Entities;
using System.Text.Json;

namespace ForexTestSuite.Infrastructure.Data.Context;

public class ForexTestSuiteDbContext : DbContext
{
    public ForexTestSuiteDbContext(DbContextOptions<ForexTestSuiteDbContext> options) : base(options)
    {
    }

    // Test Management Tables
    public DbSet<TestSuite> TestSuites { get; set; } = null!;
    public DbSet<TestCase> TestCases { get; set; } = null!;
    public DbSet<TestExecution> TestExecutions { get; set; } = null!;
    public DbSet<TestStep> TestSteps { get; set; } = null!;
    public DbSet<TestStepExecution> TestStepExecutions { get; set; } = null!;
    public DbSet<TestSession> TestSessions { get; set; } = null!;
    public DbSet<TestReport> TestReports { get; set; } = null!;
    public DbSet<TestEnvironment> TestEnvironments { get; set; } = null!;
    public DbSet<TestConfiguration> TestConfigurations { get; set; } = null!;
    public DbSet<TestAsset> TestAssets { get; set; } = null!;

    // Forex-specific Tables
    public DbSet<ForexTradingTest> ForexTradingTests { get; set; } = null!;
    public DbSet<MarketDataSimulation> MarketDataSimulations { get; set; } = null!;
    public DbSet<TradingOrderTest> TradingOrderTests { get; set; } = null!;
    public DbSet<RiskManagementTest> RiskManagementTests { get; set; } = null!;
    public DbSet<ComplianceTest> ComplianceTests { get; set; } = null!;
    public DbSet<FinancialCalculationTest> FinancialCalculationTests { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure JSON properties
        ConfigureJsonProperties(modelBuilder);

        // Configure relationships
        ConfigureRelationships(modelBuilder);

        // Configure indexes
        ConfigureIndexes(modelBuilder);

        // Seed data
        SeedData(modelBuilder);
    }

    private static void ConfigureJsonProperties(ModelBuilder modelBuilder)
    {
        // TestSuite JSON properties
        modelBuilder.Entity<TestSuite>()
            .Property(e => e.Tags)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>());

        modelBuilder.Entity<TestSuite>()
            .Property(e => e.Configuration)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>());

        // TestCase JSON properties
        modelBuilder.Entity<TestCase>()
            .Property(e => e.Tags)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>());

        modelBuilder.Entity<TestCase>()
            .Property(e => e.Parameters)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>());

        modelBuilder.Entity<TestCase>()
            .Property(e => e.Environment)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, string>());

        modelBuilder.Entity<ComplianceTest>()
            .Property(e => e.ExpectedResults)
            .HasConversion(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>());

        modelBuilder.Entity<ComplianceTest>()
            .Property(e => e.TestData)
            .HasConversion(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>());

        modelBuilder.Entity<ComplianceTest>()
            .Property(e => e.ValidationCriteria)
            .HasConversion(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>());

        modelBuilder.Entity<FinancialCalculationTest>()
            .Property(e => e.CalculationParameters)
            .HasConversion(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>());

        modelBuilder.Entity<MarketDataSimulation>()
            .Property(e => e.PriceMovements)
            .HasConversion(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>());

        modelBuilder.Entity<MarketDataSimulation>()
            .Property(e => e.NewsEvents)
            .HasConversion(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>());

        modelBuilder.Entity<RiskManagementTest>()
            .Property(e => e.RiskRules)
            .HasConversion(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>());

        modelBuilder.Entity<RiskManagementTest>()
            .Property(e => e.ExpectedOutcomes)
            .HasConversion(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>());

        modelBuilder.Entity<TestAsset>()
            .Property(e => e.Metadata)
            .HasConversion(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>());

        // TestReport JSON properties
        modelBuilder.Entity<TestReport>()
            .Property(e => e.Statistics)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>());

        modelBuilder.Entity<TestReport>()
            .Property(e => e.Metadata)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>());

        // TestConfiguration JSON properties
        modelBuilder.Entity<TestConfiguration>()
            .Property(e => e.Settings)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>());
        modelBuilder.Entity<TestConfiguration>()
            .Property(e => e.BrowserSettings)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>());
        modelBuilder.Entity<TestConfiguration>()
            .Property(e => e.ApiSettings)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>());
        modelBuilder.Entity<TestConfiguration>()
            .Property(e => e.DatabaseSettings)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>());

        modelBuilder.Entity<TestConfiguration>()
            .Property(e => e.ConnectionStrings)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, string>());

        // TestStep JSON properties
        modelBuilder.Entity<TestStep>()
            .Property(e => e.Parameters)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>());
        modelBuilder.Entity<TestStep>()
            .Property(e => e.ExpectedResults)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>());

        // TestStepExecution JSON properties
        modelBuilder.Entity<TestStepExecution>()
            .Property(e => e.ActualResults)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>());

        // TestSession JSON properties
        modelBuilder.Entity<TestSession>()
            .Property(e => e.Configuration)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>());
        modelBuilder.Entity<TestSession>()
            .Property(e => e.Environment)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, string>());

        modelBuilder.Entity<TradingOrderTest>()
            .Property(e => e.ValidationRules)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>());

        // Continue with other entities...
        ConfigureTestExecutionJsonProperties(modelBuilder);
        ConfigureConfigurationJsonProperties(modelBuilder);
        ConfigureForexJsonProperties(modelBuilder);
    }

    private static void ConfigureTestExecutionJsonProperties(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestExecution>()
            .Property(e => e.Results)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>());

        modelBuilder.Entity<TestExecution>()
            .Property(e => e.Screenshots)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>());

        modelBuilder.Entity<TestExecution>()
            .Property(e => e.Artifacts)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>());
    }

    private static void ConfigureConfigurationJsonProperties(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestEnvironment>()
            .Property(e => e.Variables)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, string>());

        modelBuilder.Entity<TestEnvironment>()
            .Property(e => e.Configuration)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>());
    }

    private static void ConfigureForexJsonProperties(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ForexTradingTest>()
            .Property(e => e.TradingParameters)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>());

        modelBuilder.Entity<ForexTradingTest>()
            .Property(e => e.RiskParameters)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>());
    }

    private static void ConfigureRelationships(ModelBuilder modelBuilder)
    {
        // TestSuite -> TestCase (One-to-Many)
        modelBuilder.Entity<TestCase>()
            .HasOne(tc => tc.TestSuite)
            .WithMany(ts => ts.TestCases)
            .HasForeignKey(tc => tc.TestSuiteId)
            .OnDelete(DeleteBehavior.Cascade);

        // TestCase -> TestStep (One-to-Many)
        modelBuilder.Entity<TestStep>()
            .HasOne(ts => ts.TestCase)
            .WithMany(tc => tc.TestSteps)
            .HasForeignKey(ts => ts.TestCaseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Other relationships...
    }

    private static void ConfigureIndexes(ModelBuilder modelBuilder)
    {
        // TestSuite indexes
        modelBuilder.Entity<TestSuite>()
            .HasIndex(ts => ts.Name);

        modelBuilder.Entity<TestSuite>()
            .HasIndex(ts => ts.IsActive);

        // TestCase indexes
        modelBuilder.Entity<TestCase>()
            .HasIndex(tc => tc.TestSuiteId);

        modelBuilder.Entity<TestCase>()
            .HasIndex(tc => tc.TestType);

        // TestExecution indexes
        modelBuilder.Entity<TestExecution>()
            .HasIndex(te => te.Status);

        modelBuilder.Entity<TestExecution>()
            .HasIndex(te => te.TestCaseId);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Seed default test environment
        modelBuilder.Entity<TestEnvironment>().HasData(
            new TestEnvironment
            {
                Id = Guid.NewGuid(),
                Name = "Development",
                Description = "Default development environment",
                Type = Domain.Enums.EnvironmentType.Development,
                BaseUrl = "http://localhost:3000",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        );
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<Domain.Common.BaseEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}