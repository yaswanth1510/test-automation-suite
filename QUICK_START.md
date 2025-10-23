# Quick Start Guide - Forex Test Automation Suite

This guide will help you quickly understand and get started with the Forex Test Automation Suite repository.

---

## 📚 What is This Repository?

The **Forex Test Automation Suite** is an enterprise-grade, full-stack test automation framework specifically designed for **Forex (Foreign Exchange) trading applications**. It provides comprehensive testing capabilities with real-time monitoring, advanced reporting, and domain-specific features for financial trading systems.

### Key Highlights

- **🏗️ Modern Architecture**: Clean Architecture with CQRS pattern
- **⚡ Real-time Updates**: SignalR WebSocket for live test monitoring
- **🎯 Forex-Specific**: Built specifically for trading platform testing
- **📊 Rich Reporting**: Multiple formats (HTML, JSON, XML, PDF, Excel)
- **🚀 Scalable**: Containerized with Docker, ready for production
- **💻 Full-Stack**: .NET 8 backend + React TypeScript frontend

---

## 🗂️ Repository Structure at a Glance

```
test-automation-suite/
│
├── 📂 backend/              # .NET 8 Web API (Clean Architecture)
│   └── src/
│       ├── ForexTestSuite.Api/          # REST API & SignalR Hubs
│       ├── ForexTestSuite.Application/  # Business Logic (CQRS)
│       ├── ForexTestSuite.Domain/       # Domain Entities
│       └── ForexTestSuite.Infrastructure/ # Data Access & Services
│
├── 📂 frontend/             # React + TypeScript Web Application
│   └── src/
│       ├── components/      # React Components (Dashboard, etc.)
│       ├── services/        # API & SignalR Services
│       ├── store/           # Redux State Management
│       └── types/           # TypeScript Type Definitions
│
├── 📂 TestFramework/        # Reusable Test Automation Framework
│   ├── Core/                # StepManager, ParameterGenerator, etc.
│   ├── Clients/             # HTTP, SQL, Web Test Clients
│   ├── Modules/             # Forex Trading Test Module
│   └── Tests/               # Sample Integration Tests
│
├── 🐳 docker-compose.yml    # Container Orchestration
├── 🚀 start.sh              # One-command Startup Script
└── 📖 README.md             # Comprehensive Documentation
```

---

## 🔧 Technology Stack

### Backend
- **.NET 8.0** - Modern C# framework
- **ASP.NET Core** - Web API
- **SignalR** - Real-time communication
- **Entity Framework Core** - ORM
- **MediatR** - CQRS implementation
- **NUnit** - Testing framework

### Frontend
- **React 18** - UI library
- **TypeScript** - Type-safe JavaScript
- **Material-UI** - Component library
- **Redux Toolkit** - State management
- **Chart.js** - Data visualization
- **Axios** - HTTP client

### Infrastructure
- **SQL Server 2022** - Database
- **Redis 7** - Caching
- **MinIO** - Object storage
- **Docker** - Containerization
- **Nginx** - Web server

---

## 🚀 Quick Start (3 Steps)

### Prerequisites

- ✅ .NET 8.0 SDK
- ✅ Node.js 18+
- ✅ Docker & Docker Compose
- ✅ Git

### Option 1: Automated Start (Recommended)

```bash
# Clone the repository
git clone <repository-url>
cd test-automation-suite

# Run the automated startup script
chmod +x start.sh
./start.sh
```

This script will:
1. Check prerequisites
2. Start infrastructure (SQL Server, Redis, MinIO)
3. Build and test backend
4. Build and test frontend
5. Build test framework
6. Launch all services

### Option 2: Manual Start

```bash
# Step 1: Start infrastructure services
docker-compose up -d sqlserver redis minio

# Step 2: Start backend API
cd backend/src/ForexTestSuite.Api
dotnet restore
dotnet run

# Step 3: Start frontend (in new terminal)
cd frontend
npm install
npm start
```

### 🌐 Access the Application

Once started, access:

- **Web UI**: http://localhost:3000
- **API Swagger**: http://localhost:5000/swagger
- **Hangfire Dashboard**: http://localhost:5000/hangfire
- **MinIO Console**: http://localhost:9001 (admin/password123)

---

## 📖 Documentation Overview

We've created comprehensive documentation to help you understand this repository:

### 1. **README.md** - Main Documentation
   - Project overview
   - Feature list
   - Installation guide
   - Usage examples
   - Configuration details

### 2. **REPOSITORY_ANALYSIS.md** - Deep Dive Analysis
   - Executive summary
   - Detailed architecture explanation
   - Component breakdown
   - Technology stack details
   - Use cases and capabilities
   - Performance characteristics
   - **👉 Read this for complete understanding**

### 3. **ARCHITECTURE.md** - System Design
   - Architecture diagrams (ASCII art)
   - System layers and dependencies
   - Data flow explanations
   - Design patterns used
   - Deployment architecture
   - **👉 Read this for architectural insights**

### 4. **CODE_STRUCTURE.md** - Developer Guide
   - Directory structure walkthrough
   - File organization
   - Code conventions
   - Common development tasks
   - Navigation tips
   - **👉 Read this before coding**

---

## 🎯 What Can You Do With This?

### 1. **Test Forex Trading Platforms**
- Validate order execution
- Verify market data accuracy
- Test P&L calculations
- Ensure risk management rules

### 2. **Automate Regression Testing**
- Create test suites
- Schedule automated runs
- Monitor test execution in real-time
- Generate comprehensive reports

### 3. **Performance Testing**
- Load testing for high-frequency trading
- Latency measurements
- Stress testing under market volatility
- Concurrent user simulation

### 4. **Compliance Testing**
- KYC/AML workflow validation
- Transaction reporting accuracy
- Regulatory limit enforcement
- Audit trail verification

---

## 🏗️ Architecture at a Glance

```
┌─────────────────────────────────────────────┐
│          React Frontend (Port 3000)         │
│     Dashboard | Test Suites | Reports       │
└──────────────────┬──────────────────────────┘
                   │ HTTP REST + WebSocket
┌──────────────────┴──────────────────────────┐
│        .NET API (Port 5000/5001)            │
│    Controllers | SignalR Hubs | Services    │
└──────────────────┬──────────────────────────┘
                   │
     ┌─────────────┼─────────────┐
     │             │             │
┌────┴────┐  ┌────┴────┐  ┌─────┴─────┐
│   SQL   │  │  Redis  │  │   MinIO   │
│ Server  │  │  Cache  │  │  Storage  │
└─────────┘  └─────────┘  └───────────┘
```

**Clean Architecture Layers**:
1. **Domain** - Business entities and rules
2. **Application** - Use cases (CQRS with MediatR)
3. **Infrastructure** - Data access and external services
4. **API** - REST endpoints and SignalR hubs

---

## 💡 Key Features Explained

### Real-time Test Monitoring
- Watch tests execute live via WebSocket
- See step-by-step progress
- Instant failure notifications
- No page refresh needed

### CQRS Pattern
- Separate read and write operations
- Optimized for different concerns
- Scalable architecture
- Clear separation of responsibilities

### Test Framework Components

**StepManager**
- Orchestrates test execution
- Manages test steps and flow
- Tracks execution history

**ParameterGenerator**
- Creates dynamic test data
- Forex-specific parameters (currency pairs, prices, amounts)
- Configurable constraints

**ComparisonTool**
- Validates test results
- JSON/XML comparison
- Numeric tolerance checking

**Test Clients**
- HTTP Client for API testing
- SQL Client for database validation
- Web Client for browser automation

---

## 📊 Sample Test Flow

```
1. User creates test suite in Web UI
2. Frontend sends API request
3. Backend creates test suite in database
4. User triggers test execution
5. Background worker picks up job
6. Test Framework executes tests
7. SignalR broadcasts real-time updates
8. Results stored in database
9. Artifacts uploaded to MinIO
10. Report generated and displayed
```

---

## 🛠️ Common Development Tasks

### Adding a New API Endpoint

1. Create command/query in `Application/Features/`
2. Create handler implementing `IRequestHandler`
3. Add controller action in `Api/Controllers/`
4. Test with Swagger UI

### Adding a New React Component

1. Create component in `frontend/src/components/`
2. Define TypeScript props interface
3. Implement with React hooks
4. Connect to Redux if needed
5. Add to parent component

### Creating a New Test Case

1. Create test class in `TestFramework/Tests/`
2. Use `[TestFixture]` and `[Test]` attributes
3. Utilize framework components (StepManager, etc.)
4. Run with `dotnet test`

---

## 📦 What's Included?

### Backend (C# .NET)
- ✅ REST API with Swagger documentation
- ✅ SignalR real-time hubs
- ✅ Entity Framework Core with SQL Server
- ✅ CQRS implementation with MediatR
- ✅ Background jobs with Hangfire
- ✅ Structured logging with Serilog
- ✅ Unit tests with NUnit

### Frontend (React TypeScript)
- ✅ Modern React with hooks
- ✅ Material-UI components
- ✅ Redux Toolkit state management
- ✅ SignalR client integration
- ✅ Chart.js visualizations
- ✅ Jest + React Testing Library tests

### Test Framework (C# NUnit)
- ✅ Step-based test execution
- ✅ Dynamic parameter generation
- ✅ HTTP/SQL/Web test clients
- ✅ Forex-specific test modules
- ✅ Sample integration tests

### Infrastructure
- ✅ Docker Compose setup
- ✅ SQL Server database
- ✅ Redis caching
- ✅ MinIO object storage
- ✅ Automated startup scripts

---

## 🎓 Learning Path

### For Beginners
1. Read **README.md** for overview
2. Run **start.sh** to see it in action
3. Explore Web UI at http://localhost:3000
4. Review sample tests in `TestFramework/Tests/`

### For Developers
1. Read **CODE_STRUCTURE.md** for codebase navigation
2. Read **ARCHITECTURE.md** for design patterns
3. Study backend controllers and frontend components
4. Try modifying a component or adding a new feature

### For Architects
1. Read **ARCHITECTURE.md** for system design
2. Review Clean Architecture implementation
3. Study CQRS pattern usage
4. Analyze scalability and deployment options

---

## 🔍 Exploring the Code

### Want to see API endpoints?
👉 `backend/src/ForexTestSuite.Api/Controllers/`

### Want to see business logic?
👉 `backend/src/ForexTestSuite.Application/Features/`

### Want to see React components?
👉 `frontend/src/components/`

### Want to see test examples?
👉 `TestFramework/Tests/SampleForexTradingTests.cs`

### Want to see database models?
👉 `backend/src/ForexTestSuite.Domain/Entities/`

---

## ❓ Frequently Asked Questions

**Q: What is this repository for?**
A: It's a complete test automation framework for Forex trading applications, with real-time monitoring and comprehensive reporting.

**Q: Can I use this for non-Forex applications?**
A: Yes! The framework is extensible. You can create custom test modules for any domain.

**Q: Is this production-ready?**
A: Yes, it follows enterprise-grade patterns and is containerized for deployment.

**Q: How do I stop all services?**
A: Run `./stop.sh` or `docker-compose down`

**Q: Where are the tests?**
A: Sample tests are in `TestFramework/Tests/`. You can add more tests there.

**Q: Can I run tests without Docker?**
A: Yes, but you'll need SQL Server and Redis installed locally.

---

## 🎯 Next Steps

1. **Explore the Documentation**
   - [ ] Read REPOSITORY_ANALYSIS.md for complete understanding
   - [ ] Review ARCHITECTURE.md for design insights
   - [ ] Check CODE_STRUCTURE.md for development guide

2. **Run the Application**
   - [ ] Execute `./start.sh` to start all services
   - [ ] Open http://localhost:3000 in your browser
   - [ ] Explore the dashboard and features

3. **Try Sample Tests**
   - [ ] Navigate to `TestFramework/Tests/`
   - [ ] Run `dotnet test` to execute sample tests
   - [ ] Review test results

4. **Make Changes**
   - [ ] Try modifying a React component
   - [ ] Add a new API endpoint
   - [ ] Create a custom test case

---

## 📞 Getting Help

- 📖 Read the documentation files in this repository
- 🐛 Check GitHub Issues for known problems
- 💬 Contact the development team
- 📚 Review the inline code comments

---

## 🏁 Summary

This repository is a **professional, production-ready test automation framework** for Forex trading applications, built with:

- **Modern technologies** (.NET 8, React 18, TypeScript)
- **Clean architecture** (maintainable and scalable)
- **Real-time capabilities** (SignalR WebSocket)
- **Comprehensive testing** (NUnit framework)
- **Full documentation** (you're reading it!)

Whether you're a QA engineer, developer, or architect, this repository demonstrates best practices in software engineering and provides a solid foundation for building test automation solutions.

**Ready to dive in?** Start with `./start.sh` and explore the Web UI at http://localhost:3000! 🚀
