import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { 
  TestStartedEvent, 
  TestCompletedEvent, 
  TestProgressEvent, 
  TestErrorEvent, 
  SessionStartedEvent, 
  SessionCompletedEvent, 
  SystemNotificationEvent 
} from '../../types';

interface Notification {
  id: string;
  type: 'test-started' | 'test-completed' | 'test-progress' | 'test-error' | 'session-started' | 'session-completed' | 'system';
  title: string;
  message: string;
  severity: 'info' | 'warning' | 'error' | 'success';
  timestamp: string;
  read: boolean;
  data?: any;
}

interface NotificationsState {
  notifications: Notification[];
  unreadCount: number;
  showNotifications: boolean;
}

const initialState: NotificationsState = {
  notifications: [],
  unreadCount: 0,
  showNotifications: false,
};

function generateId(): string {
  return `${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
}

const notificationsSlice = createSlice({
  name: 'notifications',
  initialState,
  reducers: {
    addTestStartedNotification: (state, action: PayloadAction<TestStartedEvent>) => {
      const notification: Notification = {
        id: generateId(),
        type: 'test-started',
        title: 'Test Started',
        message: `Test "${action.payload.testName}" has started`,
        severity: 'info',
        timestamp: action.payload.timestamp,
        read: false,
        data: action.payload,
      };
      state.notifications.unshift(notification);
      state.unreadCount += 1;
    },
    addTestCompletedNotification: (state, action: PayloadAction<TestCompletedEvent>) => {
      const severity = action.payload.status === 'Passed' ? 'success' : 
                      action.payload.status === 'Failed' ? 'error' : 'info';
      const notification: Notification = {
        id: generateId(),
        type: 'test-completed',
        title: 'Test Completed',
        message: `Test "${action.payload.testName}" completed with status: ${action.payload.status}`,
        severity,
        timestamp: action.payload.timestamp,
        read: false,
        data: action.payload,
      };
      state.notifications.unshift(notification);
      state.unreadCount += 1;
    },
    addTestProgressNotification: (state, action: PayloadAction<TestProgressEvent>) => {
      const notification: Notification = {
        id: generateId(),
        type: 'test-progress',
        title: 'Test Progress',
        message: `Step "${action.payload.stepName}" status: ${action.payload.status}`,
        severity: 'info',
        timestamp: action.payload.timestamp,
        read: false,
        data: action.payload,
      };
      state.notifications.unshift(notification);
      state.unreadCount += 1;
    },
    addTestErrorNotification: (state, action: PayloadAction<TestErrorEvent>) => {
      const notification: Notification = {
        id: generateId(),
        type: 'test-error',
        title: 'Test Error',
        message: action.payload.errorMessage,
        severity: 'error',
        timestamp: action.payload.timestamp,
        read: false,
        data: action.payload,
      };
      state.notifications.unshift(notification);
      state.unreadCount += 1;
    },
    addSessionStartedNotification: (state, action: PayloadAction<SessionStartedEvent>) => {
      const notification: Notification = {
        id: generateId(),
        type: 'session-started',
        title: 'Session Started',
        message: `Session "${action.payload.sessionName}" started with ${action.payload.totalTests} tests`,
        severity: 'info',
        timestamp: action.payload.timestamp,
        read: false,
        data: action.payload,
      };
      state.notifications.unshift(notification);
      state.unreadCount += 1;
    },
    addSessionCompletedNotification: (state, action: PayloadAction<SessionCompletedEvent>) => {
      const notification: Notification = {
        id: generateId(),
        type: 'session-completed',
        title: 'Session Completed',
        message: `Session "${action.payload.sessionName}" completed: ${action.payload.passed} passed, ${action.payload.failed} failed, ${action.payload.skipped} skipped`,
        severity: action.payload.failed > 0 ? 'warning' : 'success',
        timestamp: action.payload.timestamp,
        read: false,
        data: action.payload,
      };
      state.notifications.unshift(notification);
      state.unreadCount += 1;
    },
    addSystemNotification: (state, action: PayloadAction<SystemNotificationEvent>) => {
      const notification: Notification = {
        id: generateId(),
        type: 'system',
        title: 'System Notification',
        message: action.payload.message,
        severity: action.payload.type,
        timestamp: action.payload.timestamp,
        read: false,
        data: action.payload,
      };
      state.notifications.unshift(notification);
      state.unreadCount += 1;
    },
    markNotificationAsRead: (state, action: PayloadAction<string>) => {
      const notification = state.notifications.find(n => n.id === action.payload);
      if (notification && !notification.read) {
        notification.read = true;
        state.unreadCount = Math.max(0, state.unreadCount - 1);
      }
    },
    markAllNotificationsAsRead: (state) => {
      state.notifications.forEach(notification => {
        notification.read = true;
      });
      state.unreadCount = 0;
    },
    removeNotification: (state, action: PayloadAction<string>) => {
      const index = state.notifications.findIndex(n => n.id === action.payload);
      if (index !== -1) {
        const notification = state.notifications[index];
        if (!notification.read) {
          state.unreadCount = Math.max(0, state.unreadCount - 1);
        }
        state.notifications.splice(index, 1);
      }
    },
    clearAllNotifications: (state) => {
      state.notifications = [];
      state.unreadCount = 0;
    },
    toggleNotificationPanel: (state) => {
      state.showNotifications = !state.showNotifications;
    },
    setShowNotifications: (state, action: PayloadAction<boolean>) => {
      state.showNotifications = action.payload;
    },
  },
});

export const {
  addTestStartedNotification,
  addTestCompletedNotification,
  addTestProgressNotification,
  addTestErrorNotification,
  addSessionStartedNotification,
  addSessionCompletedNotification,
  addSystemNotification,
  markNotificationAsRead,
  markAllNotificationsAsRead,
  removeNotification,
  clearAllNotifications,
  toggleNotificationPanel,
  setShowNotifications,
} = notificationsSlice.actions;

export default notificationsSlice.reducer;