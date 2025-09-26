using RestSharp;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ForexTestFramework.Clients;

/// <summary>
/// HTTP client for API testing with advanced features
/// </summary>
public class HttpTestClient
{
    private readonly RestClient _client;
    private readonly ILogger<HttpTestClient> _logger;
    private readonly Dictionary<string, string> _defaultHeaders = new();

    public HttpTestClient(string baseUrl, ILogger<HttpTestClient> logger)
    {
        var options = new RestClientOptions(baseUrl)
        {
            ThrowOnAnyError = false,
            Timeout = TimeSpan.FromSeconds(30)
        };
        
        _client = new RestClient(options);
        _logger = logger;

        // Add default headers
        _defaultHeaders.Add("User-Agent", "ForexTestFramework/1.0");
        _defaultHeaders.Add("Accept", "application/json");
    }

    /// <summary>
    /// Set default headers for all requests
    /// </summary>
    public void SetDefaultHeader(string name, string value)
    {
        _defaultHeaders[name] = value;
        _logger.LogDebug("Set default header: {Name} = {Value}", name, value);
    }

    /// <summary>
    /// Set authentication token
    /// </summary>
    public void SetAuthToken(string token, string scheme = "Bearer")
    {
        SetDefaultHeader("Authorization", $"{scheme} {token}");
    }

    /// <summary>
    /// Execute GET request
    /// </summary>
    public async Task<ApiResponse<T>> GetAsync<T>(string resource, Dictionary<string, object>? parameters = null)
    {
        var request = new RestRequest(resource, Method.Get);
        AddDefaultHeaders(request);
        AddParameters(request, parameters);

        _logger.LogInformation("Executing GET request to {Resource}", resource);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            var response = await _client.ExecuteAsync<T>(request);
            stopwatch.Stop();

            var apiResponse = CreateApiResponse<T>(response, stopwatch.ElapsedMilliseconds);
            LogResponse(resource, "GET", response.StatusCode, stopwatch.ElapsedMilliseconds);

            return apiResponse;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "GET request failed for {Resource}", resource);
            return CreateErrorResponse<T>(ex, stopwatch.ElapsedMilliseconds);
        }
    }

    /// <summary>
    /// Execute POST request
    /// </summary>
    public async Task<ApiResponse<T>> PostAsync<T>(string resource, object? body = null, Dictionary<string, object>? parameters = null)
    {
        var request = new RestRequest(resource, Method.Post);
        AddDefaultHeaders(request);
        AddParameters(request, parameters);
        
        if (body != null)
        {
            request.AddJsonBody(body);
        }

        _logger.LogInformation("Executing POST request to {Resource}", resource);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            var response = await _client.ExecuteAsync<T>(request);
            stopwatch.Stop();

            var apiResponse = CreateApiResponse<T>(response, stopwatch.ElapsedMilliseconds);
            LogResponse(resource, "POST", response.StatusCode, stopwatch.ElapsedMilliseconds);

            return apiResponse;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "POST request failed for {Resource}", resource);
            return CreateErrorResponse<T>(ex, stopwatch.ElapsedMilliseconds);
        }
    }

    /// <summary>
    /// Execute PUT request
    /// </summary>
    public async Task<ApiResponse<T>> PutAsync<T>(string resource, object? body = null, Dictionary<string, object>? parameters = null)
    {
        var request = new RestRequest(resource, Method.Put);
        AddDefaultHeaders(request);
        AddParameters(request, parameters);
        
        if (body != null)
        {
            request.AddJsonBody(body);
        }

        _logger.LogInformation("Executing PUT request to {Resource}", resource);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            var response = await _client.ExecuteAsync<T>(request);
            stopwatch.Stop();

            var apiResponse = CreateApiResponse<T>(response, stopwatch.ElapsedMilliseconds);
            LogResponse(resource, "PUT", response.StatusCode, stopwatch.ElapsedMilliseconds);

            return apiResponse;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "PUT request failed for {Resource}", resource);
            return CreateErrorResponse<T>(ex, stopwatch.ElapsedMilliseconds);
        }
    }

    /// <summary>
    /// Execute DELETE request
    /// </summary>
    public async Task<ApiResponse<T>> DeleteAsync<T>(string resource, Dictionary<string, object>? parameters = null)
    {
        var request = new RestRequest(resource, Method.Delete);
        AddDefaultHeaders(request);
        AddParameters(request, parameters);

        _logger.LogInformation("Executing DELETE request to {Resource}", resource);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            var response = await _client.ExecuteAsync<T>(request);
            stopwatch.Stop();

            var apiResponse = CreateApiResponse<T>(response, stopwatch.ElapsedMilliseconds);
            LogResponse(resource, "DELETE", response.StatusCode, stopwatch.ElapsedMilliseconds);

            return apiResponse;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "DELETE request failed for {Resource}", resource);
            return CreateErrorResponse<T>(ex, stopwatch.ElapsedMilliseconds);
        }
    }

    /// <summary>
    /// Execute custom request with full control
    /// </summary>
    public async Task<ApiResponse<T>> ExecuteAsync<T>(RestRequest request)
    {
        AddDefaultHeaders(request);
        
        _logger.LogInformation("Executing {Method} request to {Resource}", request.Method, request.Resource);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            var response = await _client.ExecuteAsync<T>(request);
            stopwatch.Stop();

            var apiResponse = CreateApiResponse<T>(response, stopwatch.ElapsedMilliseconds);
            LogResponse(request.Resource, request.Method.ToString(), response.StatusCode, stopwatch.ElapsedMilliseconds);

            return apiResponse;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "{Method} request failed for {Resource}", request.Method, request.Resource);
            return CreateErrorResponse<T>(ex, stopwatch.ElapsedMilliseconds);
        }
    }

    private void AddDefaultHeaders(RestRequest request)
    {
        foreach (var header in _defaultHeaders)
        {
            request.AddHeader(header.Key, header.Value);
        }
    }

    private void AddParameters(RestRequest request, Dictionary<string, object>? parameters)
    {
        if (parameters == null) return;

        foreach (var param in parameters)
        {
            request.AddParameter(param.Key, param.Value);
        }
    }

    private ApiResponse<T> CreateApiResponse<T>(RestResponse<T> response, long elapsedMilliseconds)
    {
        return new ApiResponse<T>
        {
            IsSuccess = response.IsSuccessful,
            StatusCode = (int)response.StatusCode,
            StatusDescription = response.StatusDescription ?? string.Empty,
            Data = response.Data,
            RawContent = response.Content ?? string.Empty,
            Headers = response.Headers?.ToDictionary(h => h.Name, h => h.Value?.ToString() ?? string.Empty) ?? new(),
            ElapsedMilliseconds = elapsedMilliseconds,
            ErrorMessage = response.ErrorMessage
        };
    }

    private ApiResponse<T> CreateErrorResponse<T>(Exception ex, long elapsedMilliseconds)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            StatusCode = 0,
            StatusDescription = "Exception",
            Data = default,
            RawContent = string.Empty,
            Headers = new Dictionary<string, string>(),
            ElapsedMilliseconds = elapsedMilliseconds,
            ErrorMessage = ex.Message,
            Exception = ex
        };
    }

    private void LogResponse(string resource, string method, System.Net.HttpStatusCode statusCode, long elapsedMs)
    {
        _logger.LogInformation("{Method} {Resource} responded with {StatusCode} in {ElapsedMs}ms", 
            method, resource, statusCode, elapsedMs);
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}

public class ApiResponse<T>
{
    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; }
    public string StatusDescription { get; set; } = string.Empty;
    public T? Data { get; set; }
    public string RawContent { get; set; } = string.Empty;
    public Dictionary<string, string> Headers { get; set; } = new();
    public long ElapsedMilliseconds { get; set; }
    public string? ErrorMessage { get; set; }
    public Exception? Exception { get; set; }
}

/// <summary>
/// Forex-specific HTTP test client with trading API methods
/// </summary>
public class ForexApiClient : HttpTestClient
{
    public ForexApiClient(string baseUrl, ILogger<HttpTestClient> logger) : base(baseUrl, logger)
    {
    }

    /// <summary>
    /// Get market data for a currency pair
    /// </summary>
    public async Task<ApiResponse<MarketData>> GetMarketDataAsync(string currencyPair)
    {
        return await GetAsync<MarketData>($"market-data/{currencyPair}");
    }

    /// <summary>
    /// Place a trading order
    /// </summary>
    public async Task<ApiResponse<OrderResponse>> PlaceOrderAsync(OrderRequest order)
    {
        return await PostAsync<OrderResponse>("orders", order);
    }

    /// <summary>
    /// Get account balance
    /// </summary>
    public async Task<ApiResponse<AccountBalance>> GetAccountBalanceAsync(string accountId)
    {
        return await GetAsync<AccountBalance>($"accounts/{accountId}/balance");
    }

    /// <summary>
    /// Get trading positions
    /// </summary>
    public async Task<ApiResponse<List<Position>>> GetPositionsAsync(string accountId)
    {
        return await GetAsync<List<Position>>($"accounts/{accountId}/positions");
    }

    /// <summary>
    /// Close a position
    /// </summary>
    public async Task<ApiResponse<ClosePositionResponse>> ClosePositionAsync(string positionId)
    {
        return await DeleteAsync<ClosePositionResponse>($"positions/{positionId}");
    }
}

// Forex-specific DTOs
public class MarketData
{
    public string CurrencyPair { get; set; } = string.Empty;
    public decimal Bid { get; set; }
    public decimal Ask { get; set; }
    public decimal Spread => Ask - Bid;
    public DateTime Timestamp { get; set; }
    public decimal High24h { get; set; }
    public decimal Low24h { get; set; }
    public decimal Volume { get; set; }
}

public class OrderRequest
{
    public string CurrencyPair { get; set; } = string.Empty;
    public string OrderType { get; set; } = string.Empty; // Market, Limit, Stop
    public decimal Amount { get; set; }
    public decimal? Price { get; set; }
    public decimal? StopLoss { get; set; }
    public decimal? TakeProfit { get; set; }
    public string Side { get; set; } = string.Empty; // Buy, Sell
}

public class OrderResponse
{
    public string OrderId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal ExecutedAmount { get; set; }
    public decimal ExecutedPrice { get; set; }
    public DateTime ExecutionTime { get; set; }
}

public class AccountBalance
{
    public string AccountId { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public decimal AvailableMargin { get; set; }
    public decimal UsedMargin { get; set; }
    public decimal UnrealizedPnL { get; set; }
}

public class Position
{
    public string PositionId { get; set; } = string.Empty;
    public string CurrencyPair { get; set; } = string.Empty;
    public string Side { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal OpenPrice { get; set; }
    public decimal CurrentPrice { get; set; }
    public decimal UnrealizedPnL { get; set; }
    public DateTime OpenTime { get; set; }
}

public class ClosePositionResponse
{
    public string PositionId { get; set; } = string.Empty;
    public decimal ClosePrice { get; set; }
    public decimal RealizedPnL { get; set; }
    public DateTime CloseTime { get; set; }
}