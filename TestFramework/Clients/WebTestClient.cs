using Microsoft.Playwright;
using Microsoft.Extensions.Logging;

namespace ForexTestFramework.Clients;

/// <summary>
/// Advanced web automation client using Playwright
/// </summary>
public class WebTestClient
{
    private readonly ILogger<WebTestClient> _logger;
    private IBrowser? _browser;
    private IPage? _currentPage;
    private readonly WebTestOptions _options;

    public WebTestClient(ILogger<WebTestClient> logger, WebTestOptions? options = null)
    {
        _logger = logger;
        _options = options ?? new WebTestOptions();
    }

    /// <summary>
    /// Initialize browser with specified options
    /// </summary>
    public async Task InitializeBrowserAsync(string browserType = "chromium")
    {
        _logger.LogInformation("Initializing {BrowserType} browser", browserType);
        
        var playwright = await Playwright.CreateAsync();
        
        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = _options.Headless,
            SlowMo = _options.SlowMo,
            Args = _options.BrowserArgs,
            Timeout = _options.LaunchTimeout
        };

        _browser = browserType.ToLowerInvariant() switch
        {
            "chromium" => await playwright.Chromium.LaunchAsync(launchOptions),
            "firefox" => await playwright.Firefox.LaunchAsync(launchOptions),
            "webkit" => await playwright.Webkit.LaunchAsync(launchOptions),
            _ => await playwright.Chromium.LaunchAsync(launchOptions)
        };

        _logger.LogInformation("Browser initialized successfully");
    }

    /// <summary>
    /// Create a new page/tab
    /// </summary>
    public async Task<IPage> CreatePageAsync()
    {
        if (_browser == null)
            throw new InvalidOperationException("Browser must be initialized first");

        var context = await _browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = _options.ViewportSize,
            UserAgent = _options.UserAgent,
            Locale = _options.Locale,
            TimezoneId = _options.TimezoneId
        });

        var page = await context.NewPageAsync();
        
        // Set default timeouts
        page.SetDefaultTimeout(_options.DefaultTimeout);
        page.SetDefaultNavigationTimeout(_options.NavigationTimeout);

        _currentPage = page;
        _logger.LogDebug("New page created");
        
        return page;
    }

    /// <summary>
    /// Navigate to URL
    /// </summary>
    public async Task<WebActionResult> NavigateAsync(string url)
    {
        if (_currentPage == null)
            await CreatePageAsync();

        _logger.LogInformation("Navigating to {Url}", url);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            var response = await _currentPage!.GotoAsync(url);
            stopwatch.Stop();

            var result = new WebActionResult
            {
                IsSuccess = response?.Ok ?? false,
                ActionType = "Navigate",
                Target = url,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                Message = $"Navigated to {url}",
                StatusCode = response?.Status ?? 0
            };

            _logger.LogInformation("Navigation completed in {ElapsedMs}ms with status {Status}", 
                stopwatch.ElapsedMilliseconds, response?.Status);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Navigation failed to {Url}", url);
            
            return new WebActionResult
            {
                IsSuccess = false,
                ActionType = "Navigate",
                Target = url,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                Message = ex.Message,
                Exception = ex
            };
        }
    }

    /// <summary>
    /// Click an element
    /// </summary>
    public async Task<WebActionResult> ClickAsync(string selector, WebElementOptions? options = null)
    {
        return await ExecuteActionAsync("Click", selector, async () =>
        {
            var element = await _currentPage!.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions
            {
                Timeout = options?.Timeout ?? _options.DefaultTimeout,
                State = WaitForSelectorState.Visible
            });

            if (element == null)
                throw new Exception($"Element not found: {selector}");

            await element.ClickAsync(new LocatorClickOptions
            {
                Force = options?.Force ?? false,
                Timeout = options?.Timeout ?? _options.DefaultTimeout
            });

            return "Element clicked successfully";
        });
    }

    /// <summary>
    /// Type text into an element
    /// </summary>
    public async Task<WebActionResult> TypeAsync(string selector, string text, WebElementOptions? options = null)
    {
        return await ExecuteActionAsync("Type", selector, async () =>
        {
            var element = await _currentPage!.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions
            {
                Timeout = options?.Timeout ?? _options.DefaultTimeout,
                State = WaitForSelectorState.Visible
            });

            if (element == null)
                throw new Exception($"Element not found: {selector}");

            if (options?.Clear ?? false)
                await element.ClearAsync();

            await element.TypeAsync(text, new LocatorTypeOptions
            {
                Delay = options?.TypeDelay ?? 0,
                Timeout = options?.Timeout ?? _options.DefaultTimeout
            });

            return $"Text typed: {text}";
        });
    }

    /// <summary>
    /// Get element text
    /// </summary>
    public async Task<WebActionResult<string>> GetTextAsync(string selector, WebElementOptions? options = null)
    {
        var result = await ExecuteActionAsync("GetText", selector, async () =>
        {
            var element = await _currentPage!.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions
            {
                Timeout = options?.Timeout ?? _options.DefaultTimeout,
                State = WaitForSelectorState.Attached
            });

            if (element == null)
                throw new Exception($"Element not found: {selector}");

            return await element.TextContentAsync() ?? string.Empty;
        });

        return new WebActionResult<string>
        {
            IsSuccess = result.IsSuccess,
            ActionType = result.ActionType,
            Target = result.Target,
            ElapsedMilliseconds = result.ElapsedMilliseconds,
            Message = result.Message,
            Exception = result.Exception,
            Data = result.IsSuccess ? result.Message : string.Empty
        };
    }

    /// <summary>
    /// Wait for element to be visible
    /// </summary>
    public async Task<WebActionResult> WaitForElementAsync(string selector, WebElementOptions? options = null)
    {
        return await ExecuteActionAsync("WaitForElement", selector, async () =>
        {
            var element = await _currentPage!.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions
            {
                Timeout = options?.Timeout ?? _options.DefaultTimeout,
                State = WaitForSelectorState.Visible
            });

            return element != null ? "Element is visible" : "Element not found";
        });
    }

    /// <summary>
    /// Take screenshot
    /// </summary>
    public async Task<WebActionResult<byte[]>> TakeScreenshotAsync(string? path = null)
    {
        return await ExecuteActionAsync<byte[]>("Screenshot", "page", async () =>
        {
            var screenshotOptions = new PageScreenshotOptions
            {
                Path = path,
                FullPage = _options.FullPageScreenshots,
                Type = ScreenshotType.Png
            };

            var screenshot = await _currentPage!.ScreenshotAsync(screenshotOptions);
            _logger.LogDebug("Screenshot captured, size: {Size} bytes", screenshot.Length);
            
            return screenshot;
        });
    }

    /// <summary>
    /// Execute JavaScript
    /// </summary>
    public async Task<WebActionResult<T>> ExecuteJavaScriptAsync<T>(string script)
    {
        return await ExecuteActionAsync<T>("ExecuteJS", script, async () =>
        {
            var result = await _currentPage!.EvaluateAsync<T>(script);
            return result;
        });
    }

    /// <summary>
    /// Wait for page load
    /// </summary>
    public async Task<WebActionResult> WaitForPageLoadAsync(string? url = null)
    {
        return await ExecuteActionAsync("WaitForPageLoad", url ?? "current page", async () =>
        {
            await _currentPage!.WaitForLoadStateAsync(LoadState.NetworkIdle);
            
            if (!string.IsNullOrEmpty(url))
            {
                await _currentPage.WaitForURLAsync(url);
            }

            return "Page loaded successfully";
        });
    }

    /// <summary>
    /// Fill a form with data
    /// </summary>
    public async Task<WebActionResult> FillFormAsync(Dictionary<string, string> formData)
    {
        var results = new List<WebActionResult>();

        foreach (var field in formData)
        {
            var result = await TypeAsync(field.Key, field.Value);
            results.Add(result);

            if (!result.IsSuccess)
            {
                return new WebActionResult
                {
                    IsSuccess = false,
                    ActionType = "FillForm",
                    Target = "form",
                    Message = $"Failed to fill field {field.Key}: {result.Message}",
                    Exception = result.Exception
                };
            }
        }

        return new WebActionResult
        {
            IsSuccess = true,
            ActionType = "FillForm",
            Target = "form",
            Message = $"Successfully filled {formData.Count} fields"
        };
    }

    private async Task<WebActionResult> ExecuteActionAsync(string actionType, string target, Func<Task<string>> action)
    {
        if (_currentPage == null)
            await CreatePageAsync();

        _logger.LogDebug("Executing {ActionType} on {Target}", actionType, target);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            var message = await action();
            stopwatch.Stop();

            var result = new WebActionResult
            {
                IsSuccess = true,
                ActionType = actionType,
                Target = target,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                Message = message
            };

            _logger.LogDebug("{ActionType} completed successfully in {ElapsedMs}ms", 
                actionType, stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "{ActionType} failed on {Target}", actionType, target);

            return new WebActionResult
            {
                IsSuccess = false,
                ActionType = actionType,
                Target = target,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                Message = ex.Message,
                Exception = ex
            };
        }
    }

    private async Task<WebActionResult<T>> ExecuteActionAsync<T>(string actionType, string target, Func<Task<T>> action)
    {
        if (_currentPage == null)
            await CreatePageAsync();

        _logger.LogDebug("Executing {ActionType} on {Target}", actionType, target);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            var data = await action();
            stopwatch.Stop();

            var result = new WebActionResult<T>
            {
                IsSuccess = true,
                ActionType = actionType,
                Target = target,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                Message = "Action completed successfully",
                Data = data
            };

            _logger.LogDebug("{ActionType} completed successfully in {ElapsedMs}ms", 
                actionType, stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "{ActionType} failed on {Target}", actionType, target);

            return new WebActionResult<T>
            {
                IsSuccess = false,
                ActionType = actionType,
                Target = target,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                Message = ex.Message,
                Exception = ex,
                Data = default
            };
        }
    }

    /// <summary>
    /// Close browser
    /// </summary>
    public async Task CloseBrowserAsync()
    {
        if (_browser != null)
        {
            await _browser.CloseAsync();
            _browser = null;
            _currentPage = null;
            _logger.LogInformation("Browser closed");
        }
    }

    public void Dispose()
    {
        Task.Run(async () => await CloseBrowserAsync()).Wait();
    }
}

public class WebTestOptions
{
    public bool Headless { get; set; } = false;
    public float SlowMo { get; set; } = 0;
    public string[]? BrowserArgs { get; set; }
    public float LaunchTimeout { get; set; } = 30000;
    public ViewportSize? ViewportSize { get; set; } = new() { Width = 1920, Height = 1080 };
    public string? UserAgent { get; set; }
    public string? Locale { get; set; } = "en-US";
    public string? TimezoneId { get; set; }
    public float DefaultTimeout { get; set; } = 30000;
    public float NavigationTimeout { get; set; } = 30000;
    public bool FullPageScreenshots { get; set; } = false;
}

public class WebElementOptions
{
    public float? Timeout { get; set; }
    public bool Force { get; set; } = false;
    public bool Clear { get; set; } = false;
    public float TypeDelay { get; set; } = 0;
}

public class WebActionResult
{
    public bool IsSuccess { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string Target { get; set; } = string.Empty;
    public long ElapsedMilliseconds { get; set; }
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public Exception? Exception { get; set; }
}

public class WebActionResult<T> : WebActionResult
{
    public T? Data { get; set; }
}