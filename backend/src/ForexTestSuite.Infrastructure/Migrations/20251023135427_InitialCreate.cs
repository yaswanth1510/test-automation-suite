using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForexTestSuite.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComplianceTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegulationType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TestData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValidationCriteria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpectedResults = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplianceTests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FinancialCalculationTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CalculationType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrencyPair = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OpenPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ClosePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CalculationParameters = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpectedResult = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tolerance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialCalculationTests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ForexTradingTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrencyPair = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TestAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Leverage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TradingParameters = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RiskParameters = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForexTradingTests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RiskManagementTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxRiskPerTrade = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxDrawdown = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Leverage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RiskRules = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpectedOutcomes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiskManagementTests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TestConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Settings = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConnectionStrings = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrowserSettings = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApiSettings = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatabaseSettings = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TestEnvironments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    BaseUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Variables = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Configuration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestEnvironments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TestSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: true),
                    ExecutionMode = table.Column<int>(type: "int", nullable: false),
                    MaxParallelTests = table.Column<int>(type: "int", nullable: false),
                    Environment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Configuration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TestSuites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Version = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Configuration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestSuites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarketDataSimulations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrencyPair = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EndPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Volatility = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SimulationStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SimulationEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PriceMovements = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewsEvents = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ForexTradingTestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketDataSimulations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketDataSimulations_ForexTradingTests_ForexTradingTestId",
                        column: x => x.ForexTradingTestId,
                        principalTable: "ForexTradingTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TradingOrderTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrencyPair = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    StopLoss = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TakeProfit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OrderTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpectedResult = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValidationRules = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ForexTradingTestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradingOrderTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TradingOrderTests_ForexTradingTests_ForexTradingTestId",
                        column: x => x.ForexTradingTestId,
                        principalTable: "ForexTradingTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Format = table.Column<int>(type: "int", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Statistics = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TestSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestReports_TestSessions_TestSessionId",
                        column: x => x.TestSessionId,
                        principalTable: "TestSessions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TestCases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TestType = table.Column<int>(type: "int", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClassName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MethodName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    TimeoutSeconds = table.Column<int>(type: "int", nullable: false),
                    IsParallelizable = table.Column<bool>(type: "bit", nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Parameters = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Environment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TestSuiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestCases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestCases_TestSuites_TestSuiteId",
                        column: x => x.TestSuiteId,
                        principalTable: "TestSuites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestExecutions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StackTrace = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Results = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Screenshots = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Artifacts = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExecutionMode = table.Column<int>(type: "int", nullable: false),
                    TestCaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TestSuiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TestSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestExecutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestExecutions_TestCases_TestCaseId",
                        column: x => x.TestCaseId,
                        principalTable: "TestCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestExecutions_TestSessions_TestSessionId",
                        column: x => x.TestSessionId,
                        principalTable: "TestSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TestExecutions_TestSuites_TestSuiteId",
                        column: x => x.TestSuiteId,
                        principalTable: "TestSuites",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TestSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Parameters = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpectedResults = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TestCaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestSteps_TestCases_TestCaseId",
                        column: x => x.TestCaseId,
                        principalTable: "TestCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestStepExecutions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: true),
                    ActualResults = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Screenshots = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Logs = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TestStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TestExecutionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestStepExecutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestStepExecutions_TestExecutions_TestExecutionId",
                        column: x => x.TestExecutionId,
                        principalTable: "TestExecutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestStepExecutions_TestSteps_TestStepId",
                        column: x => x.TestStepId,
                        principalTable: "TestSteps",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TestAssets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TestExecutionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TestStepExecutionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestAssets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestAssets_TestExecutions_TestExecutionId",
                        column: x => x.TestExecutionId,
                        principalTable: "TestExecutions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TestAssets_TestStepExecutions_TestStepExecutionId",
                        column: x => x.TestStepExecutionId,
                        principalTable: "TestStepExecutions",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "TestEnvironments",
                columns: new[] { "Id", "BaseUrl", "Configuration", "CreatedAt", "CreatedBy", "Description", "IsActive", "Name", "Type", "UpdatedAt", "UpdatedBy", "Variables" },
                values: new object[] { new Guid("87feac38-9973-4db8-bb54-137711c94c3f"), "http://localhost:3000", "{}", new DateTime(2025, 10, 23, 13, 54, 25, 704, DateTimeKind.Utc).AddTicks(3396), null, "Default development environment", true, "Development", 0, null, null, "{}" });

            migrationBuilder.CreateIndex(
                name: "IX_MarketDataSimulations_ForexTradingTestId",
                table: "MarketDataSimulations",
                column: "ForexTradingTestId");

            migrationBuilder.CreateIndex(
                name: "IX_TestAssets_TestExecutionId",
                table: "TestAssets",
                column: "TestExecutionId");

            migrationBuilder.CreateIndex(
                name: "IX_TestAssets_TestStepExecutionId",
                table: "TestAssets",
                column: "TestStepExecutionId");

            migrationBuilder.CreateIndex(
                name: "IX_TestCases_TestSuiteId",
                table: "TestCases",
                column: "TestSuiteId");

            migrationBuilder.CreateIndex(
                name: "IX_TestCases_TestType",
                table: "TestCases",
                column: "TestType");

            migrationBuilder.CreateIndex(
                name: "IX_TestExecutions_Status",
                table: "TestExecutions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TestExecutions_TestCaseId",
                table: "TestExecutions",
                column: "TestCaseId");

            migrationBuilder.CreateIndex(
                name: "IX_TestExecutions_TestSessionId",
                table: "TestExecutions",
                column: "TestSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_TestExecutions_TestSuiteId",
                table: "TestExecutions",
                column: "TestSuiteId");

            migrationBuilder.CreateIndex(
                name: "IX_TestReports_TestSessionId",
                table: "TestReports",
                column: "TestSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_TestStepExecutions_TestExecutionId",
                table: "TestStepExecutions",
                column: "TestExecutionId");

            migrationBuilder.CreateIndex(
                name: "IX_TestStepExecutions_TestStepId",
                table: "TestStepExecutions",
                column: "TestStepId");

            migrationBuilder.CreateIndex(
                name: "IX_TestSteps_TestCaseId",
                table: "TestSteps",
                column: "TestCaseId");

            migrationBuilder.CreateIndex(
                name: "IX_TestSuites_IsActive",
                table: "TestSuites",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_TestSuites_Name",
                table: "TestSuites",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_TradingOrderTests_ForexTradingTestId",
                table: "TradingOrderTests",
                column: "ForexTradingTestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComplianceTests");

            migrationBuilder.DropTable(
                name: "FinancialCalculationTests");

            migrationBuilder.DropTable(
                name: "MarketDataSimulations");

            migrationBuilder.DropTable(
                name: "RiskManagementTests");

            migrationBuilder.DropTable(
                name: "TestAssets");

            migrationBuilder.DropTable(
                name: "TestConfigurations");

            migrationBuilder.DropTable(
                name: "TestEnvironments");

            migrationBuilder.DropTable(
                name: "TestReports");

            migrationBuilder.DropTable(
                name: "TradingOrderTests");

            migrationBuilder.DropTable(
                name: "TestStepExecutions");

            migrationBuilder.DropTable(
                name: "ForexTradingTests");

            migrationBuilder.DropTable(
                name: "TestExecutions");

            migrationBuilder.DropTable(
                name: "TestSteps");

            migrationBuilder.DropTable(
                name: "TestSessions");

            migrationBuilder.DropTable(
                name: "TestCases");

            migrationBuilder.DropTable(
                name: "TestSuites");
        }
    }
}
