using NUnit.Framework;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using ForexTestFramework.Core;
using ForexTestFramework.Clients;
using ForexTestFramework.Modules;

namespace ForexTestFramework.Tests;

/// <summary>
/// Sample integration tests demonstrating the framework capabilities
/// </summary>
[TestFixture]
public class SampleForexTradingTests
{
    private IServiceProvider _serviceProvider = null!;
    private ILogger<SampleForexTradingTests> _logger = null!;
    private ForexTradingTestModule _tradingModule = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        // Setup dependency injection
        var services = new ServiceCollection();
        
        // Add logging
        services.AddLogging(builder => builder.AddConsole().AddDebug());
        
        // Add HTTP clients
        services.AddScoped<HttpTestClient>(provider => 
            new HttpTestClient("https://api.example-forex.com/v1", 
                provider.GetRequiredService<ILogger<HttpTestClient>>()));
        
        services.AddScoped<ForexApiClient>(provider => 
            new ForexApiClient("https://api.example-forex.com/v1", 
                provider.GetRequiredService<ILogger<HttpTestClient>>()));
        
        // Add database client
        services.AddScoped<SqlTestClient>(provider => 
            new SqlTestClient("Server=localhost,1433;Database=ForexTestSuite;User Id=sa;Password=TestPassword123!;TrustServerCertificate=true", 
                provider.GetRequiredService<ILogger<SqlTestClient>>()));
        
        services.AddScoped<ForexDatabaseClient>(provider => 
            new ForexDatabaseClient("Server=localhost,1433;Database=ForexTestSuite;User Id=sa;Password=TestPassword123!;TrustServerCertificate=true", 
                provider.GetRequiredService<ILogger<SqlTestClient>>()));
        
        // Add framework components
        services.AddScoped<StepManager>();
        services.AddScoped<ParameterGenerator>();
        services.AddScoped<ForexParameterGenerator>();
        services.AddScoped<ComparisonTool>();
        
        // Add test modules
        services.AddScoped<ForexTradingTestModule>();
        
        _serviceProvider = services.BuildServiceProvider();
        _logger = _serviceProvider.GetRequiredService<ILogger<SampleForexTradingTests>>();
        _tradingModule = _serviceProvider.GetRequiredService<ForexTradingTestModule>();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _serviceProvider?.Dispose();
    }

    [Test, Category("Sample")]
    [Description("Sample test demonstrating framework usage")]
    public async Task SampleMarketDataTest()
    {
        _logger.LogInformation("Running sample market data test");
        
        var stepManager = _serviceProvider.GetRequiredService<StepManager>();
        var parameterGenerator = _serviceProvider.GetRequiredService<ForexParameterGenerator>();
        var httpClient = _serviceProvider.GetRequiredService<ForexApiClient>();

        // Generate test parameters
        var parameters = parameterGenerator.GenerateParameters(new Dictionary<string, ParameterConfig>
        {
            ["currencyPair"] = new() 
            { 
                Type = "currency_pair",
                Description = "Random currency pair for testing"
            },
            ["expectedSpread"] = new()
            {
                Type = "decimal",
                Configuration = new Dictionary<string, object> { ["min"] = 0.0001, ["max"] = 0.01, ["decimals"] = 5 },
                Description = "Expected maximum spread for the pair"
            }
        });

        // Register test steps
        stepManager.RegisterStep("GetMarketData", "Retrieve market data for currency pair", async (stepParams) =>
        {
            var pair = stepParams["currencyPair"].ToString()!;
            var result = await httpClient.GetMarketDataAsync(pair);
            
            if (result.IsSuccess && result.Data != null)
            {
                return StepResult.Success($"Retrieved market data for {pair}", new Dictionary<string, object>
                {
                    ["bid"] = result.Data.Bid,
                    ["ask"] = result.Data.Ask,
                    ["spread"] = result.Data.Spread,
                    ["timestamp"] = result.Data.Timestamp
                });
            }
            
            return StepResult.Failed($"Failed to get market data for {pair}: {result.ErrorMessage}");
        });

        stepManager.RegisterStep("ValidateSpread", "Validate that spread is within acceptable limits", async (stepParams) =>
        {
            var spread = (decimal)stepParams["spread"];
            var maxSpread = (decimal)stepParams["expectedSpread"];
            
            if (spread <= maxSpread)
            {
                return StepResult.Success($"Spread {spread:F5} is within limit {maxSpread:F5}");
            }
            
            return StepResult.Failed($"Spread {spread:F5} exceeds limit {maxSpread:F5}", 
                stopOnFailure: false); // Continue with other validations
        });

        stepManager.RegisterStep("ValidatePrices", "Validate bid/ask price relationship", async (stepParams) =>
        {
            var bid = (decimal)stepParams["bid"];
            var ask = (decimal)stepParams["ask"];
            
            if (ask > bid && bid > 0 && ask > 0)
            {
                return StepResult.Success($"Price validation passed - Bid: {bid:F5}, Ask: {ask:F5}");
            }
            
            return StepResult.Failed($"Invalid price relationship - Bid: {bid:F5}, Ask: {ask:F5}");
        });

        // Execute test steps
        var stepIds = new List<string> { "GetMarketData", "ValidateSpread", "ValidatePrices" };
        var results = await stepManager.ExecuteStepsAsync(stepIds, parameters);

        // Assert results
        Assert.That(results.Count, Is.EqualTo(3), "Should execute all 3 steps");
        Assert.That(results[0].IsSuccess, Is.True, $"Step 1 failed: {results[0].Message}");
        Assert.That(results.All(r => r.IsSuccess), Is.True, 
            $"Some steps failed: {string.Join(", ", results.Where(r => !r.IsSuccess).Select(r => r.Message))}");

        // Log execution history
        var history = stepManager.GetExecutionHistory();
        foreach (var execution in history.TakeLast(3))
        {
            _logger.LogInformation("Step: {Step}, Duration: {Duration}ms, Success: {Success}", 
                execution.StepName, execution.Duration?.TotalMilliseconds, execution.Success);
        }
    }

    [Test, Category("Sample")]
    [Description("Sample data validation test")]
    public async Task SampleDataValidationTest()
    {
        _logger.LogInformation("Running sample data validation test");

        var comparisonTool = _serviceProvider.GetRequiredService<ComparisonTool>();
        
        // Test JSON comparison
        var expectedJson = """
        {
            "currencyPair": "EUR/USD",
            "bid": 1.1000,
            "ask": 1.1005,
            "timestamp": "2023-01-01T12:00:00Z"
        }
        """;

        var actualJson = """
        {
            "currencyPair": "EUR/USD", 
            "bid": 1.1000,
            "ask": 1.1005,
            "timestamp": "2023-01-01T12:00:00Z",
            "volume": 1000000
        }
        """;

        var jsonComparison = comparisonTool.CompareJson(expectedJson, actualJson, new ComparisonOptions
        {
            IgnoreProperties = new List<string> { "volume" } // Ignore volume field
        });

        Assert.That(jsonComparison.IsMatch, Is.True, 
            $"JSON comparison failed: {string.Join(", ", jsonComparison.Differences)}");

        // Test numeric comparison with tolerance
        var expectedPrice = 1.10050m;
        var actualPrice = 1.10052m;
        var tolerance = 0.00005m;

        var numericComparison = comparisonTool.CompareNumeric(expectedPrice, actualPrice, tolerance);
        Assert.That(numericComparison.IsMatch, Is.True, numericComparison.Message);

        _logger.LogInformation("Data validation test completed successfully");
    }

    [Test, Category("Sample")]
    [Description("Sample parameter generation test")]
    public void SampleParameterGenerationTest()
    {
        _logger.LogInformation("Running sample parameter generation test");

        var parameterGenerator = _serviceProvider.GetRequiredService<ForexParameterGenerator>();

        // Generate various forex parameters
        var currencyPair = parameterGenerator.GenerateParameter("currency_pair");
        var tradeAmount = parameterGenerator.GenerateParameter("trade_amount", new Dictionary<string, object> 
        { 
            ["min"] = 1000, 
            ["max"] = 50000 
        });
        var leverage = parameterGenerator.GenerateParameter("leverage");
        var marketCondition = parameterGenerator.GenerateParameter("market_condition");

        Assert.That(currencyPair, Is.Not.Null.And.Not.Empty);
        Assert.That(tradeAmount, Is.InstanceOf<decimal>().And.GreaterThan(999));
        Assert.That(leverage, Is.InstanceOf<int>().And.GreaterThan(0));
        Assert.That(marketCondition, Is.Not.Null.And.Not.Empty);

        _logger.LogInformation("Generated parameters - Pair: {Pair}, Amount: {Amount}, Leverage: {Leverage}, Market: {Market}", 
            currencyPair, tradeAmount, leverage, marketCondition);

        // Generate parameter sets for data-driven testing
        var parameterConfigs = new Dictionary<string, ParameterConfig>
        {
            ["pair"] = new() { Type = "currency_pair" },
            ["side"] = new() { Type = "string" }, // Will generate random string
            ["amount"] = new() { Type = "trade_amount" }
        };

        var parameterSets = parameterGenerator.GenerateParameterSets(parameterConfigs, count: 5);
        
        Assert.That(parameterSets.Count, Is.EqualTo(5));
        Assert.That(parameterSets.All(ps => ps.ContainsKey("pair")), Is.True);
        Assert.That(parameterSets.All(ps => ps.ContainsKey("amount")), Is.True);

        foreach (var (index, paramSet) in parameterSets.Select((ps, i) => (i, ps)))
        {
            _logger.LogInformation("Parameter Set {Index}: {Parameters}", 
                index + 1, string.Join(", ", paramSet.Select(kv => $"{kv.Key}={kv.Value}")));
        }
    }
}

/// <summary>
/// Performance tests demonstrating framework capabilities under load
/// </summary>
[TestFixture, Category("Performance")]
public class SamplePerformanceTests
{
    [Test]
    [Description("Concurrent parameter generation performance test")]
    public async Task ConcurrentParameterGenerationTest()
    {
        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .AddScoped<ForexParameterGenerator>()
            .BuildServiceProvider();

        var parameterGenerator = serviceProvider.GetRequiredService<ForexParameterGenerator>();
        var tasks = new List<Task>();

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Generate 1000 parameter sets concurrently
        for (int i = 0; i < 100; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                var configs = new Dictionary<string, ParameterConfig>
                {
                    ["pair"] = new() { Type = "currency_pair" },
                    ["amount"] = new() { Type = "trade_amount" },
                    ["price"] = new() { Type = "forex_price" }
                };
                
                var parameters = parameterGenerator.GenerateParameterSets(configs, 10);
                Assert.That(parameters.Count, Is.EqualTo(10));
            }));
        }

        await Task.WhenAll(tasks);
        stopwatch.Stop();

        // Should complete within reasonable time
        Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(5000), 
            $"Parameter generation took {stopwatch.ElapsedMilliseconds}ms, expected < 5000ms");

        Console.WriteLine($"Generated 1000 parameter sets in {stopwatch.ElapsedMilliseconds}ms");
    }
}