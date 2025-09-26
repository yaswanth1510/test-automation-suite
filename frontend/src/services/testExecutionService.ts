import { apiClient } from './apiClient';
import { TestExecutionRequest, TestSuiteExecutionRequest, TestExecutionResult, ApiResponse } from '../types';

export class TestExecutionService {
  async executeTest(request: TestExecutionRequest): Promise<ApiResponse<TestExecutionResult>> {
    const response = await apiClient.post<TestExecutionResult>('/testexecution/execute-test', request);
    return {
      data: response.data,
      success: response.status >= 200 && response.status < 300,
    };
  }

  async executeTestSuite(request: TestSuiteExecutionRequest): Promise<ApiResponse<TestExecutionResult[]>> {
    const response = await apiClient.post<TestExecutionResult[]>('/testexecution/execute-suite', request);
    return {
      data: response.data,
      success: response.status >= 200 && response.status < 300,
    };
  }

  async getExecutionStatus(executionId: string): Promise<ApiResponse<TestExecutionResult>> {
    const response = await apiClient.get<TestExecutionResult>(`/testexecution/status/${executionId}`);
    return {
      data: response.data,
      success: response.status >= 200 && response.status < 300,
    };
  }

  async cancelExecution(executionId: string): Promise<ApiResponse<boolean>> {
    const response = await apiClient.post<boolean>(`/testexecution/cancel/${executionId}`);
    return {
      data: response.data,
      success: response.status >= 200 && response.status < 300,
    };
  }

  async discoverTests(rootPath: string): Promise<ApiResponse<any[]>> {
    const response = await apiClient.post<any[]>('/testdiscovery/discover', { rootPath });
    return {
      data: response.data,
      success: response.status >= 200 && response.status < 300,
    };
  }

  async discoverTestsInAssembly(assemblyPath: string): Promise<ApiResponse<any[]>> {
    const response = await apiClient.post<any[]>('/testdiscovery/discover-assembly', { assemblyPath });
    return {
      data: response.data,
      success: response.status >= 200 && response.status < 300,
    };
  }
}

export const testExecutionService = new TestExecutionService();