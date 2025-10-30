using Bogus;
using Microsoft.Extensions.Logging;

namespace ForexTestFramework.Core;

/// <summary>
/// Generates dynamic parameters for test execution
/// </summary>
public class ParameterGenerator
{
    private readonly ILogger<ParameterGenerator> _logger;
    private readonly Dictionary<string, Func<Dictionary<string, object>, object>> _generators = new();

    public ParameterGenerator(ILogger<ParameterGenerator> logger)
    {
        _logger = logger;
        RegisterDefaultGenerators();
    }

    private void RegisterDefaultGenerators()
    {
        // Basic data generators
        RegisterGenerator("string", _ => new Faker().Lorem.Word());
        RegisterGenerator("email", _ => new Faker().Internet.Email());
        RegisterGenerator("name", _ => new Faker().Name.FullName());
        RegisterGenerator("phone", _ => new Faker().Phone.PhoneNumber());
        RegisterGenerator("address", _ => new Faker().Address.FullAddress());
        RegisterGenerator("company", _ => new Faker().Company.CompanyName());
        
        // Numeric generators
        RegisterGenerator("int", config =>
        {
            var min = config.ContainsKey("min") ? Convert.ToInt32(config["min"]) : 1;
            var max = config.ContainsKey("max") ? Convert.ToInt32(config["max"]) : 100;
            return new Faker().Random.Int(min, max);
        });
        
        RegisterGenerator("decimal", config =>
        {
            var min = config.ContainsKey("min") ? Convert.ToDecimal(config["min"]) : 1.0m;
            var max = config.ContainsKey("max") ? Convert.ToDecimal(config["max"]) : 100.0m;
            var decimals = config.ContainsKey("decimals") ? Convert.ToInt32(config["decimals"]) : 2;
            return Math.Round(new Faker().Random.Decimal(min, max), decimals);
        });

        // Date generators
        RegisterGenerator("date", config =>
        {
            var past = config.ContainsKey("past") ? Convert.ToInt32(config["past"]) : 365;
            return new Faker().Date.Past(past);
        });

        RegisterGenerator("future_date", config =>
        {
            var days = config.ContainsKey("days") ? Convert.ToInt32(config["days"]) : 365;
            return new Faker().Date.Future(days);
        });

        // Forex-specific generators
        RegisterForexGenerators();
    }

    private void RegisterForexGenerators()
    {
        var currencyPairs = new[] { "EUR/USD", "GBP/USD", "USD/JPY", "AUD/USD", "USD/CHF", "USD/CAD", "NZD/USD" };
        
        RegisterGenerator("currency_pair", _ => new Faker().PickRandom(currencyPairs));
        
        RegisterGenerator("forex_price", config =>
        {
            var pair = config.ContainsKey("pair") ? config["pair"].ToString() : "EUR/USD";
            var basePrice = GetBasePriceForPair(pair);
            var variance = config.ContainsKey("variance") ? Convert.ToDecimal(config["variance"]) : 0.001m;
            
            return Math.Round(basePrice + new Faker().Random.Decimal(-variance, variance), 5);
        });

        RegisterGenerator("trade_amount", config =>
        {
            var min = config.ContainsKey("min") ? Convert.ToDecimal(config["min"]) : 0.01m;
            var max = config.ContainsKey("max") ? Convert.ToDecimal(config["max"]) : 10.0m;
            return Math.Round(new Faker().Random.Decimal(min, max), 2);
        });

        RegisterGenerator("leverage", config =>
        {
            var leverages = new[] { 1, 5, 10, 20, 30, 50, 100, 200, 400, 500 };
            return new Faker().PickRandom(leverages);
        });

        RegisterGenerator("account_balance", config =>
        {
            var min = config.ContainsKey("min") ? Convert.ToDecimal(config["min"]) : 1000.0m;
            var max = config.ContainsKey("max") ? Convert.ToDecimal(config["max"]) : 100000.0m;
            return Math.Round(new Faker().Random.Decimal(min, max), 2);
        });

        RegisterGenerator("order_type", _ => new Faker().PickRandom("Market", "Limit", "Stop", "StopLimit"));
    }

    private decimal GetBasePriceForPair(string pair)
    {
        return pair switch
        {
            "EUR/USD" => 1.1000m,
            "GBP/USD" => 1.3000m,
            "USD/JPY" => 110.00m,
            "AUD/USD" => 0.7500m,
            "USD/CHF" => 0.9200m,
            "USD/CAD" => 1.2500m,
            "NZD/USD" => 0.7000m,
            _ => 1.0000m
        };
    }

    /// <summary>
    /// Register a custom parameter generator
    /// </summary>
    public void RegisterGenerator(string type, Func<Dictionary<string, object>, object> generator)
    {
        _generators[type.ToLowerInvariant()] = generator;
        _logger.LogDebug("Registered parameter generator: {Type}", type);
    }

    /// <summary>
    /// Generate a single parameter
    /// </summary>
    public object GenerateParameter(string type, Dictionary<string, object>? config = null)
    {
        config ??= new Dictionary<string, object>();
        
        if (!_generators.TryGetValue(type.ToLowerInvariant(), out var generator))
        {
            _logger.LogWarning("No generator found for type: {Type}", type);
            return $"GENERATED_{type.ToUpper()}_{Guid.NewGuid().ToString()[..8]}";
        }

        try
        {
            var value = generator(config);
            _logger.LogDebug("Generated parameter - Type: {Type}, Value: {Value}", type, value);
            return value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate parameter of type: {Type}", type);
            return $"ERROR_GENERATING_{type.ToUpper()}";
        }
    }

    /// <summary>
    /// Generate multiple parameters based on configuration
    /// </summary>
    public Dictionary<string, object> GenerateParameters(Dictionary<string, ParameterConfig> parameterConfigs)
    {
        var parameters = new Dictionary<string, object>();

        foreach (var (name, config) in parameterConfigs)
        {
            try
            {
                var value = GenerateParameter(config.Type, config.Configuration);
                parameters[name] = value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate parameter: {ParameterName}", name);
                parameters[name] = $"ERROR_{name.ToUpper()}";
            }
        }

        return parameters;
    }

    /// <summary>
    /// Generate parameter sets for data-driven testing
    /// </summary>
    public List<Dictionary<string, object>> GenerateParameterSets(
        Dictionary<string, ParameterConfig> parameterConfigs, 
        int count = 1)
    {
        var parameterSets = new List<Dictionary<string, object>>();

        for (int i = 0; i < count; i++)
        {
            var parameters = GenerateParameters(parameterConfigs);
            parameterSets.Add(parameters);
        }

        return parameterSets;
    }

    /// <summary>
    /// Get available generator types
    /// </summary>
    public List<string> GetAvailableGenerators() => _generators.Keys.ToList();
}

public class ParameterConfig
{
    public string Type { get; set; } = string.Empty;
    public Dictionary<string, object> Configuration { get; set; } = new();
    public string Description { get; set; } = string.Empty;
    public bool IsRequired { get; set; } = true;
}

/// <summary>
/// Forex-specific parameter generator
/// </summary>
public class ForexParameterGenerator : ParameterGenerator
{
    public ForexParameterGenerator(ILogger<ParameterGenerator> logger) : base(logger)
    {
        RegisterForexSpecificGenerators();
    }

    private void RegisterForexSpecificGenerators()
    {
        // Market conditions
        RegisterGenerator("market_condition", _ => new Faker().PickRandom("Bullish", "Bearish", "Sideways", "Volatile"));
        
        // Trading session
        RegisterGenerator("trading_session", _ => new Faker().PickRandom("London", "NewYork", "Tokyo", "Sydney"));
        
        // Risk level
        RegisterGenerator("risk_level", _ => new Faker().PickRandom("Conservative", "Moderate", "Aggressive"));
        
        // News impact
        RegisterGenerator("news_impact", _ => new Faker().PickRandom("High", "Medium", "Low", "None"));

        // Economic indicators
        RegisterGenerator("economic_indicator", config =>
        {
            var indicators = new[] { "GDP", "CPI", "NFP", "Interest Rate", "PMI", "Unemployment Rate" };
            return new Faker().PickRandom(indicators);
        });

        // Price movement patterns
        RegisterGenerator("price_pattern", _ => new Faker().PickRandom(
            "Trend", "Range", "Breakout", "Reversal", "Consolidation"));

        // Volatility scenarios
        RegisterGenerator("volatility_scenario", config =>
        {
            var scenarios = new Dictionary<string, object>
            {
                ["low"] = new { min = 0.0001m, max = 0.005m, description = "Low volatility market" },
                ["medium"] = new { min = 0.005m, max = 0.015m, description = "Medium volatility market" },
                ["high"] = new { min = 0.015m, max = 0.050m, description = "High volatility market" },
                ["extreme"] = new { min = 0.050m, max = 0.200m, description = "Extreme volatility market" }
            };

            var level = config.ContainsKey("level") ? config["level"].ToString()?.ToLower() : "medium";
            return scenarios.ContainsKey(level!) ? scenarios[level!] : scenarios["medium"];
        });
    }
}