using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;

namespace ForexTestFramework.Clients;

/// <summary>
/// Database test client for SQL operations and validations
/// </summary>
public class SqlTestClient
{
    private readonly string _connectionString;
    private readonly ILogger<SqlTestClient> _logger;

    public SqlTestClient(string connectionString, ILogger<SqlTestClient> logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }

    /// <summary>
    /// Execute a SQL query and return results as DataTable
    /// </summary>
    public async Task<DatabaseResult<DataTable>> ExecuteQueryAsync(string query, Dictionary<string, object>? parameters = null)
    {
        _logger.LogInformation("Executing query: {Query}", query);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            AddParameters(command, parameters);

            using var adapter = new SqlDataAdapter(command);
            var dataTable = new DataTable();
            adapter.Fill(dataTable);

            stopwatch.Stop();

            _logger.LogInformation("Query executed successfully, returned {RowCount} rows in {ElapsedMs}ms", 
                dataTable.Rows.Count, stopwatch.ElapsedMilliseconds);

            return new DatabaseResult<DataTable>
            {
                IsSuccess = true,
                Data = dataTable,
                RowsAffected = dataTable.Rows.Count,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                Message = $"Query executed successfully, {dataTable.Rows.Count} rows returned"
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Query execution failed: {Query}", query);

            return new DatabaseResult<DataTable>
            {
                IsSuccess = false,
                Data = new DataTable(),
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                Message = ex.Message,
                Exception = ex
            };
        }
    }

    /// <summary>
    /// Execute a SQL command (INSERT, UPDATE, DELETE)
    /// </summary>
    public async Task<DatabaseResult<int>> ExecuteCommandAsync(string command, Dictionary<string, object>? parameters = null)
    {
        _logger.LogInformation("Executing command: {Command}", command);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var sqlCommand = new SqlCommand(command, connection);
            AddParameters(sqlCommand, parameters);

            var rowsAffected = await sqlCommand.ExecuteNonQueryAsync();
            stopwatch.Stop();

            _logger.LogInformation("Command executed successfully, {RowsAffected} rows affected in {ElapsedMs}ms", 
                rowsAffected, stopwatch.ElapsedMilliseconds);

            return new DatabaseResult<int>
            {
                IsSuccess = true,
                Data = rowsAffected,
                RowsAffected = rowsAffected,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                Message = $"Command executed successfully, {rowsAffected} rows affected"
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Command execution failed: {Command}", command);

            return new DatabaseResult<int>
            {
                IsSuccess = false,
                Data = 0,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                Message = ex.Message,
                Exception = ex
            };
        }
    }

    /// <summary>
    /// Execute a scalar query (returns single value)
    /// </summary>
    public async Task<DatabaseResult<T>> ExecuteScalarAsync<T>(string query, Dictionary<string, object>? parameters = null)
    {
        _logger.LogInformation("Executing scalar query: {Query}", query);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            AddParameters(command, parameters);

            var result = await command.ExecuteScalarAsync();
            stopwatch.Stop();

            T? data = default;
            if (result != null && result != DBNull.Value)
            {
                data = (T)Convert.ChangeType(result, typeof(T));
            }

            _logger.LogInformation("Scalar query executed successfully in {ElapsedMs}ms, result: {Result}", 
                stopwatch.ElapsedMilliseconds, data);

            return new DatabaseResult<T>
            {
                IsSuccess = true,
                Data = data,
                RowsAffected = 1,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                Message = $"Scalar query executed successfully, result: {data}"
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Scalar query execution failed: {Query}", query);

            return new DatabaseResult<T>
            {
                IsSuccess = false,
                Data = default,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                Message = ex.Message,
                Exception = ex
            };
        }
    }

    /// <summary>
    /// Test database connection
    /// </summary>
    public async Task<DatabaseResult<bool>> TestConnectionAsync()
    {
        _logger.LogInformation("Testing database connection");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            stopwatch.Stop();

            _logger.LogInformation("Database connection successful in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);

            return new DatabaseResult<bool>
            {
                IsSuccess = true,
                Data = true,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                Message = "Database connection successful"
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Database connection failed");

            return new DatabaseResult<bool>
            {
                IsSuccess = false,
                Data = false,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                Message = ex.Message,
                Exception = ex
            };
        }
    }

    /// <summary>
    /// Execute transaction with multiple commands
    /// </summary>
    public async Task<DatabaseResult<List<int>>> ExecuteTransactionAsync(List<string> commands, Dictionary<string, object>? parameters = null)
    {
        _logger.LogInformation("Executing transaction with {CommandCount} commands", commands.Count);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var results = new List<int>();

        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            foreach (var commandText in commands)
            {
                using var command = new SqlCommand(commandText, connection, transaction);
                AddParameters(command, parameters);

                var rowsAffected = await command.ExecuteNonQueryAsync();
                results.Add(rowsAffected);
            }

            await transaction.CommitAsync();
            stopwatch.Stop();

            var totalRowsAffected = results.Sum();
            _logger.LogInformation("Transaction completed successfully, {TotalRows} total rows affected in {ElapsedMs}ms", 
                totalRowsAffected, stopwatch.ElapsedMilliseconds);

            return new DatabaseResult<List<int>>
            {
                IsSuccess = true,
                Data = results,
                RowsAffected = totalRowsAffected,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                Message = $"Transaction completed successfully, {totalRowsAffected} total rows affected"
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Transaction failed");

            return new DatabaseResult<List<int>>
            {
                IsSuccess = false,
                Data = results,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                Message = ex.Message,
                Exception = ex
            };
        }
    }

    /// <summary>
    /// Validate data integrity
    /// </summary>
    public async Task<DatabaseValidationResult> ValidateDataIntegrityAsync(List<DataValidationRule> rules)
    {
        var result = new DatabaseValidationResult();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        _logger.LogInformation("Starting data integrity validation with {RuleCount} rules", rules.Count);

        foreach (var rule in rules)
        {
            try
            {
                var queryResult = await ExecuteScalarAsync<int>(rule.Query, rule.Parameters);
                var validationResult = new ValidationRuleResult
                {
                    RuleName = rule.Name,
                    Query = rule.Query,
                    ExpectedResult = rule.ExpectedResult,
                    ActualResult = queryResult.Data,
                    IsValid = queryResult.IsSuccess && queryResult.Data == rule.ExpectedResult,
                    Message = queryResult.IsSuccess 
                        ? (queryResult.Data == rule.ExpectedResult ? "Validation passed" : $"Expected {rule.ExpectedResult}, got {queryResult.Data}")
                        : queryResult.Message
                };

                result.RuleResults.Add(validationResult);

                if (!validationResult.IsValid)
                {
                    result.FailedRules.Add(rule.Name);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Validation rule failed: {RuleName}", rule.Name);
                
                var validationResult = new ValidationRuleResult
                {
                    RuleName = rule.Name,
                    Query = rule.Query,
                    ExpectedResult = rule.ExpectedResult,
                    ActualResult = -1,
                    IsValid = false,
                    Message = ex.Message,
                    Exception = ex
                };

                result.RuleResults.Add(validationResult);
                result.FailedRules.Add(rule.Name);
            }
        }

        stopwatch.Stop();
        result.IsSuccess = result.FailedRules.Count == 0;
        result.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        result.Message = result.IsSuccess 
            ? $"All {rules.Count} validation rules passed"
            : $"{result.FailedRules.Count} of {rules.Count} validation rules failed";

        _logger.LogInformation("Data integrity validation completed in {ElapsedMs}ms - {Status}", 
            stopwatch.ElapsedMilliseconds, result.IsSuccess ? "SUCCESS" : "FAILED");

        return result;
    }

    private static void AddParameters(SqlCommand command, Dictionary<string, object>? parameters)
    {
        if (parameters == null) return;

        foreach (var param in parameters)
        {
            command.Parameters.AddWithValue($"@{param.Key}", param.Value ?? DBNull.Value);
        }
    }
}

public class DatabaseResult<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public int RowsAffected { get; set; }
    public long ElapsedMilliseconds { get; set; }
    public string Message { get; set; } = string.Empty;
    public Exception? Exception { get; set; }
}

public class DataValidationRule
{
    public string Name { get; set; } = string.Empty;
    public string Query { get; set; } = string.Empty;
    public int ExpectedResult { get; set; }
    public Dictionary<string, object>? Parameters { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class ValidationRuleResult
{
    public string RuleName { get; set; } = string.Empty;
    public string Query { get; set; } = string.Empty;
    public int ExpectedResult { get; set; }
    public int ActualResult { get; set; }
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
    public Exception? Exception { get; set; }
}

public class DatabaseValidationResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public long ElapsedMilliseconds { get; set; }
    public List<ValidationRuleResult> RuleResults { get; set; } = new();
    public List<string> FailedRules { get; set; } = new();
}

/// <summary>
/// Forex-specific database client with trading data queries
/// </summary>
public class ForexDatabaseClient : SqlTestClient
{
    public ForexDatabaseClient(string connectionString, ILogger<SqlTestClient> logger) : base(connectionString, logger)
    {
    }

    /// <summary>
    /// Validate account balance integrity
    /// </summary>
    public async Task<DatabaseResult<bool>> ValidateAccountBalanceAsync(string accountId, decimal expectedBalance, decimal tolerance = 0.01m)
    {
        var query = "SELECT Balance FROM Accounts WHERE AccountId = @accountId";
        var parameters = new Dictionary<string, object> { ["accountId"] = accountId };

        var result = await ExecuteScalarAsync<decimal>(query, parameters);
        
        if (result.IsSuccess)
        {
            var actualBalance = result.Data;
            var difference = Math.Abs(expectedBalance - actualBalance);
            var isValid = difference <= tolerance;

            return new DatabaseResult<bool>
            {
                IsSuccess = true,
                Data = isValid,
                ElapsedMilliseconds = result.ElapsedMilliseconds,
                Message = isValid 
                    ? $"Balance validation passed. Expected: {expectedBalance}, Actual: {actualBalance}"
                    : $"Balance validation failed. Expected: {expectedBalance}, Actual: {actualBalance}, Difference: {difference}"
            };
        }

        return new DatabaseResult<bool>
        {
            IsSuccess = false,
            Data = false,
            ElapsedMilliseconds = result.ElapsedMilliseconds,
            Message = result.Message,
            Exception = result.Exception
        };
    }

    /// <summary>
    /// Get trading positions for account
    /// </summary>
    public async Task<DatabaseResult<DataTable>> GetTradingPositionsAsync(string accountId, bool openOnly = true)
    {
        var query = openOnly 
            ? "SELECT * FROM Positions WHERE AccountId = @accountId AND Status = 'Open'"
            : "SELECT * FROM Positions WHERE AccountId = @accountId";
        
        var parameters = new Dictionary<string, object> { ["accountId"] = accountId };
        
        return await ExecuteQueryAsync(query, parameters);
    }

    /// <summary>
    /// Validate P&L calculation
    /// </summary>
    public async Task<DatabaseResult<bool>> ValidatePnLCalculationAsync(string positionId)
    {
        var query = @"
            SELECT 
                p.Amount,
                p.OpenPrice,
                p.CurrentPrice,
                p.UnrealizedPnL,
                (p.Amount * (p.CurrentPrice - p.OpenPrice)) as CalculatedPnL
            FROM Positions p 
            WHERE p.PositionId = @positionId";
        
        var parameters = new Dictionary<string, object> { ["positionId"] = positionId };
        var result = await ExecuteQueryAsync(query, parameters);

        if (result.IsSuccess && result.Data.Rows.Count > 0)
        {
            var row = result.Data.Rows[0];
            var storedPnL = Convert.ToDecimal(row["UnrealizedPnL"]);
            var calculatedPnL = Convert.ToDecimal(row["CalculatedPnL"]);
            var difference = Math.Abs(storedPnL - calculatedPnL);
            var isValid = difference < 0.01m; // 1 cent tolerance

            return new DatabaseResult<bool>
            {
                IsSuccess = true,
                Data = isValid,
                ElapsedMilliseconds = result.ElapsedMilliseconds,
                Message = isValid 
                    ? $"P&L calculation is correct. Stored: {storedPnL}, Calculated: {calculatedPnL}"
                    : $"P&L calculation error. Stored: {storedPnL}, Calculated: {calculatedPnL}, Difference: {difference}"
            };
        }

        return new DatabaseResult<bool>
        {
            IsSuccess = false,
            Data = false,
            ElapsedMilliseconds = result.ElapsedMilliseconds,
            Message = "Position not found or query failed"
        };
    }
}