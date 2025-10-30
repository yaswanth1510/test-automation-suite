using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;

namespace ForexTestFramework.Core;

/// <summary>
/// Provides advanced comparison capabilities for test validation
/// </summary>
public class ComparisonTool
{
    private readonly ILogger<ComparisonTool> _logger;

    public ComparisonTool(ILogger<ComparisonTool> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Compare two objects using FluentAssertions
    /// </summary>
    public ComparisonResult CompareObjects<T>(T expected, T actual, ComparisonOptions? options = null)
    {
        options ??= new ComparisonOptions();
        var result = new ComparisonResult();

        try
        {
            if (options.IgnoreProperties.Any())
            {
                actual.Should().BeEquivalentTo(expected, opt => 
                    opt.Excluding(x => options.IgnoreProperties.Contains(x.Name)));
            }
            else
            {
                actual.Should().BeEquivalentTo(expected);
            }

            result.IsMatch = true;
            result.Message = "Objects are equivalent";
            _logger.LogDebug("Object comparison successful");
        }
        catch (Exception ex)
        {
            result.IsMatch = false;
            result.Message = ex.Message;
            result.Differences.Add($"Object comparison failed: {ex.Message}");
            _logger.LogWarning("Object comparison failed: {Error}", ex.Message);
        }

        return result;
    }

    /// <summary>
    /// Compare JSON strings with detailed differences
    /// </summary>
    public ComparisonResult CompareJson(string expectedJson, string actualJson, ComparisonOptions? options = null)
    {
        options ??= new ComparisonOptions();
        var result = new ComparisonResult();

        try
        {
            var expectedToken = JToken.Parse(expectedJson);
            var actualToken = JToken.Parse(actualJson);

            var differences = CompareJTokens(expectedToken, actualToken, "", options);
            
            result.IsMatch = !differences.Any();
            result.Differences = differences;
            result.Message = result.IsMatch ? "JSON objects are equivalent" : $"Found {differences.Count} differences";
            
            _logger.LogDebug("JSON comparison completed - IsMatch: {IsMatch}, Differences: {Count}", 
                result.IsMatch, differences.Count);
        }
        catch (JsonException ex)
        {
            result.IsMatch = false;
            result.Message = $"JSON parsing error: {ex.Message}";
            result.Differences.Add(result.Message);
            _logger.LogError(ex, "JSON comparison failed due to parsing error");
        }

        return result;
    }

    /// <summary>
    /// Compare XML documents
    /// </summary>
    public ComparisonResult CompareXml(string expectedXml, string actualXml, ComparisonOptions? options = null)
    {
        options ??= new ComparisonOptions();
        var result = new ComparisonResult();

        try
        {
            var expectedDoc = XDocument.Parse(expectedXml);
            var actualDoc = XDocument.Parse(actualXml);

            var differences = CompareXElements(expectedDoc.Root!, actualDoc.Root!, "", options);
            
            result.IsMatch = !differences.Any();
            result.Differences = differences;
            result.Message = result.IsMatch ? "XML documents are equivalent" : $"Found {differences.Count} differences";
            
            _logger.LogDebug("XML comparison completed - IsMatch: {IsMatch}, Differences: {Count}", 
                result.IsMatch, differences.Count);
        }
        catch (Exception ex)
        {
            result.IsMatch = false;
            result.Message = $"XML parsing error: {ex.Message}";
            result.Differences.Add(result.Message);
            _logger.LogError(ex, "XML comparison failed due to parsing error");
        }

        return result;
    }

    /// <summary>
    /// Compare numeric values with tolerance
    /// </summary>
    public ComparisonResult CompareNumeric(decimal expected, decimal actual, decimal tolerance = 0.001m)
    {
        var result = new ComparisonResult();
        var difference = Math.Abs(expected - actual);
        
        result.IsMatch = difference <= tolerance;
        result.Message = result.IsMatch 
            ? $"Numbers are within tolerance (difference: {difference})"
            : $"Numbers differ by {difference}, tolerance: {tolerance}";

        if (!result.IsMatch)
        {
            result.Differences.Add($"Expected: {expected}, Actual: {actual}, Difference: {difference}");
        }

        _logger.LogDebug("Numeric comparison - Expected: {Expected}, Actual: {Actual}, Tolerance: {Tolerance}, Match: {IsMatch}",
            expected, actual, tolerance, result.IsMatch);

        return result;
    }

    /// <summary>
    /// Compare collections with detailed analysis
    /// </summary>
    public ComparisonResult CompareCollections<T>(IEnumerable<T> expected, IEnumerable<T> actual, ComparisonOptions? options = null)
    {
        options ??= new ComparisonOptions();
        var result = new ComparisonResult();
        
        var expectedList = expected.ToList();
        var actualList = actual.ToList();

        // Count comparison
        if (expectedList.Count != actualList.Count)
        {
            result.Differences.Add($"Count mismatch - Expected: {expectedList.Count}, Actual: {actualList.Count}");
        }

        // Element-by-element comparison
        var maxCount = Math.Max(expectedList.Count, actualList.Count);
        for (int i = 0; i < maxCount; i++)
        {
            if (i >= expectedList.Count)
            {
                result.Differences.Add($"Extra item at index {i}: {actualList[i]}");
            }
            else if (i >= actualList.Count)
            {
                result.Differences.Add($"Missing item at index {i}: {expectedList[i]}");
            }
            else
            {
                var itemComparison = CompareObjects(expectedList[i], actualList[i], options);
                if (!itemComparison.IsMatch)
                {
                    result.Differences.Add($"Item at index {i}: {itemComparison.Message}");
                    result.Differences.AddRange(itemComparison.Differences.Select(d => $"  {d}"));
                }
            }
        }

        result.IsMatch = !result.Differences.Any();
        result.Message = result.IsMatch ? "Collections are equivalent" : $"Found {result.Differences.Count} differences";

        return result;
    }

    /// <summary>
    /// Compare database query results
    /// </summary>
    public ComparisonResult CompareDataSets(System.Data.DataTable expected, System.Data.DataTable actual, ComparisonOptions? options = null)
    {
        var result = new ComparisonResult();
        
        // Row count comparison
        if (expected.Rows.Count != actual.Rows.Count)
        {
            result.Differences.Add($"Row count mismatch - Expected: {expected.Rows.Count}, Actual: {actual.Rows.Count}");
        }

        // Column comparison
        if (expected.Columns.Count != actual.Columns.Count)
        {
            result.Differences.Add($"Column count mismatch - Expected: {expected.Columns.Count}, Actual: {actual.Columns.Count}");
        }

        // Column name comparison
        for (int i = 0; i < Math.Min(expected.Columns.Count, actual.Columns.Count); i++)
        {
            if (expected.Columns[i].ColumnName != actual.Columns[i].ColumnName)
            {
                result.Differences.Add($"Column name mismatch at index {i} - Expected: {expected.Columns[i].ColumnName}, Actual: {actual.Columns[i].ColumnName}");
            }
        }

        // Data comparison
        var maxRows = Math.Max(expected.Rows.Count, actual.Rows.Count);
        for (int rowIndex = 0; rowIndex < maxRows; rowIndex++)
        {
            if (rowIndex >= expected.Rows.Count)
            {
                result.Differences.Add($"Extra row at index {rowIndex}");
                continue;
            }
            
            if (rowIndex >= actual.Rows.Count)
            {
                result.Differences.Add($"Missing row at index {rowIndex}");
                continue;
            }

            var expectedRow = expected.Rows[rowIndex];
            var actualRow = actual.Rows[rowIndex];

            for (int colIndex = 0; colIndex < Math.Min(expected.Columns.Count, actual.Columns.Count); colIndex++)
            {
                var expectedValue = expectedRow[colIndex];
                var actualValue = actualRow[colIndex];

                if (!Equals(expectedValue, actualValue))
                {
                    result.Differences.Add($"Data mismatch at row {rowIndex}, column {expected.Columns[colIndex].ColumnName} - Expected: {expectedValue}, Actual: {actualValue}");
                }
            }
        }

        result.IsMatch = !result.Differences.Any();
        result.Message = result.IsMatch ? "DataSets are equivalent" : $"Found {result.Differences.Count} differences";

        return result;
    }

    private List<string> CompareJTokens(JToken expected, JToken actual, string path, ComparisonOptions options)
    {
        var differences = new List<string>();

        if (expected.Type != actual.Type)
        {
            differences.Add($"{path}: Type mismatch - Expected: {expected.Type}, Actual: {actual.Type}");
            return differences;
        }

        switch (expected.Type)
        {
            case JTokenType.Object:
                var expectedObj = (JObject)expected;
                var actualObj = (JObject)actual;

                foreach (var expectedProp in expectedObj.Properties())
                {
                    var currentPath = string.IsNullOrEmpty(path) ? expectedProp.Name : $"{path}.{expectedProp.Name}";
                    
                    if (options.IgnoreProperties.Contains(expectedProp.Name))
                        continue;

                    if (!actualObj.ContainsKey(expectedProp.Name))
                    {
                        differences.Add($"{currentPath}: Property missing in actual");
                    }
                    else
                    {
                        differences.AddRange(CompareJTokens(expectedProp.Value, actualObj[expectedProp.Name]!, currentPath, options));
                    }
                }

                foreach (var actualProp in actualObj.Properties())
                {
                    if (!expectedObj.ContainsKey(actualProp.Name) && !options.IgnoreProperties.Contains(actualProp.Name))
                    {
                        var currentPath = string.IsNullOrEmpty(path) ? actualProp.Name : $"{path}.{actualProp.Name}";
                        differences.Add($"{currentPath}: Extra property in actual");
                    }
                }
                break;

            case JTokenType.Array:
                var expectedArray = (JArray)expected;
                var actualArray = (JArray)actual;

                if (expectedArray.Count != actualArray.Count)
                {
                    differences.Add($"{path}: Array length mismatch - Expected: {expectedArray.Count}, Actual: {actualArray.Count}");
                }

                var maxLength = Math.Max(expectedArray.Count, actualArray.Count);
                for (int i = 0; i < maxLength; i++)
                {
                    var currentPath = $"{path}[{i}]";
                    
                    if (i >= expectedArray.Count)
                    {
                        differences.Add($"{currentPath}: Extra item in actual array");
                    }
                    else if (i >= actualArray.Count)
                    {
                        differences.Add($"{currentPath}: Missing item in actual array");
                    }
                    else
                    {
                        differences.AddRange(CompareJTokens(expectedArray[i], actualArray[i], currentPath, options));
                    }
                }
                break;

            default:
                var expectedValue = expected.ToString();
                var actualValue = actual.ToString();

                if (options.IgnoreCase)
                {
                    expectedValue = expectedValue.ToLowerInvariant();
                    actualValue = actualValue.ToLowerInvariant();
                }

                if (options.IgnoreWhitespace)
                {
                    expectedValue = expectedValue.Trim();
                    actualValue = actualValue.Trim();
                }

                if (expectedValue != actualValue)
                {
                    differences.Add($"{path}: Value mismatch - Expected: '{expected}', Actual: '{actual}'");
                }
                break;
        }

        return differences;
    }

    private List<string> CompareXElements(XElement expected, XElement actual, string path, ComparisonOptions options)
    {
        var differences = new List<string>();
        var currentPath = string.IsNullOrEmpty(path) ? expected.Name.LocalName : $"{path}/{expected.Name.LocalName}";

        // Name comparison
        if (expected.Name != actual.Name)
        {
            differences.Add($"{currentPath}: Element name mismatch - Expected: {expected.Name}, Actual: {actual.Name}");
            return differences;
        }

        // Attribute comparison
        var expectedAttrs = expected.Attributes().ToDictionary(a => a.Name, a => a.Value);
        var actualAttrs = actual.Attributes().ToDictionary(a => a.Name, a => a.Value);

        foreach (var expectedAttr in expectedAttrs)
        {
            if (options.IgnoreProperties.Contains(expectedAttr.Key.LocalName))
                continue;

            if (!actualAttrs.ContainsKey(expectedAttr.Key))
            {
                differences.Add($"{currentPath}@{expectedAttr.Key}: Attribute missing");
            }
            else
            {
                var expectedValue = expectedAttr.Value;
                var actualValue = actualAttrs[expectedAttr.Key];

                if (options.IgnoreCase)
                {
                    expectedValue = expectedValue.ToLowerInvariant();
                    actualValue = actualValue.ToLowerInvariant();
                }

                if (expectedValue != actualValue)
                {
                    differences.Add($"{currentPath}@{expectedAttr.Key}: Attribute value mismatch - Expected: '{expectedAttr.Value}', Actual: '{actualAttrs[expectedAttr.Key]}'");
                }
            }
        }

        // Value comparison (if no child elements)
        if (!expected.HasElements && !actual.HasElements)
        {
            var expectedValue = expected.Value;
            var actualValue = actual.Value;

            if (options.IgnoreCase)
            {
                expectedValue = expectedValue.ToLowerInvariant();
                actualValue = actualValue.ToLowerInvariant();
            }

            if (options.IgnoreWhitespace)
            {
                expectedValue = expectedValue.Trim();
                actualValue = actualValue.Trim();
            }

            if (expectedValue != actualValue)
            {
                differences.Add($"{currentPath}: Text content mismatch - Expected: '{expected.Value}', Actual: '{actual.Value}'");
            }
        }

        // Child element comparison
        var expectedChildren = expected.Elements().ToList();
        var actualChildren = actual.Elements().ToList();

        if (expectedChildren.Count != actualChildren.Count)
        {
            differences.Add($"{currentPath}: Child element count mismatch - Expected: {expectedChildren.Count}, Actual: {actualChildren.Count}");
        }

        var maxChildren = Math.Max(expectedChildren.Count, actualChildren.Count);
        for (int i = 0; i < maxChildren; i++)
        {
            if (i >= expectedChildren.Count)
            {
                differences.Add($"{currentPath}[{i}]: Extra child element in actual");
            }
            else if (i >= actualChildren.Count)
            {
                differences.Add($"{currentPath}[{i}]: Missing child element in actual");
            }
            else
            {
                differences.AddRange(CompareXElements(expectedChildren[i], actualChildren[i], currentPath, options));
            }
        }

        return differences;
    }
}

public class ComparisonResult
{
    public bool IsMatch { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Differences { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class ComparisonOptions
{
    public bool IgnoreCase { get; set; } = false;
    public bool IgnoreWhitespace { get; set; } = false;
    public List<string> IgnoreProperties { get; set; } = new();
    public decimal NumericTolerance { get; set; } = 0.001m;
}