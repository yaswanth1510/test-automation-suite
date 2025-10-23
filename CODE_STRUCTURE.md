# Code Structure Guide

This guide provides a detailed walkthrough of the codebase organization, key files, and how to navigate the repository effectively.

---

## Table of Contents

1. [Repository Organization](#repository-organization)
2. [Backend Structure](#backend-structure)
3. [Frontend Structure](#frontend-structure)
4. [Test Framework Structure](#test-framework-structure)
5. [Key Files Reference](#key-files-reference)
6. [Common Tasks](#common-tasks)
7. [Code Conventions](#code-conventions)

---

## Repository Organization

```
test-automation-suite/
├── backend/                    # .NET backend application
├── frontend/                   # React frontend application
├── TestFramework/              # Reusable test automation framework
├── docker-compose.yml          # Container orchestration
├── start.sh                    # Startup automation script
├── stop.sh                     # Shutdown script
├── package.json                # Root package configuration
└── README.md                   # Main documentation
```

---

## Backend Structure

### Directory Layout

```
backend/
├── src/
│   ├── ForexTestSuite.Api/             # API/Presentation Layer
│   ├── ForexTestSuite.Application/     # Application/Use Case Layer
│   ├── ForexTestSuite.Domain/          # Domain/Business Logic Layer
│   └── ForexTestSuite.Infrastructure/  # Infrastructure/Data Access Layer
├── tests/                              # Test projects
├── Dockerfile                          # API container image
├── Dockerfile.worker                   # Worker container image
└── ForexTestSuite.sln                  # Solution file
```

### 1. API Layer (`ForexTestSuite.Api`)

**Purpose**: HTTP endpoints, SignalR hubs, middleware configuration

```
ForexTestSuite.Api/
├── Controllers/                # REST API endpoints
│   ├── TestSuitesController.cs
│   ├── TestExecutionController.cs
│   ├── TestCasesController.cs
│   └── ReportsController.cs
│
├── Hubs/                       # SignalR real-time hubs
│   └── TestExecutionHub.cs
│
├── Middleware/                 # Custom middleware
│   ├── ExceptionHandlingMiddleware.cs
│   └── RequestLoggingMiddleware.cs
│
├── Program.cs                  # Application entry point
├── appsettings.json            # Configuration
└── appsettings.Development.json
```

**Key Files**:

**`Program.cs`**
- Application startup and configuration
- Dependency injection container setup
- Middleware pipeline configuration
- SignalR endpoint mapping

**`Controllers/TestSuitesController.cs`**
```csharp
// Example controller structure
[ApiController]
[Route("api/[controller]")]
public class TestSuitesController : ControllerBase
{
    [HttpGet]                           // GET /api/testsuites
    public async Task<ActionResult> GetTestSuites(...);
    
    [HttpGet("{id:guid}")]              // GET /api/testsuites/{id}
    public async Task<ActionResult> GetTestSuite(Guid id);
    
    [HttpPost]                          // POST /api/testsuites
    public async Task<ActionResult> CreateTestSuite(...);
    
    [HttpPut("{id:guid}")]              // PUT /api/testsuites/{id}
    public async Task<ActionResult> UpdateTestSuite(...);
    
    [HttpDelete("{id:guid}")]           // DELETE /api/testsuites/{id}
    public async Task<ActionResult> DeleteTestSuite(Guid id);
}
```

**`Hubs/TestExecutionHub.cs`**
```csharp
// SignalR hub for real-time updates
public class TestExecutionHub : Hub
{
    // Clients can call these methods from frontend
    public async Task JoinExecutionGroup(string executionId);
    public async Task LeaveExecutionGroup(string executionId);
    
    // Server broadcasts to clients
    // Clients.Group(executionId).SendAsync("TestStarted", data)
}
```

### 2. Application Layer (`ForexTestSuite.Application`)

**Purpose**: Business logic, use cases, CQRS commands/queries

```
ForexTestSuite.Application/
├── Features/                   # Feature-based organization
│   ├── TestSuites/
│   │   ├── Commands/
│   │   │   ├── CreateTestSuiteCommand.cs
│   │   │   ├── UpdateTestSuiteCommand.cs
│   │   │   ├── DeleteTestSuiteCommand.cs
│   │   │   └── TestSuiteCommands.cs
│   │   └── Queries/
│   │       ├── GetTestSuitesQuery.cs
│   │       ├── GetTestSuiteByIdQuery.cs
│   │       └── SearchTestSuitesQuery.cs
│   │
│   ├── TestExecution/
│   │   ├── Commands/
│   │   │   ├── ExecuteTestSuiteCommand.cs
│   │   │   └── StopExecutionCommand.cs
│   │   └── Queries/
│   │       ├── GetExecutionStatusQuery.cs
│   │       └── GetExecutionResultsQuery.cs
│   │
│   └── Reports/
│       └── Queries/
│           └── GenerateReportQuery.cs
│
├── Services/                   # Application services
│   ├── ITestServices.cs
│   ├── TestExecutionService.cs
│   ├── ReportGenerationService.cs
│   └── NotificationService.cs
│
├── Common/                     # Shared application code
│   ├── Interfaces.cs
│   ├── Result.cs              # Result pattern implementation
│   ├── Mappings/              # AutoMapper profiles
│   └── Behaviors/             # MediatR pipeline behaviors
│
└── DTOs/                       # Data Transfer Objects
    ├── TestSuiteDto.cs
    ├── TestCaseDto.cs
    └── ExecutionResultDto.cs
```

**CQRS Pattern Example**:

**Command** (Write operation):
```csharp
public record CreateTestSuiteCommand : IRequest<Result<TestSuiteDto>>
{
    public string Name { get; init; }
    public string Description { get; init; }
    public List<string> Tags { get; init; }
}

public class CreateTestSuiteCommandHandler 
    : IRequestHandler<CreateTestSuiteCommand, Result<TestSuiteDto>>
{
    public async Task<Result<TestSuiteDto>> Handle(
        CreateTestSuiteCommand request, 
        CancellationToken cancellationToken)
    {
        // 1. Validate
        // 2. Create domain entity
        // 3. Save to repository
        // 4. Return DTO
    }
}
```

**Query** (Read operation):
```csharp
public record GetTestSuitesQuery : IRequest<Result<List<TestSuiteDto>>>
{
    public bool ActiveOnly { get; init; }
    public List<string>? Tags { get; init; }
}

public class GetTestSuitesQueryHandler 
    : IRequestHandler<GetTestSuitesQuery, Result<List<TestSuiteDto>>>
{
    public async Task<Result<List<TestSuiteDto>>> Handle(
        GetTestSuitesQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Query database
        // 2. Apply filters
        // 3. Map to DTOs
        // 4. Return results
    }
}
```

### 3. Domain Layer (`ForexTestSuite.Domain`)

**Purpose**: Core business entities, value objects, domain logic

```
ForexTestSuite.Domain/
├── Entities/                   # Domain entities
│   ├── TestSuite.cs
│   ├── TestCase.cs
│   ├── TestExecution.cs
│   ├── TestResult.cs
│   └── ExecutionStep.cs
│
├── ValueObjects/               # Value objects
│   ├── TestConfiguration.cs
│   ├── EnvironmentSettings.cs
│   └── ExecutionMetrics.cs
│
├── Enums/                      # Domain enumerations
│   ├── TestStatus.cs
│   ├── ExecutionStatus.cs
│   └── ResultStatus.cs
│
├── Interfaces/                 # Domain interfaces
│   ├── IRepository.cs
│   ├── IUnitOfWork.cs
│   └── IDomainEvent.cs
│
└── Exceptions/                 # Domain exceptions
    ├── TestNotFoundException.cs
    └── InvalidTestStateException.cs
```

**Example Entity**:
```csharp
public class TestSuite
{
    // Properties
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    // Navigation properties
    public ICollection<TestCase> TestCases { get; private set; }
    public ICollection<TestExecution> Executions { get; private set; }
    
    // Domain methods
    public void Activate() { IsActive = true; }
    public void Deactivate() { IsActive = false; }
    public void AddTestCase(TestCase testCase) { /* logic */ }
    public void RemoveTestCase(Guid testCaseId) { /* logic */ }
}
```

### 4. Infrastructure Layer (`ForexTestSuite.Infrastructure`)

**Purpose**: Database access, external services, file storage

```
ForexTestSuite.Infrastructure/
├── Data/
│   ├── Context/
│   │   └── ForexTestSuiteDbContext.cs
│   ├── Repositories/
│   │   ├── Repository.cs             # Generic repository
│   │   ├── TestRepositories.cs       # Specific repositories
│   │   └── UnitOfWork.cs
│   ├── Configurations/               # EF Core entity configurations
│   │   ├── TestSuiteConfiguration.cs
│   │   └── TestCaseConfiguration.cs
│   └── Migrations/                   # EF Core migrations
│
├── Services/                         # External service implementations
│   ├── EmailService.cs
│   ├── StorageService.cs
│   └── CacheService.cs
│
└── ForexTestSuite.Infrastructure.csproj
```

**DbContext Example**:
```csharp
public class ForexTestSuiteDbContext : DbContext
{
    public DbSet<TestSuite> TestSuites { get; set; }
    public DbSet<TestCase> TestCases { get; set; }
    public DbSet<TestExecution> TestExecutions { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ForexTestSuiteDbContext).Assembly);
    }
}
```

---

## Frontend Structure

### Directory Layout

```
frontend/
├── public/                     # Static files
│   ├── index.html
│   ├── favicon.ico
│   └── manifest.json
│
├── src/
│   ├── components/             # React components
│   ├── services/               # API and SignalR services
│   ├── store/                  # Redux store
│   ├── types/                  # TypeScript types
│   ├── __mocks__/              # Test mocks
│   ├── __tests__/              # Test files
│   ├── App.tsx                 # Root component
│   ├── App.css
│   ├── index.tsx               # Entry point
│   └── setupTests.ts
│
├── package.json
├── tsconfig.json
├── jest.config.js
└── Dockerfile
```

### 1. Components (`src/components/`)

```
components/
├── Dashboard/
│   ├── Dashboard.tsx           # Main dashboard component
│   ├── Dashboard.test.tsx
│   └── Dashboard.css
│
├── TestSuites/
│   ├── TestSuiteList.tsx       # List of test suites
│   ├── TestSuiteDetail.tsx     # Test suite details
│   ├── TestSuiteForm.tsx       # Create/edit form
│   └── TestSuiteCard.tsx       # Card component
│
├── TestExecution/
│   ├── ExecutionStatus.tsx     # Real-time execution status
│   ├── ExecutionHistory.tsx    # Execution history
│   └── ExecutionDetails.tsx    # Detailed execution view
│
├── Reports/
│   ├── ReportViewer.tsx        # Report display
│   ├── ReportList.tsx          # List of reports
│   └── ReportGenerator.tsx     # Report generation form
│
├── Common/
│   ├── LoadingSpinner.tsx      # Loading indicator
│   ├── ErrorBoundary.tsx       # Error handling
│   ├── Notification.tsx        # Toast notifications
│   └── ConfirmDialog.tsx       # Confirmation dialogs
│
└── Layout/
    ├── NavigationBar.tsx       # Top navigation
    ├── Sidebar.tsx             # Side navigation
    └── Footer.tsx              # Footer
```

**Component Example**:
```typescript
// Dashboard.tsx
import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from '../../store/store';
import { fetchTestSuites } from '../../store/slices/testSuitesSlice';

const Dashboard: React.FC = () => {
  const dispatch = useDispatch();
  const { testSuites, isLoading, error } = useSelector(
    (state: RootState) => state.testSuites
  );

  useEffect(() => {
    dispatch(fetchTestSuites({ activeOnly: true }) as any);
  }, [dispatch]);

  if (isLoading) return <LoadingSpinner />;
  if (error) return <ErrorMessage error={error} />;

  return (
    <Box>
      {/* Dashboard content */}
    </Box>
  );
};

export default Dashboard;
```

### 2. Services (`src/services/`)

```
services/
├── apiClient.ts                # Axios configuration
├── testSuiteService.ts         # Test suite API calls
├── testExecutionService.ts     # Test execution API calls
├── reportService.ts            # Report API calls
└── SignalRContext.tsx          # SignalR connection management
```

**API Client Example**:
```typescript
// apiClient.ts
import axios from 'axios';

const apiClient = axios.create({
  baseURL: process.env.REACT_APP_API_URL || 'http://localhost:5000',
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor
apiClient.interceptors.request.use(
  (config) => {
    // Add auth token if available
    const token = localStorage.getItem('authToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response interceptor
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    // Handle errors globally
    return Promise.reject(error);
  }
);

export default apiClient;
```

**Service Example**:
```typescript
// testSuiteService.ts
import apiClient from './apiClient';
import { TestSuite, CreateTestSuiteRequest } from '../types';

export const testSuiteService = {
  getAll: async (params?: { activeOnly?: boolean; tags?: string[] }) => {
    const response = await apiClient.get<TestSuite[]>('/api/testsuites', { params });
    return response.data;
  },

  getById: async (id: string) => {
    const response = await apiClient.get<TestSuite>(`/api/testsuites/${id}`);
    return response.data;
  },

  create: async (data: CreateTestSuiteRequest) => {
    const response = await apiClient.post<TestSuite>('/api/testsuites', data);
    return response.data;
  },

  update: async (id: string, data: Partial<TestSuite>) => {
    const response = await apiClient.put<TestSuite>(`/api/testsuites/${id}`, data);
    return response.data;
  },

  delete: async (id: string) => {
    await apiClient.delete(`/api/testsuites/${id}`);
  },
};
```

**SignalR Context**:
```typescript
// SignalRContext.tsx
import React, { createContext, useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';

interface SignalRContextType {
  connection: signalR.HubConnection | null;
  isConnected: boolean;
}

export const SignalRContext = createContext<SignalRContextType>({
  connection: null,
  isConnected: false,
});

export const SignalRProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
  const [isConnected, setIsConnected] = useState(false);

  useEffect(() => {
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl(process.env.REACT_APP_SIGNALR_URL || 'http://localhost:5000/testExecutionHub')
      .withAutomaticReconnect()
      .build();

    setConnection(newConnection);

    newConnection.start()
      .then(() => {
        console.log('SignalR Connected');
        setIsConnected(true);
      })
      .catch(err => console.error('SignalR Connection Error:', err));

    return () => {
      newConnection.stop();
    };
  }, []);

  return (
    <SignalRContext.Provider value={{ connection, isConnected }}>
      {children}
    </SignalRContext.Provider>
  );
};
```

### 3. Redux Store (`src/store/`)

```
store/
├── store.ts                    # Store configuration
└── slices/
    ├── testSuitesSlice.ts      # Test suites state
    ├── testExecutionSlice.ts   # Test execution state
    ├── notificationsSlice.ts   # Notifications state
    └── uiSlice.ts              # UI state (theme, dialogs)
```

**Store Configuration**:
```typescript
// store.ts
import { configureStore } from '@reduxjs/toolkit';
import testSuitesReducer from './slices/testSuitesSlice';
import testExecutionReducer from './slices/testExecutionSlice';
import notificationsReducer from './slices/notificationsSlice';
import uiReducer from './slices/uiSlice';

export const store = configureStore({
  reducer: {
    testSuites: testSuitesReducer,
    testExecution: testExecutionReducer,
    notifications: notificationsReducer,
    ui: uiReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
```

**Slice Example**:
```typescript
// testSuitesSlice.ts
import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { testSuiteService } from '../../services/testSuiteService';
import { TestSuite } from '../../types';

interface TestSuitesState {
  testSuites: TestSuite[];
  isLoading: boolean;
  error: string | null;
}

const initialState: TestSuitesState = {
  testSuites: [],
  isLoading: false,
  error: null,
};

// Async thunk for fetching test suites
export const fetchTestSuites = createAsyncThunk(
  'testSuites/fetchAll',
  async (params: { activeOnly?: boolean; tags?: string[] }) => {
    return await testSuiteService.getAll(params);
  }
);

const testSuitesSlice = createSlice({
  name: 'testSuites',
  initialState,
  reducers: {
    clearError: (state) => {
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchTestSuites.pending, (state) => {
        state.isLoading = true;
        state.error = null;
      })
      .addCase(fetchTestSuites.fulfilled, (state, action) => {
        state.isLoading = false;
        state.testSuites = action.payload;
      })
      .addCase(fetchTestSuites.rejected, (state, action) => {
        state.isLoading = false;
        state.error = action.error.message || 'Failed to fetch test suites';
      });
  },
});

export const { clearError } = testSuitesSlice.actions;
export default testSuitesSlice.reducer;
```

### 4. TypeScript Types (`src/types/`)

```typescript
// types/index.ts
export interface TestSuite {
  id: string;
  name: string;
  description: string;
  version: string;
  isActive: boolean;
  tags: string[];
  testCases?: TestCase[];
  createdAt: string;
  updatedAt?: string;
}

export interface TestCase {
  id: string;
  testSuiteId: string;
  name: string;
  description: string;
  priority: 'Low' | 'Medium' | 'High' | 'Critical';
  isEnabled: boolean;
  tags: string[];
}

export interface TestExecution {
  id: string;
  testSuiteId: string;
  status: 'Queued' | 'Running' | 'Completed' | 'Failed' | 'Cancelled';
  startTime: string;
  endTime?: string;
  environment: Record<string, string>;
  results: TestResult[];
}

export interface TestResult {
  testCaseId: string;
  status: 'Passed' | 'Failed' | 'Skipped';
  duration: number;
  errorMessage?: string;
  screenshots: string[];
}
```

---

## Test Framework Structure

```
TestFramework/
├── Core/                       # Core framework components
│   ├── StepManager.cs
│   ├── ParameterGenerator.cs
│   └── ComparisonTool.cs
│
├── Clients/                    # Test client implementations
│   ├── HttpTestClient.cs
│   ├── SqlTestClient.cs
│   └── WebTestClient.cs
│
├── Modules/                    # Domain-specific test modules
│   └── ForexTradingTestModule.cs
│
├── Tests/                      # Sample test implementations
│   ├── SampleForexTradingTests.cs
│   └── ForexTestFramework.Tests.csproj
│
└── ForexTestFramework.csproj
```

### Key Components

**StepManager**: Orchestrates test step execution
**ParameterGenerator**: Generates dynamic test data
**ComparisonTool**: Validates data and results
**Test Clients**: Interface with different systems (HTTP, SQL, Web)

---

## Key Files Reference

### Configuration Files

| File | Purpose | Location |
|------|---------|----------|
| `appsettings.json` | Backend configuration | `backend/src/ForexTestSuite.Api/` |
| `package.json` | Frontend dependencies | `frontend/` |
| `tsconfig.json` | TypeScript configuration | `frontend/` |
| `docker-compose.yml` | Container orchestration | Root |
| `.gitignore` | Git ignore rules | Root |

### Entry Points

| Component | Entry Point | Location |
|-----------|-------------|----------|
| Backend API | `Program.cs` | `backend/src/ForexTestSuite.Api/` |
| Frontend | `index.tsx` | `frontend/src/` |
| Test Framework | Sample tests | `TestFramework/Tests/` |

### Important Scripts

| Script | Purpose | Location |
|--------|---------|----------|
| `start.sh` | Start all services | Root |
| `stop.sh` | Stop all services | Root |

---

## Common Tasks

### Adding a New API Endpoint

1. **Create Command/Query** in `Application/Features/`
2. **Create Handler** implementing `IRequestHandler<TRequest, TResponse>`
3. **Add Controller Action** in `Api/Controllers/`
4. **Test the endpoint**

### Adding a New React Component

1. **Create component file** in `frontend/src/components/`
2. **Define props interface** (TypeScript)
3. **Implement component** with hooks
4. **Add tests** in `__tests__/`
5. **Export and use** in parent component

### Adding a New Redux Slice

1. **Create slice file** in `frontend/src/store/slices/`
2. **Define state interface**
3. **Create async thunks** for API calls
4. **Define reducers**
5. **Add to store** configuration

### Adding a New Test Case

1. **Create test class** in `TestFramework/Tests/`
2. **Use `[TestFixture]` attribute**
3. **Implement test methods** with `[Test]` attribute
4. **Use framework components** (StepManager, ParameterGenerator, etc.)
5. **Run tests** with `dotnet test`

---

## Code Conventions

### Backend (C#)

- **PascalCase** for classes, methods, properties
- **camelCase** for local variables, parameters
- **Async suffix** for async methods (e.g., `GetTestSuiteAsync`)
- **Interface prefix** `I` for interfaces
- **Private fields** prefix `_` (e.g., `_logger`)
- **XML documentation** for public APIs

### Frontend (TypeScript/React)

- **PascalCase** for components, interfaces, types
- **camelCase** for functions, variables
- **Props interface** named `{Component}Props`
- **Functional components** with hooks
- **Arrow functions** for component definitions
- **Named exports** for components

### File Naming

- **Backend**: `PascalCase.cs` (e.g., `TestSuite.cs`)
- **Frontend**: `PascalCase.tsx` for components, `camelCase.ts` for utilities
- **Tests**: `{Name}.test.tsx` or `{Name}Tests.cs`

---

## Navigation Tips

### Finding Specific Functionality

**Looking for API endpoints?**
→ `backend/src/ForexTestSuite.Api/Controllers/`

**Looking for business logic?**
→ `backend/src/ForexTestSuite.Application/Features/`

**Looking for database queries?**
→ `backend/src/ForexTestSuite.Infrastructure/Data/`

**Looking for React components?**
→ `frontend/src/components/`

**Looking for API calls?**
→ `frontend/src/services/`

**Looking for state management?**
→ `frontend/src/store/slices/`

**Looking for test examples?**
→ `TestFramework/Tests/`

### Following a Request Flow

1. **User Action** → React Component (`frontend/src/components/`)
2. **Dispatch Action** → Redux Slice (`frontend/src/store/slices/`)
3. **API Call** → Service (`frontend/src/services/`)
4. **HTTP Request** → Controller (`backend/src/Api/Controllers/`)
5. **Command/Query** → Handler (`backend/src/Application/Features/`)
6. **Business Logic** → Domain (`backend/src/Domain/`)
7. **Data Access** → Repository (`backend/src/Infrastructure/`)
8. **Database** → SQL Server

---

## Development Workflow

### Setting Up Development Environment

1. Clone repository
2. Install prerequisites (.NET 8, Node 18+, Docker)
3. Start infrastructure: `docker-compose up -d sqlserver redis minio`
4. Run backend: `cd backend/src/ForexTestSuite.Api && dotnet run`
5. Run frontend: `cd frontend && npm install && npm start`

### Making Changes

1. Create feature branch
2. Make changes following conventions
3. Write/update tests
4. Run tests locally
5. Commit with descriptive message
6. Push and create pull request

### Testing

**Backend**: `dotnet test`
**Frontend**: `npm test`
**Test Framework**: `cd TestFramework && dotnet test`

---

## Conclusion

This code structure guide provides a roadmap for navigating and contributing to the Forex Test Automation Suite. The codebase follows industry best practices with clear separation of concerns, making it maintainable and scalable.

For more information:
- See [README.md](README.md) for general documentation
- See [REPOSITORY_ANALYSIS.md](REPOSITORY_ANALYSIS.md) for detailed analysis
- See [ARCHITECTURE.md](ARCHITECTURE.md) for architectural diagrams
