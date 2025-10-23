# Architecture Documentation

## System Architecture Overview

This document provides detailed architectural diagrams and explanations for the Forex Test Automation Suite.

---

## 1. High-Level System Architecture

```
┌─────────────────────────────────────────────────────────────────────┐
│                           CLIENT TIER                               │
│                                                                     │
│  ┌─────────────────────────────────────────────────────────────┐  │
│  │                    Web Browser                               │  │
│  │                                                               │  │
│  │  ┌─────────────────────────────────────────────────────┐    │  │
│  │  │         React Application (Port 3000)               │    │  │
│  │  │                                                       │    │  │
│  │  │  • Material-UI Components                           │    │  │
│  │  │  • Redux State Management                            │    │  │
│  │  │  • Chart.js Visualizations                           │    │  │
│  │  │  • SignalR Client (WebSocket)                        │    │  │
│  │  └─────────────────────────────────────────────────────┘    │  │
│  └─────────────────────────────────────────────────────────────┘  │
└────────────────────────┬────────────────────────────────────────────┘
                         │ HTTP/HTTPS (REST)
                         │ WebSocket (SignalR)
┌────────────────────────┴────────────────────────────────────────────┐
│                        APPLICATION TIER                             │
│                                                                     │
│  ┌─────────────────────────────────────────────────────────────┐  │
│  │          ASP.NET Core Web API (Port 5000/5001)             │  │
│  │                                                             │  │
│  │  ┌────────────────┐  ┌─────────────────┐  ┌─────────────┐ │  │
│  │  │  Controllers   │  │  SignalR Hubs   │  │  Middleware │ │  │
│  │  │  (REST APIs)   │  │  (Real-time)    │  │  Pipeline   │ │  │
│  │  └────────┬───────┘  └────────┬────────┘  └─────────────┘ │  │
│  │           │                    │                            │  │
│  │  ┌────────┴────────────────────┴────────────────────────┐  │  │
│  │  │           Application Layer (CQRS)                   │  │  │
│  │  │                                                       │  │  │
│  │  │  ┌──────────────┐         ┌──────────────┐          │  │  │
│  │  │  │  Commands    │         │   Queries    │          │  │  │
│  │  │  │  (Write)     │         │   (Read)     │          │  │  │
│  │  │  └──────┬───────┘         └──────┬───────┘          │  │  │
│  │  │         │                         │                   │  │  │
│  │  │  ┌──────┴─────────────────────────┴───────┐          │  │  │
│  │  │  │         MediatR Pipeline               │          │  │  │
│  │  │  └────────────────┬───────────────────────┘          │  │  │
│  │  │                   │                                   │  │  │
│  │  │  ┌────────────────┴───────────────────────┐          │  │  │
│  │  │  │         Domain Layer                   │          │  │  │
│  │  │  │  • Entities                            │          │  │  │
│  │  │  │  • Value Objects                       │          │  │  │
│  │  │  │  • Business Rules                      │          │  │  │
│  │  │  └────────────────┬───────────────────────┘          │  │  │
│  │  └───────────────────┼──────────────────────────────────┘  │  │
│  │                      │                                      │  │
│  │  ┌───────────────────┴──────────────────────────────────┐  │  │
│  │  │         Infrastructure Layer                         │  │  │
│  │  │                                                       │  │  │
│  │  │  • Entity Framework Core (ORM)                       │  │  │
│  │  │  • Repository Implementation                         │  │  │
│  │  │  • External Service Clients                          │  │  │
│  │  └───────────────────────────────────────────────────────┘  │  │
│  └─────────────────────────────────────────────────────────────┘  │
│                                                                     │
│  ┌─────────────────────────────────────────────────────────────┐  │
│  │         Background Worker (Hangfire)                        │  │
│  │                                                             │  │
│  │  • Scheduled Test Execution                                │  │
│  │  • Async Job Processing                                     │  │
│  │  • Queue Management                                         │  │
│  └─────────────────────────────────────────────────────────────┘  │
│                                                                     │
│  ┌─────────────────────────────────────────────────────────────┐  │
│  │         Test Framework (NUnit)                              │  │
│  │                                                             │  │
│  │  ┌──────────────┐  ┌─────────────┐  ┌─────────────────┐   │  │
│  │  │ StepManager  │  │  Parameter  │  │  Comparison     │   │  │
│  │  │              │  │  Generator  │  │  Tool           │   │  │
│  │  └──────────────┘  └─────────────┘  └─────────────────┘   │  │
│  │                                                             │  │
│  │  ┌──────────────┐  ┌─────────────┐  ┌─────────────────┐   │  │
│  │  │ HttpTest     │  │  SqlTest    │  │  WebTest        │   │  │
│  │  │ Client       │  │  Client     │  │  Client         │   │  │
│  │  └──────────────┘  └─────────────┘  └─────────────────┘   │  │
│  └─────────────────────────────────────────────────────────────┘  │
└────────────────────────┬────────────────────────────────────────────┘
                         │
┌────────────────────────┴────────────────────────────────────────────┐
│                         DATA TIER                                   │
│                                                                     │
│  ┌────────────────┐  ┌────────────────┐  ┌───────────────────┐    │
│  │  SQL Server    │  │  Redis Cache   │  │  MinIO Storage    │    │
│  │  (Port 1433)   │  │  (Port 6379)   │  │  (Port 9000/9001) │    │
│  │                │  │                │  │                   │    │
│  │  • Test Data   │  │  • Session     │  │  • Screenshots    │    │
│  │  • Results     │  │  • Cache       │  │  • Reports        │    │
│  │  • Config      │  │  • Locks       │  │  • Logs           │    │
│  └────────────────┘  └────────────────┘  └───────────────────┘    │
└─────────────────────────────────────────────────────────────────────┘
```

---

## 2. Clean Architecture Dependency Flow

```
┌────────────────────────────────────────────────────────────┐
│                    Presentation Layer                      │
│                   (API Controllers, Hubs)                  │
│                                                            │
│                 ↓ depends on ↓                             │
└────────────────────────────────────────────────────────────┘
                           │
┌──────────────────────────┴─────────────────────────────────┐
│                   Application Layer                        │
│              (Use Cases, Commands, Queries)                │
│                                                            │
│              ↓ depends on ↓                                │
└────────────────────────────────────────────────────────────┘
                           │
┌──────────────────────────┴─────────────────────────────────┐
│                      Domain Layer                          │
│           (Entities, Value Objects, Interfaces)            │
│                                                            │
│           ↑ implemented by ↑                               │
└────────────────────────────────────────────────────────────┘
                           │
┌──────────────────────────┴─────────────────────────────────┐
│                  Infrastructure Layer                      │
│        (EF Core, Repositories, External Services)          │
└────────────────────────────────────────────────────────────┘

Key Principle: Dependencies point INWARD
- Infrastructure depends on Domain
- Application depends on Domain
- Domain depends on NOTHING
```

---

## 3. CQRS Pattern Implementation

```
┌─────────────────────────────────────────────────────────────┐
│                        Client Request                       │
└────────────────────┬────────────────────────────────────────┘
                     │
         ┌───────────┴──────────┐
         │                      │
    Write Path              Read Path
         │                      │
         ▼                      ▼
┌──────────────────┐   ┌──────────────────┐
│    Command       │   │     Query        │
│                  │   │                  │
│  • Create        │   │  • GetById       │
│  • Update        │   │  • GetList       │
│  • Delete        │   │  • Search        │
│  • Execute       │   │  • Filter        │
└────────┬─────────┘   └────────┬─────────┘
         │                      │
         ▼                      ▼
┌──────────────────┐   ┌──────────────────┐
│ Command Handler  │   │  Query Handler   │
│                  │   │                  │
│  • Validation    │   │  • Projection    │
│  • Business      │   │  • Mapping       │
│    Logic         │   │  • Filtering     │
│  • Persistence   │   │                  │
└────────┬─────────┘   └────────┬─────────┘
         │                      │
         ▼                      ▼
┌──────────────────┐   ┌──────────────────┐
│   Repository     │   │   Read Models    │
│   (Write)        │   │   (Read)         │
└────────┬─────────┘   └────────┬─────────┘
         │                      │
         └───────────┬──────────┘
                     │
                     ▼
            ┌────────────────┐
            │    Database    │
            └────────────────┘

Benefits:
✓ Optimized read and write operations
✓ Scalable independently
✓ Clear separation of concerns
✓ Better testability
```

---

## 4. SignalR Real-Time Communication Flow

```
┌──────────────────────────────────────────────────────────────┐
│                        Test Execution                        │
└────────────────────┬─────────────────────────────────────────┘
                     │
                     ▼
        ┌────────────────────────┐
        │  Test Execution Engine │
        └────────────┬────────────┘
                     │
                     │ Publishes Events
                     ▼
        ┌────────────────────────┐
        │   SignalR Hub          │
        │  (TestExecutionHub)    │
        └────────────┬────────────┘
                     │
           Broadcast to all clients
                     │
         ┌───────────┼───────────┐
         │           │           │
         ▼           ▼           ▼
    ┌────────┐  ┌────────┐  ┌────────┐
    │Client 1│  │Client 2│  │Client N│
    └───┬────┘  └───┬────┘  └───┬────┘
        │           │           │
        ▼           ▼           ▼
    ┌────────────────────────────────┐
    │   SignalR Client Context       │
    └───────────┬────────────────────┘
                │
                ▼
    ┌────────────────────────────────┐
    │     Redux Store Update         │
    └───────────┬────────────────────┘
                │
                ▼
    ┌────────────────────────────────┐
    │     React Component Render     │
    └────────────────────────────────┘

Event Types:
• TestStarted
• TestStepCompleted
• TestCompleted
• TestFailed
• ProgressUpdate
```

---

## 5. Test Execution Workflow

```
┌─────────────────┐
│  User Trigger   │
│  via Web UI     │
└────────┬────────┘
         │
         ▼
┌─────────────────────────────────────┐
│  Frontend: Dispatch Redux Action    │
└────────┬────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────┐
│  API Call: POST /api/execution      │
└────────┬────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────┐
│  Controller: Route to Handler       │
└────────┬────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────┐
│  Execute Test Command Handler       │
│                                     │
│  1. Validate Request                │
│  2. Load Test Suite                 │
│  3. Queue Background Job            │
│  4. Return Execution ID             │
└────────┬────────────────────────────┘
         │
         ├─────────────────────┐
         │                     │
         ▼                     ▼
┌────────────────┐   ┌──────────────────────┐
│ Return to UI   │   │  Hangfire Worker     │
│ with Exec ID   │   │  Processes Job       │
└────────────────┘   └─────────┬────────────┘
                               │
                               ▼
                     ┌─────────────────────┐
                     │ Test Framework      │
                     │ Execution Engine    │
                     │                     │
                     │ 1. Load Test Suite  │
                     │ 2. Initialize Env   │
                     │ 3. Run Tests        │
                     │ 4. Collect Results  │
                     └─────────┬───────────┘
                               │
                ┌──────────────┼──────────────┐
                │              │              │
                ▼              ▼              ▼
         ┌───────────┐  ┌───────────┐  ┌──────────┐
         │ SignalR   │  │ Save to   │  │ Upload   │
         │ Updates   │  │ Database  │  │ Artifacts│
         └───────────┘  └───────────┘  └──────────┘
                │              │              │
                └──────────────┴──────────────┘
                               │
                               ▼
                     ┌─────────────────────┐
                     │ Generate Reports    │
                     └─────────┬───────────┘
                               │
                               ▼
                     ┌─────────────────────┐
                     │ Notify Completion   │
                     └─────────────────────┘
```

---

## 6. Data Access Pattern (Repository + Unit of Work)

```
┌───────────────────────────────────────────────────────┐
│                  Application Layer                    │
│                   (Use Case)                          │
└────────────────────────┬──────────────────────────────┘
                         │
                         ▼
              ┌──────────────────────┐
              │   IUnitOfWork        │
              │                      │
              │  BeginTransaction()  │
              │  Commit()            │
              │  Rollback()          │
              └─────────┬────────────┘
                        │
        ┌───────────────┼───────────────┐
        │               │               │
        ▼               ▼               ▼
┌──────────────┐ ┌──────────────┐ ┌──────────────┐
│ ITestSuite   │ │ ITestCase    │ │ IExecution   │
│ Repository   │ │ Repository   │ │ Repository   │
└──────┬───────┘ └──────┬───────┘ └──────┬───────┘
       │                │                │
       └────────────────┴────────────────┘
                        │
                        ▼
              ┌──────────────────────┐
              │  DbContext (EF Core) │
              │                      │
              │  • TestSuites        │
              │  • TestCases         │
              │  • Executions        │
              │  • Results           │
              └─────────┬────────────┘
                        │
                        ▼
              ┌──────────────────────┐
              │    SQL Server        │
              └──────────────────────┘

Repository Methods:
• GetById(id)
• GetAll()
• Add(entity)
• Update(entity)
• Delete(id)
• FindBy(predicate)

Unit of Work ensures:
✓ Transaction management
✓ Multiple repository coordination
✓ Single commit point
✓ Rollback capability
```

---

## 7. Frontend State Management (Redux)

```
┌───────────────────────────────────────────────────────────┐
│                      React Components                     │
│                                                           │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌─────────┐  │
│  │Dashboard │  │TestSuite │  │Execution │  │ Reports │  │
│  │          │  │  List    │  │  Status  │  │         │  │
│  └────┬─────┘  └────┬─────┘  └────┬─────┘  └────┬────┘  │
│       │             │              │             │        │
│       │   useSelector (Read State) │             │        │
│       │   useDispatch (Dispatch)   │             │        │
│       └─────────────┴──────────────┴─────────────┘        │
└───────────────────────┬───────────────────────────────────┘
                        │
                        ▼
┌───────────────────────────────────────────────────────────┐
│                      Redux Store                          │
│                                                           │
│  ┌─────────────────────────────────────────────────────┐ │
│  │              Root Reducer                           │ │
│  │                                                     │ │
│  │  ┌───────────┐  ┌──────────┐  ┌──────────────┐    │ │
│  │  │testSuites │  │testExec  │  │notifications │    │ │
│  │  │  Slice    │  │  Slice   │  │   Slice      │    │ │
│  │  └─────┬─────┘  └────┬─────┘  └──────┬───────┘    │ │
│  │        │             │               │            │ │
│  │   State Tree:                                     │ │
│  │   {                                                │ │
│  │     testSuites: { items, loading, error },        │ │
│  │     testExecution: { results, liveExecutions },   │ │
│  │     notifications: { messages, unreadCount },     │ │
│  │     ui: { theme, dialogs }                        │ │
│  │   }                                                │ │
│  └─────────────────────────────────────────────────────┘ │
└───────────────────────┬───────────────────────────────────┘
                        │
            Actions (Async Thunks)
                        │
        ┌───────────────┼───────────────┐
        │               │               │
        ▼               ▼               ▼
┌──────────────┐ ┌──────────────┐ ┌──────────────┐
│ API Service  │ │ SignalR      │ │ Local        │
│              │ │ Context      │ │ Storage      │
└──────────────┘ └──────────────┘ └──────────────┘

Redux Toolkit Features:
✓ Slice-based state management
✓ Immutable state updates
✓ Built-in async actions (createAsyncThunk)
✓ Redux DevTools integration
✓ Automatic action creators
```

---

## 8. Test Framework Component Architecture

```
┌──────────────────────────────────────────────────────────┐
│                     Test Suite                           │
│                                                          │
│  ┌────────────────────────────────────────────────────┐ │
│  │              Test Case 1                           │ │
│  │                                                    │ │
│  │  ┌──────────┐  ┌──────────┐  ┌──────────┐       │ │
│  │  │  Step 1  │→ │  Step 2  │→ │  Step 3  │       │ │
│  │  └──────────┘  └──────────┘  └──────────┘       │ │
│  └────────────────────────────────────────────────────┘ │
│                                                          │
│  ┌────────────────────────────────────────────────────┐ │
│  │              Test Case 2                           │ │
│  └────────────────────────────────────────────────────┘ │
└──────────────────────┬───────────────────────────────────┘
                       │
                       ▼
         ┌─────────────────────────┐
         │     Step Manager        │
         │                         │
         │  • Register Steps       │
         │  • Execute Steps        │
         │  • Track Results        │
         │  • Manage Flow          │
         └────────┬────────────────┘
                  │
      ┌───────────┼───────────┐
      │           │           │
      ▼           ▼           ▼
┌──────────┐ ┌──────────┐ ┌──────────┐
│Parameter │ │Comparison│ │Test      │
│Generator │ │  Tool    │ │Clients   │
└──────────┘ └──────────┘ └────┬─────┘
                                │
                  ┌─────────────┼─────────────┐
                  │             │             │
                  ▼             ▼             ▼
            ┌─────────┐   ┌─────────┐   ┌─────────┐
            │  HTTP   │   │   SQL   │   │   Web   │
            │ Client  │   │ Client  │   │ Client  │
            └─────────┘   └─────────┘   └─────────┘
                  │             │             │
                  └─────────────┴─────────────┘
                                │
                                ▼
                    ┌────────────────────────┐
                    │  System Under Test     │
                    │  (Forex Trading App)   │
                    └────────────────────────┘

Component Responsibilities:

StepManager:
• Central orchestration
• Sequential/parallel execution
• Result aggregation
• History tracking

ParameterGenerator:
• Dynamic test data
• Forex-specific values
• Configurable constraints
• Bulk generation

ComparisonTool:
• Data validation
• JSON/XML comparison
• Numeric tolerance
• Deep object comparison

Test Clients:
• HTTP: API testing
• SQL: Database validation
• Web: Browser automation
```

---

## 9. Deployment Architecture (Docker)

```
┌─────────────────────────────────────────────────────────────┐
│                      Docker Host                            │
│                                                             │
│  ┌──────────────────────────────────────────────────────┐  │
│  │              Docker Compose Network                  │  │
│  │              (forex-network - bridge)                │  │
│  │                                                      │  │
│  │  ┌────────────┐  ┌────────────┐  ┌────────────┐    │  │
│  │  │  Frontend  │  │     API    │  │   Worker   │    │  │
│  │  │  (Nginx)   │  │  (.NET 8)  │  │ (Hangfire) │    │  │
│  │  │            │  │            │  │            │    │  │
│  │  │  Port:3000 │  │ Port:5000  │  │  (no port) │    │  │
│  │  └──────┬─────┘  └──────┬─────┘  └──────┬─────┘    │  │
│  │         │                │                │          │  │
│  │         └────────────────┴────────────────┘          │  │
│  │                          │                           │  │
│  │         ┌────────────────┼────────────────┐          │  │
│  │         │                │                │          │  │
│  │  ┌──────┴─────┐  ┌───────┴──────┐  ┌─────┴──────┐  │  │
│  │  │ SQL Server │  │    Redis     │  │   MinIO    │  │  │
│  │  │            │  │              │  │            │  │  │
│  │  │ Port: 1433 │  │  Port: 6379  │  │ Port: 9000 │  │  │
│  │  │            │  │              │  │ Port: 9001 │  │  │
│  │  └────────────┘  └──────────────┘  └────────────┘  │  │
│  │                                                      │  │
│  │  Volumes:                                            │  │
│  │  • sqlserver_data (persistent)                      │  │
│  │  • redis_data (persistent)                          │  │
│  │  • minio_data (persistent)                          │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘

Container Communication:
• All containers on same network
• Service discovery by container name
• No direct port exposure for internal services
• Volumes for data persistence
• Health checks configured
```

---

## 10. Security Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                      External Clients                       │
└─────────────────────────┬───────────────────────────────────┘
                          │
                          ▼
                 ┌──────────────────┐
                 │  HTTPS (TLS 1.3) │
                 └────────┬──────────┘
                          │
                          ▼
        ┌─────────────────────────────────┐
        │       API Gateway/Nginx         │
        │  • Rate Limiting                │
        │  • CORS Configuration           │
        │  • Request Logging              │
        └────────┬────────────────────────┘
                 │
                 ▼
        ┌─────────────────────────────────┐
        │      Authentication             │
        │  • JWT Token Validation         │
        │  • API Key Verification         │
        └────────┬────────────────────────┘
                 │
                 ▼
        ┌─────────────────────────────────┐
        │      Authorization              │
        │  • Role-Based Access Control    │
        │  • Resource Permissions         │
        └────────┬────────────────────────┘
                 │
                 ▼
        ┌─────────────────────────────────┐
        │      Input Validation           │
        │  • FluentValidation             │
        │  • XSS Prevention               │
        │  • SQL Injection Prevention     │
        └────────┬────────────────────────┘
                 │
                 ▼
        ┌─────────────────────────────────┐
        │      Business Logic             │
        └────────┬────────────────────────┘
                 │
                 ▼
        ┌─────────────────────────────────┐
        │      Data Access                │
        │  • Parameterized Queries        │
        │  • Connection Encryption        │
        └────────┬────────────────────────┘
                 │
                 ▼
        ┌─────────────────────────────────┐
        │      Audit Logging              │
        │  • All Actions Logged           │
        │  • Sensitive Data Masked        │
        └─────────────────────────────────┘

Security Measures:
✓ HTTPS/TLS encryption
✓ Authentication & Authorization
✓ Input validation
✓ SQL injection prevention (EF Core)
✓ XSS prevention
✓ CSRF protection
✓ Rate limiting
✓ Audit logging
✓ Secrets management (Environment variables)
```

---

## 11. Performance & Scalability

```
┌─────────────────────────────────────────────────────────────┐
│                    Load Balancer (Future)                   │
└────────────┬────────────────────────────────────────────────┘
             │
      ┌──────┴──────┬──────────┬──────────┐
      │             │          │          │
      ▼             ▼          ▼          ▼
┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐
│API Node 1│  │API Node 2│  │API Node 3│  │API Node N│
└────┬─────┘  └────┬─────┘  └────┬─────┘  └────┬─────┘
     │             │              │             │
     └─────────────┴──────────────┴─────────────┘
                   │
     ┌─────────────┼─────────────┐
     │             │             │
     ▼             ▼             ▼
┌─────────┐  ┌──────────┐  ┌──────────┐
│  Redis  │  │   SQL    │  │  MinIO   │
│  Cache  │  │  Server  │  │ Storage  │
│         │  │          │  │          │
│ Session │  │ Primary  │  │ Artifacts│
│ Store   │  │  Read    │  │          │
└─────────┘  └────┬─────┘  └──────────┘
                  │
            ┌─────┴─────┐
            │           │
      ┌─────▼────┐ ┌────▼─────┐
      │ Read     │ │ Read     │
      │ Replica 1│ │ Replica N│
      └──────────┘ └──────────┘

Performance Optimizations:

Caching Strategy:
• Redis for session state
• Response caching
• Query result caching
• Static asset CDN

Database Optimization:
• Read replicas for queries
• Connection pooling
• Index optimization
• Query optimization

Async Processing:
• Background jobs (Hangfire)
• Queue-based execution
• Non-blocking operations

Horizontal Scaling:
• Stateless API design
• Container orchestration ready
• Database replication
• Distributed caching
```

---

## 12. Monitoring & Observability

```
┌─────────────────────────────────────────────────────────────┐
│                      Application Tier                       │
│                                                             │
│  ┌──────────────────────────────────────────────────────┐  │
│  │              Logging (Serilog)                       │  │
│  │  • Structured logging                                │  │
│  │  • Multiple sinks (Console, File, Database)         │  │
│  │  • Log levels                                        │  │
│  └─────────────────────┬────────────────────────────────┘  │
│                        │                                    │
│  ┌─────────────────────┴────────────────────────────────┐  │
│  │              Application Insights (Optional)         │  │
│  │  • Performance monitoring                            │  │
│  │  • Exception tracking                                │  │
│  │  • Custom metrics                                    │  │
│  └─────────────────────┬────────────────────────────────┘  │
│                        │                                    │
│  ┌─────────────────────┴────────────────────────────────┐  │
│  │              Health Checks                           │  │
│  │  • /health endpoint                                  │  │
│  │  • Database connectivity                             │  │
│  │  • Redis availability                                │  │
│  │  • Storage accessibility                             │  │
│  └─────────────────────┬────────────────────────────────┘  │
└────────────────────────┼────────────────────────────────────┘
                         │
                         ▼
            ┌────────────────────────┐
            │   Hangfire Dashboard   │
            │  • Job monitoring      │
            │  • Queue statistics    │
            │  • Failed job tracking │
            └────────────────────────┘

Observability Features:
✓ Structured logging with correlation IDs
✓ Exception tracking and reporting
✓ Performance metrics
✓ Health check endpoints
✓ Real-time job monitoring
✓ Test execution tracking
✓ Custom business metrics
```

---

## Design Patterns Used

### 1. **Clean Architecture**
   - Separation of concerns
   - Dependency inversion
   - Testability

### 2. **CQRS (Command Query Responsibility Segregation)**
   - Separate read and write models
   - Optimized for different concerns

### 3. **Repository Pattern**
   - Data access abstraction
   - Testability
   - Flexibility in data source

### 4. **Unit of Work**
   - Transaction management
   - Multiple repository coordination

### 5. **Dependency Injection**
   - Loose coupling
   - Testability
   - Configuration flexibility

### 6. **Factory Pattern**
   - Object creation abstraction
   - Used in test client creation

### 7. **Observer Pattern**
   - SignalR for real-time updates
   - Event-driven architecture

### 8. **Strategy Pattern**
   - Different test execution strategies
   - Report generation strategies

---

## Conclusion

The architecture demonstrates:
- **Modern best practices** in software design
- **Scalability** through proper layering and async processing
- **Maintainability** through clean architecture and CQRS
- **Real-time capabilities** via SignalR
- **Testability** through dependency injection and separation of concerns
- **Production-readiness** with proper error handling, logging, and monitoring

The system is designed to be **enterprise-grade**, **scalable**, and **maintainable** while providing comprehensive test automation capabilities specifically tailored for Forex trading applications.
