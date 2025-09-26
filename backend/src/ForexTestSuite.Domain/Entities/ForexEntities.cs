using ForexTestSuite.Domain.Common;
using ForexTestSuite.Domain.Enums;

namespace ForexTestSuite.Domain.Entities;

public class ForexTradingTest : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CurrencyPair { get; set; } = string.Empty;
    public decimal TestAmount { get; set; }
    public decimal Leverage { get; set; } = 1.0m;
    public Dictionary<string, object> TradingParameters { get; set; } = new();
    public Dictionary<string, object> RiskParameters { get; set; } = new();
    
    // Navigation properties
    public ICollection<MarketDataSimulation> MarketDataSimulations { get; set; } = new List<MarketDataSimulation>();
    public ICollection<TradingOrderTest> TradingOrderTests { get; set; } = new List<TradingOrderTest>();
}

public class MarketDataSimulation : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string CurrencyPair { get; set; } = string.Empty;
    public decimal StartPrice { get; set; }
    public decimal EndPrice { get; set; }
    public decimal Volatility { get; set; }
    public DateTime SimulationStart { get; set; }
    public DateTime SimulationEnd { get; set; }
    public Dictionary<string, object> PriceMovements { get; set; } = new();
    public Dictionary<string, object> NewsEvents { get; set; } = new();
    
    // Foreign keys
    public Guid ForexTradingTestId { get; set; }
    
    // Navigation properties
    public ForexTradingTest ForexTradingTest { get; set; } = null!;
}

public class TradingOrderTest : BaseEntity
{
    public string OrderType { get; set; } = string.Empty; // Market, Limit, Stop, etc.
    public string CurrencyPair { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal? Price { get; set; }
    public decimal? StopLoss { get; set; }
    public decimal? TakeProfit { get; set; }
    public DateTime OrderTime { get; set; }
    public string ExpectedResult { get; set; } = string.Empty;
    public Dictionary<string, object> ValidationRules { get; set; } = new();
    
    // Foreign keys
    public Guid ForexTradingTestId { get; set; }
    
    // Navigation properties
    public ForexTradingTest ForexTradingTest { get; set; } = null!;
}

public class RiskManagementTest : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal AccountBalance { get; set; }
    public decimal MaxRiskPerTrade { get; set; }
    public decimal MaxDrawdown { get; set; }
    public decimal Leverage { get; set; }
    public Dictionary<string, object> RiskRules { get; set; } = new();
    public Dictionary<string, object> ExpectedOutcomes { get; set; } = new();
}

public class ComplianceTest : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string RegulationType { get; set; } = string.Empty; // KYC, AML, MiFID, etc.
    public Dictionary<string, object> TestData { get; set; } = new();
    public Dictionary<string, object> ValidationCriteria { get; set; } = new();
    public Dictionary<string, object> ExpectedResults { get; set; } = new();
}

public class FinancialCalculationTest : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string CalculationType { get; set; } = string.Empty; // PnL, Swap, Fee, etc.
    public string CurrencyPair { get; set; } = string.Empty;
    public decimal OpenPrice { get; set; }
    public decimal ClosePrice { get; set; }
    public decimal Amount { get; set; }
    public Dictionary<string, object> CalculationParameters { get; set; } = new();
    public decimal ExpectedResult { get; set; }
    public decimal Tolerance { get; set; } = 0.01m;
}