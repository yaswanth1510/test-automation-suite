import { apiClient } from './apiClient';
import { TestSuite, CreateTestSuiteRequest, UpdateTestSuiteRequest, ApiResponse } from '../types';

export class TestSuiteService {
  async getTestSuites(activeOnly?: boolean, tags?: string[]): Promise<ApiResponse<TestSuite[]>> {
    const params: any = {};
    if (activeOnly !== undefined) params.activeOnly = activeOnly;
    if (tags && tags.length > 0) params.tags = tags;

    const response = await apiClient.get<TestSuite[]>('/testsuites', params);
    return {
      data: response.data,
      success: response.status >= 200 && response.status < 300,
    };
  }

  async getTestSuiteById(id: string, includeTestCases?: boolean): Promise<ApiResponse<TestSuite>> {
    const params: any = {};
    if (includeTestCases !== undefined) params.includeTestCases = includeTestCases;

    const response = await apiClient.get<TestSuite>(`/testsuites/${id}`, params);
    return {
      data: response.data,
      success: response.status >= 200 && response.status < 300,
    };
  }

  async createTestSuite(request: CreateTestSuiteRequest): Promise<ApiResponse<string>> {
    const response = await apiClient.post<string>('/testsuites', request);
    return {
      data: response.data,
      success: response.status >= 200 && response.status < 300,
    };
  }

  async updateTestSuite(id: string, request: UpdateTestSuiteRequest): Promise<ApiResponse<void>> {
    const response = await apiClient.put<void>(`/testsuites/${id}`, request);
    return {
      data: response.data,
      success: response.status >= 200 && response.status < 300,
    };
  }

  async deleteTestSuite(id: string): Promise<ApiResponse<void>> {
    const response = await apiClient.delete<void>(`/testsuites/${id}`);
    return {
      data: response.data,
      success: response.status >= 200 && response.status < 300,
    };
  }
}

export const testSuiteService = new TestSuiteService();