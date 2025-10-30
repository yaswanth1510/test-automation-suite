using NUnit.Framework;
using Microsoft.Extensions.Logging;
using ForexTestFramework.Core;
using ForexTestFramework.Clients;

namespace ForexTestFramework.Modules;

/// <summary>
/// Forex-specific test module for trading application testing
/// </summary>
public class ForexTradingTestModule
{
    private readonly ILogger<ForexTradingTestModule> _logger;
    private readonly ForexApiClient _apiClient;
    private readonly ForexDatabaseClient _dbClient;
    private readonly ParameterGenerator _parameterGenerator;
    private readonly ComparisonTool _comparisonTool;

    public ForexTradingTestModule(
        ILogger<ForexTradingTestModule> logger,
        ForexApiClient apiClient,
        ForexDatabaseClient dbClient,
        ParameterGenerator parameterGenerator,
        ComparisonTool comparisonTool)
    {
        _logger = logger;
        _apiClient = apiClient;
        _dbClient = dbClient;
        _parameterGenerator = parameterGenerator;
        _comparisonTool = comparisonTool;
    }

    /// <summary>
    /// Test market data retrieval for major currency pairs
    /// </summary>
    [Test, Category("MarketData")]
    public async Task TestMarketDataRetrieval()
    {
        _logger.LogInformation("Testing market data retrieval for major currency pairs");

        var currencyPairs = new[] { "EUR/USD", "GBP/USD", "USD/JPY", "AUD/USD" };

        foreach (var pair in currencyPairs)
        {
            var result = await _apiClient.GetMarketDataAsync(pair);
            
            Assert.That(result.IsSuccess, Is.True, $"Failed to get market data for {pair}: {result.ErrorMessage}");
            Assert.That(result.Data, Is.Not.Null, $"Market data is null for {pair}");
            Assert.That(result.Data.Bid, Is.GreaterThan(0), $"Invalid bid price for {pair}");
            Assert.That(result.Data.Ask, Is.GreaterThan(0), $"Invalid ask price for {pair}");
            Assert.That(result.Data.Ask, Is.GreaterThan(result.Data.Bid), $"Ask price should be higher than bid for {pair}");
            Assert.That(result.Data.Spread, Is.LessThan(0.01m), $"Spread too high for {pair}");

            _logger.LogInformation("Market data validation passed for {Pair} - Bid: {Bid}, Ask: {Ask}, Spread: {Spread}", 
                pair, result.Data.Bid, result.Data.Ask, result.Data.Spread);
        }
    }

    /// <summary>
    /// Test order placement with various order types
    /// </summary>
    [Test, Category("OrderManagement")]
    [TestCase("EUR/USD", "Market", "Buy", 10000)]
    [TestCase("GBP/USD", "Limit", "Sell", 5000)]
    [TestCase("USD/JPY", "Stop", "Buy", 15000)]
    public async Task TestOrderPlacement(string currencyPair, string orderType, string side, decimal amount)
    {
        _logger.LogInformation("Testing {OrderType} order placement for {Pair}", orderType, currencyPair);

        // Generate test parameters
        var parameters = _parameterGenerator.GenerateParameters(new Dictionary<string, ParameterConfig>
        {
            ["price"] = new() { Type = "forex_price", Configuration = new Dictionary<string, object> { ["pair"] = currencyPair } },
            ["stopLoss"] = new() { Type = "decimal", Configuration = new Dictionary<string, object> { ["min"] = 1.0000m, ["max"] = 2.0000m, ["decimals"] = 5 } },
            ["takeProfit"] = new() { Type = "decimal", Configuration = new Dictionary<string, object> { ["min"] = 1.0000m, ["max"] = 2.0000m, ["decimals"] = 5 } }
        });

        var orderRequest = new OrderRequest
        {
            CurrencyPair = currencyPair,
            OrderType = orderType,
            Side = side,
            Amount = amount,
            Price = orderType != "Market" ? (decimal)parameters["price"] : null,
            StopLoss = (decimal)parameters["stopLoss"],
            TakeProfit = (decimal)parameters["takeProfit"]
        };

        var result = await _apiClient.PlaceOrderAsync(orderRequest);
        
        Assert.That(result.IsSuccess, Is.True, $"Order placement failed: {result.ErrorMessage}");
        Assert.That(result.Data, Is.Not.Null, "Order response is null");
        Assert.That(result.Data.OrderId, Is.Not.Empty, "Order ID is empty");
        Assert.That(result.Data.Status, Is.Not.Empty, "Order status is empty");
        
        if (orderType == "Market")
        {
            Assert.That(result.Data.Status, Is.EqualTo("Filled"), "Market order should be filled immediately");
            Assert.That(result.Data.ExecutedAmount, Is.EqualTo(amount), "Executed amount should match requested amount");
        }

        _logger.LogInformation("Order placement successful - OrderId: {OrderId}, Status: {Status}, ExecutedPrice: {Price}", 
            result.Data.OrderId, result.Data.Status, result.Data.ExecutedPrice);
    }

    /// <summary>
    /// Test account balance calculations and validations
    /// </summary>
    [Test, Category("AccountManagement")]
    public async Task TestAccountBalanceValidation()
    {
        _logger.LogInformation("Testing account balance validation");

        var accountId = _parameterGenerator.GenerateParameter("string", 
            new Dictionary<string, object> { ["prefix"] = "ACC", ["length"] = 10 }).ToString();

        // Get account balance from API
        var apiResult = await _apiClient.GetAccountBalanceAsync(accountId!);
        Assert.That(apiResult.IsSuccess, Is.True, $"Failed to get account balance: {apiResult.ErrorMessage}");

        // Validate balance in database
        var dbResult = await _dbClient.ValidateAccountBalanceAsync(accountId!, apiResult.Data!.Balance);
        Assert.That(dbResult.IsSuccess, Is.True, "Database balance validation failed");
        Assert.That(dbResult.Data, Is.True, "Account balance mismatch between API and database");

        // Validate margin calculations
        var balance = apiResult.Data;
        var totalMargin = balance!.AvailableMargin + balance.UsedMargin;
        var expectedTotalMargin = balance.Balance + balance.UnrealizedPnL;
        
        var marginComparison = _comparisonTool.CompareNumeric(expectedTotalMargin, totalMargin, 0.01m);
        Assert.That(marginComparison.IsMatch, Is.True, 
            $"Margin calculation error: {marginComparison.Message}");

        _logger.LogInformation("Account balance validation passed - Balance: {Balance}, Available Margin: {Available}, Used Margin: {Used}", 
            balance.Balance, balance.AvailableMargin, balance.UsedMargin);
    }

    /// <summary>
    /// Test P&L calculations for open positions
    /// </summary>
    [Test, Category("RiskManagement")]
    public async Task TestPnLCalculations()
    {
        _logger.LogInformation("Testing P&L calculations for open positions");

        var accountId = "TEST_ACC_001";

        // Get open positions
        var positionsResult = await _dbClient.GetTradingPositionsAsync(accountId, openOnly: true);
        Assert.That(positionsResult.IsSuccess, Is.True, "Failed to retrieve trading positions");

        if (positionsResult.Data.Rows.Count == 0)
        {
            Assert.Ignore("No open positions found for testing");
            return;
        }

        foreach (System.Data.DataRow position in positionsResult.Data.Rows)
        {
            var positionId = position["PositionId"].ToString()!;
            var validationResult = await _dbClient.ValidatePnLCalculationAsync(positionId);

            Assert.That(validationResult.IsSuccess, Is.True, 
                $"P&L validation failed for position {positionId}: {validationResult.Message}");
            Assert.That(validationResult.Data, Is.True, validationResult.Message);

            _logger.LogInformation("P&L calculation validated for position {PositionId}", positionId);
        }
    }

    /// <summary>
    /// Test risk management rules and limits
    /// </summary>
    [Test, Category("RiskManagement")]
    public async Task TestRiskManagementRules()
    {
        _logger.LogInformation("Testing risk management rules and limits");

        var accountId = "TEST_ACC_001";
        
        // Test maximum leverage
        var maxLeverage = 100m;
        var testAmount = 1000000m; // 1M units
        
        var orderRequest = new OrderRequest
        {
            CurrencyPair = "EUR/USD",
            OrderType = "Market",
            Side = "Buy",
            Amount = testAmount
        };

        // This should fail due to leverage limits
        var result = await _apiClient.PlaceOrderAsync(orderRequest);
        Assert.That(result.IsSuccess, Is.False, "Order should be rejected due to leverage limits");
        
        // Test position size limits
        var validationRules = new List<DataValidationRule>
        {
            new()
            {
                Name = "MaxPositionSize",
                Query = "SELECT COUNT(*) FROM Positions WHERE AccountId = @accountId AND Amount > 100000",
                ExpectedResult = 0,
                Parameters = new Dictionary<string, object> { ["accountId"] = accountId },
                Description = "No positions should exceed maximum size limit"
            },
            new()
            {
                Name = "MaxOpenPositions",
                Query = "SELECT COUNT(*) FROM Positions WHERE AccountId = @accountId AND Status = 'Open'",
                ExpectedResult = 0, // Adjust based on your limits
                Parameters = new Dictionary<string, object> { ["accountId"] = accountId },
                Description = "Account should not exceed maximum open positions limit"
            }
        };

        var validationResult = await _dbClient.ValidateDataIntegrityAsync(validationRules);
        Assert.That(validationResult.IsSuccess, Is.True, 
            $"Risk management validation failed: {string.Join(", ", validationResult.FailedRules)}");

        _logger.LogInformation("Risk management rules validation completed successfully");
    }

    /// <summary>
    /// Test market volatility scenarios
    /// </summary>
    [Test, Category("MarketSimulation")]
    [TestCase("low")]
    [TestCase("medium")]
    [TestCase("high")]
    [TestCase("extreme")]
    public async Task TestMarketVolatilityScenarios(string volatilityLevel)
    {
        _logger.LogInformation("Testing market volatility scenario: {Level}", volatilityLevel);

        var scenario = (dynamic)_parameterGenerator.GenerateParameter("volatility_scenario", 
            new Dictionary<string, object> { ["level"] = volatilityLevel });

        // Simulate market conditions with the specified volatility
        var currencyPair = "EUR/USD";
        var basePrice = 1.1000m;
        var minVolatility = (decimal)scenario.min;
        var maxVolatility = (decimal)scenario.max;

        // Generate price movements within volatility range
        var priceMovements = new List<decimal>();
        for (int i = 0; i < 100; i++)
        {
            var movement = (decimal)_parameterGenerator.GenerateParameter("decimal", 
                new Dictionary<string, object> 
                { 
                    ["min"] = (double)-maxVolatility, 
                    ["max"] = (double)maxVolatility, 
                    ["decimals"] = 5 
                });
            priceMovements.Add(basePrice + movement);
        }

        // Validate that price movements stay within expected volatility bounds
        var maxPrice = priceMovements.Max();
        var minPrice = priceMovements.Min();
        var actualVolatility = (maxPrice - minPrice) / basePrice;

        Assert.That(actualVolatility, Is.GreaterThanOrEqualTo(minVolatility), 
            $"Actual volatility {actualVolatility} is below minimum {minVolatility}");
        Assert.That(actualVolatility, Is.LessThanOrEqualTo(maxVolatility), 
            $"Actual volatility {actualVolatility} exceeds maximum {maxVolatility}");

        _logger.LogInformation("Volatility scenario {Level} completed - Actual volatility: {Volatility:P2}, Range: {Min:P2}-{Max:P2}", 
            volatilityLevel, actualVolatility, minVolatility, maxVolatility);
    }

    /// <summary>
    /// Test compliance and regulatory requirements
    /// </summary>
    [Test, Category("Compliance")]
    public async Task TestComplianceValidation()
    {
        _logger.LogInformation("Testing compliance and regulatory requirements");

        var accountId = "TEST_ACC_001";

        // KYC validation rules
        var kycRules = new List<DataValidationRule>
        {
            new()
            {
                Name = "KYCStatus",
                Query = "SELECT COUNT(*) FROM Accounts WHERE AccountId = @accountId AND KYCStatus = 'Verified'",
                ExpectedResult = 1,
                Parameters = new Dictionary<string, object> { ["accountId"] = accountId },
                Description = "Account must have verified KYC status"
            },
            new()
            {
                Name = "AMLCheck",
                Query = "SELECT COUNT(*) FROM Accounts WHERE AccountId = @accountId AND AMLStatus = 'Clean'",
                ExpectedResult = 1,
                Parameters = new Dictionary<string, object> { ["accountId"] = accountId },
                Description = "Account must pass AML checks"
            },
            new()
            {
                Name = "TradingLimits",
                Query = "SELECT COUNT(*) FROM TradingLimits WHERE AccountId = @accountId AND Status = 'Active'",
                ExpectedResult = 1,
                Parameters = new Dictionary<string, object> { ["accountId"] = accountId },
                Description = "Account must have active trading limits configured"
            }
        };

        var validationResult = await _dbClient.ValidateDataIntegrityAsync(kycRules);
        Assert.That(validationResult.IsSuccess, Is.True, 
            $"Compliance validation failed: {string.Join(", ", validationResult.FailedRules)}");

        // Test transaction reporting requirements
        var reportingQuery = @"
            SELECT COUNT(*) 
            FROM Transactions t
            LEFT JOIN TransactionReports tr ON t.TransactionId = tr.TransactionId
            WHERE t.AccountId = @accountId 
            AND t.Amount > 10000 
            AND tr.ReportId IS NULL";

        var reportingResult = await _dbClient.ExecuteScalarAsync<int>(reportingQuery, 
            new Dictionary<string, object> { ["accountId"] = accountId });

        Assert.That(reportingResult.IsSuccess, Is.True, "Failed to check transaction reporting");
        Assert.That(reportingResult.Data, Is.EqualTo(0), 
            $"Found {reportingResult.Data} unreported high-value transactions");

        _logger.LogInformation("Compliance validation completed successfully");
    }

    /// <summary>
    /// Performance test for high-frequency trading scenarios
    /// </summary>
    [Test, Category("Performance")]
    public async Task TestHighFrequencyTrading()
    {
        _logger.LogInformation("Testing high-frequency trading performance");

        var currencyPair = "EUR/USD";
        var numberOfOrders = 100;
        var maxLatency = 100; // milliseconds

        var tasks = new List<Task<ApiResponse<OrderResponse>>>();

        // Create multiple concurrent market orders
        for (int i = 0; i < numberOfOrders; i++)
        {
            var orderRequest = new OrderRequest
            {
                CurrencyPair = currencyPair,
                OrderType = "Market",
                Side = i % 2 == 0 ? "Buy" : "Sell",
                Amount = 1000
            };

            tasks.Add(_apiClient.PlaceOrderAsync(orderRequest));
        }

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var results = await Task.WhenAll(tasks);
        stopwatch.Stop();

        var averageLatency = stopwatch.ElapsedMilliseconds / (double)numberOfOrders;
        var successRate = results.Count(r => r.IsSuccess) / (double)numberOfOrders * 100;

        Assert.That(successRate, Is.GreaterThan(95.0), 
            $"Success rate {successRate:F1}% is below acceptable threshold");
        Assert.That(averageLatency, Is.LessThan(maxLatency), 
            $"Average latency {averageLatency:F1}ms exceeds maximum {maxLatency}ms");

        _logger.LogInformation("High-frequency trading test completed - Success Rate: {SuccessRate:F1}%, Average Latency: {Latency:F1}ms", 
            successRate, averageLatency);
    }
}