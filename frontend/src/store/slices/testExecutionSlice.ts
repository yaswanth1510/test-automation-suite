import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit';
import { TestExecution, TestExecutionRequest, TestSuiteExecutionRequest, TestExecutionResult, LoadingState } from '../../types';
import { testExecutionService } from '../../services/testExecutionService';

interface TestExecutionState extends LoadingState {
  executions: TestExecution[];
  currentExecution?: TestExecution;
  executionResults: Record<string, TestExecutionResult>;
  liveExecutions: Set<string>;
}

const initialState: TestExecutionState = {
  executions: [],
  currentExecution: undefined,
  executionResults: {},
  liveExecutions: new Set(),
  isLoading: false,
  error: undefined,
};

// Async Thunks
export const executeTest = createAsyncThunk(
  'testExecution/executeTest',
  async (request: TestExecutionRequest) => {
    const response = await testExecutionService.executeTest(request);
    return response.data;
  }
);

export const executeTestSuite = createAsyncThunk(
  'testExecution/executeTestSuite',
  async (request: TestSuiteExecutionRequest) => {
    const response = await testExecutionService.executeTestSuite(request);
    return response.data;
  }
);

export const getExecutionStatus = createAsyncThunk(
  'testExecution/getExecutionStatus',
  async (executionId: string) => {
    const response = await testExecutionService.getExecutionStatus(executionId);
    return response.data;
  }
);

export const cancelExecution = createAsyncThunk(
  'testExecution/cancelExecution',
  async (executionId: string) => {
    const response = await testExecutionService.cancelExecution(executionId);
    return { executionId, success: response.data };
  }
);

const testExecutionSlice = createSlice({
  name: 'testExecution',
  initialState,
  reducers: {
    setCurrentExecution: (state, action: PayloadAction<TestExecution | undefined>) => {
      state.currentExecution = action.payload;
    },
    updateExecutionFromSignalR: (state, action: PayloadAction<Partial<TestExecution> & { id: string }>) => {
      const index = state.executions.findIndex(e => e.id === action.payload.id);
      if (index !== -1) {
        state.executions[index] = { ...state.executions[index], ...action.payload };
      }
      if (state.currentExecution?.id === action.payload.id) {
        state.currentExecution = { ...state.currentExecution, ...action.payload };
      }
    },
    addLiveExecution: (state, action: PayloadAction<string>) => {
      state.liveExecutions = new Set([...state.liveExecutions, action.payload]);
    },
    removeLiveExecution: (state, action: PayloadAction<string>) => {
      const newSet = new Set(state.liveExecutions);
      newSet.delete(action.payload);
      state.liveExecutions = newSet;
    },
    clearError: (state) => {
      state.error = undefined;
    },
  },
  extraReducers: (builder) => {
    builder
      // Execute Test
      .addCase(executeTest.pending, (state) => {
        state.isLoading = true;
        state.error = undefined;
      })
      .addCase(executeTest.fulfilled, (state, action) => {
        state.isLoading = false;
        state.executionResults[action.payload.executionId] = action.payload;
        state.liveExecutions = new Set([...state.liveExecutions, action.payload.executionId]);
      })
      .addCase(executeTest.rejected, (state, action) => {
        state.isLoading = false;
        state.error = action.error.message || 'Failed to execute test';
      })
      // Execute Test Suite
      .addCase(executeTestSuite.pending, (state) => {
        state.isLoading = true;
        state.error = undefined;
      })
      .addCase(executeTestSuite.fulfilled, (state, action) => {
        state.isLoading = false;
        action.payload.forEach(result => {
          state.executionResults[result.executionId] = result;
          state.liveExecutions = new Set([...state.liveExecutions, result.executionId]);
        });
      })
      .addCase(executeTestSuite.rejected, (state, action) => {
        state.isLoading = false;
        state.error = action.error.message || 'Failed to execute test suite';
      })
      // Get Execution Status
      .addCase(getExecutionStatus.pending, (state) => {
        state.isLoading = true;
        state.error = undefined;
      })
      .addCase(getExecutionStatus.fulfilled, (state, action) => {
        state.isLoading = false;
        state.executionResults[action.payload.executionId] = action.payload;
      })
      .addCase(getExecutionStatus.rejected, (state, action) => {
        state.isLoading = false;
        state.error = action.error.message || 'Failed to get execution status';
      })
      // Cancel Execution
      .addCase(cancelExecution.pending, (state) => {
        state.isLoading = true;
        state.error = undefined;
      })
      .addCase(cancelExecution.fulfilled, (state, action) => {
        state.isLoading = false;
        if (action.payload.success) {
          const newSet = new Set(state.liveExecutions);
          newSet.delete(action.payload.executionId);
          state.liveExecutions = newSet;
        }
      })
      .addCase(cancelExecution.rejected, (state, action) => {
        state.isLoading = false;
        state.error = action.error.message || 'Failed to cancel execution';
      });
  },
});

export const {
  setCurrentExecution,
  updateExecutionFromSignalR,
  addLiveExecution,
  removeLiveExecution,
  clearError,
} = testExecutionSlice.actions;

export default testExecutionSlice.reducer;