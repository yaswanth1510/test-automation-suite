// API Base Types
export interface ApiResponse<T> {
  data: T;
  message?: string;
  success: boolean;
}

// Test Management Types
export interface TestSuite {
  id: string;
  name: string;
  description: string;
  version: string;
  isActive: boolean;
  tags: string[];
  configuration: Record<string, any>;
  createdAt: string;
  updatedAt?: string;
  testCases?: TestCase[];
}

export interface TestCase {
  id: string;
  name: string;
  description: string;
  testType: TestType;
  filePath: string;
  className: string;
  methodName: string;
  priority: number;
  timeoutSeconds: number;
  isParallelizable: boolean;
  tags: string[];
  parameters: Record<string, any>;
  environment: Record<string, string>;
  createdAt: string;
  testSuiteId: string;
}

export interface TestExecution {
  id: string;
  status: TestStatus;
  startTime?: string;
  endTime?: string;
  duration?: number;
  errorMessage?: string;
  stackTrace?: string;
  results: Record<string, any>;
  screenshots: string[];
  artifacts: string[];
  executionMode: ExecutionMode;
  testCaseId: string;
  testSuiteId?: string;
  testSessionId?: string;
}

export interface TestSession {
  id: string;
  name: string;
  description: string;
  status: TestStatus;
  startTime?: string;
  endTime?: string;
  duration?: number;
  executionMode: ExecutionMode;
  maxParallelTests: number;
  environment: Record<string, string>;
  configuration: Record<string, any>;
  testExecutions: TestExecution[];
}

export interface TestReport {
  id: string;
  name: string;
  description: string;
  format: ReportFormat;
  filePath: string;
  content: string;
  metadata: Record<string, any>;
  statistics: Record<string, any>;
  createdAt: string;
  testSessionId?: string;
}

// Enums
export enum TestStatus {
  Pending = 'Pending',
  Running = 'Running',
  Passed = 'Passed',
  Failed = 'Failed',
  Skipped = 'Skipped',
  Cancelled = 'Cancelled'
}

export enum TestType {
  Unit = 'Unit',
  Integration = 'Integration',
  EndToEnd = 'EndToEnd',
  Api = 'Api',
  UI = 'UI',
  Performance = 'Performance',
  Load = 'Load',
  Security = 'Security',
  ForexTrading = 'ForexTrading',
  MarketDataValidation = 'MarketDataValidation',
  RiskManagement = 'RiskManagement',
  ComplianceValidation = 'ComplianceValidation'
}

export enum ExecutionMode {
  Sequential = 'Sequential',
  Parallel = 'Parallel',
  Distributed = 'Distributed'
}

export enum ReportFormat {
  Html = 'Html',
  Json = 'Json',
  Xml = 'Xml',
  Pdf = 'Pdf',
  Excel = 'Excel'
}

// Request/Response Types for API
export interface CreateTestSuiteRequest {
  name: string;
  description: string;
  version?: string;
  isActive?: boolean;
  tags?: string[];
  configuration?: Record<string, any>;
}

export interface UpdateTestSuiteRequest extends CreateTestSuiteRequest {
  id: string;
}

export interface TestExecutionRequest {
  testCaseId: string;
  parameters?: Record<string, any>;
  environment?: Record<string, string>;
  timeoutSeconds?: number;
}

export interface TestSuiteExecutionRequest {
  testSuiteId: string;
  testCaseIds?: string[];
  parameters?: Record<string, any>;
  environment?: Record<string, string>;
  runInParallel?: boolean;
  maxParallelTests?: number;
}

export interface TestExecutionResult {
  executionId: string;
  status: string;
  startTime?: string;
  endTime?: string;
  duration?: number;
  errorMessage?: string;
  results: Record<string, any>;
  screenshots: string[];
  artifacts: string[];
}

// SignalR Event Types
export interface TestStartedEvent {
  executionId: string;
  testName: string;
  timestamp: string;
}

export interface TestCompletedEvent {
  executionId: string;
  testName: string;
  status: string;
  duration: number;
  timestamp: string;
}

export interface TestProgressEvent {
  executionId: string;
  stepName: string;
  status: string;
  timestamp: string;
}

export interface TestErrorEvent {
  executionId: string;
  errorMessage: string;
  timestamp: string;
}

export interface SessionStartedEvent {
  sessionId: string;
  sessionName: string;
  totalTests: number;
  timestamp: string;
}

export interface SessionCompletedEvent {
  sessionId: string;
  sessionName: string;
  passed: number;
  failed: number;
  skipped: number;
  timestamp: string;
}

export interface SystemNotificationEvent {
  message: string;
  type: 'info' | 'warning' | 'error' | 'success';
  timestamp: string;
}

// UI State Types
export interface LoadingState {
  isLoading: boolean;
  error?: string;
}

export interface FilterState {
  testType?: TestType;
  status?: TestStatus;
  tags?: string[];
  dateRange?: {
    start: string;
    end: string;
  };
}