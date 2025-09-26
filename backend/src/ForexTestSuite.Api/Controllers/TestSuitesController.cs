using MediatR;
using Microsoft.AspNetCore.Mvc;
using ForexTestSuite.Application.Features.TestSuites.Queries;
using ForexTestSuite.Application.Features.TestSuites.Commands;

namespace ForexTestSuite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestSuitesController : ControllerBase
{
    private readonly IMediator _mediator;

    public TestSuitesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all test suites
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TestSuiteDto>>> GetTestSuites(
        [FromQuery] bool activeOnly = true,
        [FromQuery] List<string>? tags = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTestSuitesQuery { ActiveOnly = activeOnly, Tags = tags };
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsSuccess)
            return Ok(result.Value);

        return BadRequest(result.ErrorMessage);
    }

    /// <summary>
    /// Get test suite by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TestSuiteDto>> GetTestSuite(
        Guid id,
        [FromQuery] bool includeTestCases = true,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTestSuiteByIdQuery { Id = id, IncludeTestCases = includeTestCases };
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsSuccess)
            return Ok(result.Value);

        if (result.ErrorMessage?.Contains("not found") == true)
            return NotFound(result.ErrorMessage);

        return BadRequest(result.ErrorMessage);
    }

    /// <summary>
    /// Create a new test suite
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateTestSuite(
        CreateTestSuiteCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetTestSuite), new { id = result.Value }, result.Value);

        return BadRequest(result.ErrorMessage);
    }

    /// <summary>
    /// Update an existing test suite
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateTestSuite(
        Guid id,
        UpdateTestSuiteCommand command,
        CancellationToken cancellationToken = default)
    {
        command.Id = id;
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
            return NoContent();

        if (result.ErrorMessage?.Contains("not found") == true)
            return NotFound(result.ErrorMessage);

        return BadRequest(result.ErrorMessage);
    }

    /// <summary>
    /// Delete a test suite
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteTestSuite(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteTestSuiteCommand { Id = id };
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
            return NoContent();

        if (result.ErrorMessage?.Contains("not found") == true)
            return NotFound(result.ErrorMessage);

        return BadRequest(result.ErrorMessage);
    }
}