import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit';
import { TestSuite, CreateTestSuiteRequest, UpdateTestSuiteRequest, LoadingState } from '../../types';
import { testSuiteService } from '../../services/testSuiteService';

interface TestSuitesState extends LoadingState {
  testSuites: TestSuite[];
  selectedTestSuite?: TestSuite;
  filters: {
    activeOnly: boolean;
    tags: string[];
    searchQuery: string;
  };
}

const initialState: TestSuitesState = {
  testSuites: [],
  selectedTestSuite: undefined,
  isLoading: false,
  error: undefined,
  filters: {
    activeOnly: true,
    tags: [],
    searchQuery: '',
  },
};

// Async Thunks
export const fetchTestSuites = createAsyncThunk(
  'testSuites/fetchTestSuites',
  async (params: { activeOnly?: boolean; tags?: string[] } = {}) => {
    const response = await testSuiteService.getTestSuites(params.activeOnly, params.tags);
    return response.data;
  }
);

export const fetchTestSuiteById = createAsyncThunk(
  'testSuites/fetchTestSuiteById',
  async (params: { id: string; includeTestCases?: boolean }) => {
    const response = await testSuiteService.getTestSuiteById(params.id, params.includeTestCases);
    return response.data;
  }
);

export const createTestSuite = createAsyncThunk(
  'testSuites/createTestSuite',
  async (request: CreateTestSuiteRequest) => {
    const response = await testSuiteService.createTestSuite(request);
    return response.data;
  }
);

export const updateTestSuite = createAsyncThunk(
  'testSuites/updateTestSuite',
  async (request: UpdateTestSuiteRequest) => {
    await testSuiteService.updateTestSuite(request.id, request);
    return request;
  }
);

export const deleteTestSuite = createAsyncThunk(
  'testSuites/deleteTestSuite',
  async (id: string) => {
    await testSuiteService.deleteTestSuite(id);
    return id;
  }
);

const testSuitesSlice = createSlice({
  name: 'testSuites',
  initialState,
  reducers: {
    setSelectedTestSuite: (state, action: PayloadAction<TestSuite | undefined>) => {
      state.selectedTestSuite = action.payload;
    },
    setFilters: (state, action: PayloadAction<Partial<TestSuitesState['filters']>>) => {
      state.filters = { ...state.filters, ...action.payload };
    },
    clearError: (state) => {
      state.error = undefined;
    },
  },
  extraReducers: (builder) => {
    builder
      // Fetch Test Suites
      .addCase(fetchTestSuites.pending, (state) => {
        state.isLoading = true;
        state.error = undefined;
      })
      .addCase(fetchTestSuites.fulfilled, (state, action) => {
        state.isLoading = false;
        state.testSuites = action.payload;
      })
      .addCase(fetchTestSuites.rejected, (state, action) => {
        state.isLoading = false;
        state.error = action.error.message || 'Failed to fetch test suites';
      })
      // Fetch Test Suite by ID
      .addCase(fetchTestSuiteById.pending, (state) => {
        state.isLoading = true;
        state.error = undefined;
      })
      .addCase(fetchTestSuiteById.fulfilled, (state, action) => {
        state.isLoading = false;
        state.selectedTestSuite = action.payload;
      })
      .addCase(fetchTestSuiteById.rejected, (state, action) => {
        state.isLoading = false;
        state.error = action.error.message || 'Failed to fetch test suite';
      })
      // Create Test Suite
      .addCase(createTestSuite.pending, (state) => {
        state.isLoading = true;
        state.error = undefined;
      })
      .addCase(createTestSuite.fulfilled, (state, action) => {
        state.isLoading = false;
        // Refresh the list after creation
      })
      .addCase(createTestSuite.rejected, (state, action) => {
        state.isLoading = false;
        state.error = action.error.message || 'Failed to create test suite';
      })
      // Update Test Suite
      .addCase(updateTestSuite.pending, (state) => {
        state.isLoading = true;
        state.error = undefined;
      })
      .addCase(updateTestSuite.fulfilled, (state, action) => {
        state.isLoading = false;
        const index = state.testSuites.findIndex(ts => ts.id === action.payload.id);
        if (index !== -1) {
          state.testSuites[index] = action.payload as TestSuite;
        }
      })
      .addCase(updateTestSuite.rejected, (state, action) => {
        state.isLoading = false;
        state.error = action.error.message || 'Failed to update test suite';
      })
      // Delete Test Suite
      .addCase(deleteTestSuite.pending, (state) => {
        state.isLoading = true;
        state.error = undefined;
      })
      .addCase(deleteTestSuite.fulfilled, (state, action) => {
        state.isLoading = false;
        state.testSuites = state.testSuites.filter(ts => ts.id !== action.payload);
        if (state.selectedTestSuite?.id === action.payload) {
          state.selectedTestSuite = undefined;
        }
      })
      .addCase(deleteTestSuite.rejected, (state, action) => {
        state.isLoading = false;
        state.error = action.error.message || 'Failed to delete test suite';
      });
  },
});

export const { setSelectedTestSuite, setFilters, clearError } = testSuitesSlice.actions;
export default testSuitesSlice.reducer;