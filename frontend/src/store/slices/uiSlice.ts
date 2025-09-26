import { createSlice, PayloadAction } from '@reduxjs/toolkit';

interface UiState {
  sidebarOpen: boolean;
  activeTab: string;
  theme: 'light' | 'dark';
  loading: {
    [key: string]: boolean;
  };
  dialogs: {
    createTestSuite: boolean;
    editTestSuite: boolean;
    confirmDelete: boolean;
    testExecutionDetails: boolean;
  };
  selectedItems: {
    testSuiteId?: string;
    testCaseIds: string[];
    executionId?: string;
  };
}

const initialState: UiState = {
  sidebarOpen: true,
  activeTab: 'dashboard',
  theme: 'dark',
  loading: {},
  dialogs: {
    createTestSuite: false,
    editTestSuite: false,
    confirmDelete: false,
    testExecutionDetails: false,
  },
  selectedItems: {
    testCaseIds: [],
  },
};

const uiSlice = createSlice({
  name: 'ui',
  initialState,
  reducers: {
    toggleSidebar: (state) => {
      state.sidebarOpen = !state.sidebarOpen;
    },
    setSidebarOpen: (state, action: PayloadAction<boolean>) => {
      state.sidebarOpen = action.payload;
    },
    setActiveTab: (state, action: PayloadAction<string>) => {
      state.activeTab = action.payload;
    },
    setTheme: (state, action: PayloadAction<'light' | 'dark'>) => {
      state.theme = action.payload;
    },
    setLoading: (state, action: PayloadAction<{ key: string; isLoading: boolean }>) => {
      state.loading[action.payload.key] = action.payload.isLoading;
    },
    clearLoading: (state, action: PayloadAction<string>) => {
      delete state.loading[action.payload];
    },
    openDialog: (state, action: PayloadAction<keyof UiState['dialogs']>) => {
      state.dialogs[action.payload] = true;
    },
    closeDialog: (state, action: PayloadAction<keyof UiState['dialogs']>) => {
      state.dialogs[action.payload] = false;
    },
    setSelectedTestSuite: (state, action: PayloadAction<string | undefined>) => {
      state.selectedItems.testSuiteId = action.payload;
    },
    setSelectedTestCases: (state, action: PayloadAction<string[]>) => {
      state.selectedItems.testCaseIds = action.payload;
    },
    toggleTestCaseSelection: (state, action: PayloadAction<string>) => {
      const index = state.selectedItems.testCaseIds.indexOf(action.payload);
      if (index === -1) {
        state.selectedItems.testCaseIds.push(action.payload);
      } else {
        state.selectedItems.testCaseIds.splice(index, 1);
      }
    },
    setSelectedExecution: (state, action: PayloadAction<string | undefined>) => {
      state.selectedItems.executionId = action.payload;
    },
    clearSelections: (state) => {
      state.selectedItems = {
        testCaseIds: [],
      };
    },
  },
});

export const {
  toggleSidebar,
  setSidebarOpen,
  setActiveTab,
  setTheme,
  setLoading,
  clearLoading,
  openDialog,
  closeDialog,
  setSelectedTestSuite,
  setSelectedTestCases,
  toggleTestCaseSelection,
  setSelectedExecution,
  clearSelections,
} = uiSlice.actions;

export default uiSlice.reducer;