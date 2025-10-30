using ForexTestSuite.Application.Common;
using ForexTestSuite.Domain.Interfaces;
using MediatR;

namespace ForexTestSuite.Application.Features.TestSuites.Queries;

public class GetTestSuitesQueryHandler : IRequestHandler<GetTestSuitesQuery, Result<IEnumerable<TestSuiteDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTestSuitesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<TestSuiteDto>>> Handle(GetTestSuitesQuery request, CancellationToken cancellationToken)
    {
        var testSuites = await _unitOfWork.TestSuites.GetAllAsync(cancellationToken);
        
        if (request.ActiveOnly)
        {
            testSuites = testSuites.Where(ts => ts.IsActive);
        }
        
        if (request.Tags != null && request.Tags.Any())
        {
            testSuites = testSuites.Where(ts => request.Tags.Any(tag => ts.Tags.Contains(tag)));
        }
        
        var dtos = testSuites.Select(ts => new TestSuiteDto
        {
            Id = ts.Id,
            Name = ts.Name,
            Description = ts.Description,
            Version = ts.Version,
            IsActive = ts.IsActive,
            Tags = ts.Tags,
            Configuration = ts.Configuration,
            CreatedAt = ts.CreatedAt,
            UpdatedAt = ts.UpdatedAt
        });
        
        return Result<IEnumerable<TestSuiteDto>>.Success(dtos);
    }
}

public class GetTestSuiteByIdQueryHandler : IRequestHandler<GetTestSuiteByIdQuery, Result<TestSuiteDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTestSuiteByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TestSuiteDto>> Handle(GetTestSuiteByIdQuery request, CancellationToken cancellationToken)
    {
        var testSuite = await _unitOfWork.TestSuites.GetByIdAsync(request.Id, cancellationToken);
        
        if (testSuite == null)
        {
            return Result<TestSuiteDto>.Failure($"Test suite with ID {request.Id} not found");
        }
        
        var dto = new TestSuiteDto
        {
            Id = testSuite.Id,
            Name = testSuite.Name,
            Description = testSuite.Description,
            Version = testSuite.Version,
            IsActive = testSuite.IsActive,
            Tags = testSuite.Tags,
            Configuration = testSuite.Configuration,
            CreatedAt = testSuite.CreatedAt,
            UpdatedAt = testSuite.UpdatedAt
        };
        
        if (request.IncludeTestCases)
        {
            var testCases = await _unitOfWork.TestCases.GetByTestSuiteIdAsync(testSuite.Id, cancellationToken);
            dto.TestCases = testCases.Select(tc => new TestCaseDto
            {
                Id = tc.Id,
                Name = tc.Name,
                Description = tc.Description,
                TestType = tc.TestType.ToString(),
                FilePath = tc.FilePath,
                ClassName = tc.ClassName,
                MethodName = tc.MethodName,
                Priority = tc.Priority,
                TimeoutSeconds = tc.TimeoutSeconds,
                IsParallelizable = tc.IsParallelizable,
                Tags = tc.Tags,
                Parameters = tc.Parameters,
                Environment = tc.Environment,
                CreatedAt = tc.CreatedAt
            }).ToList();
        }
        
        return Result<TestSuiteDto>.Success(dto);
    }
}