import React, { createContext, useContext, useEffect, ReactNode } from 'react';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { useDispatch } from 'react-redux';
import { 
  addTestStartedNotification,
  addTestCompletedNotification,
  addTestProgressNotification,
  addTestErrorNotification,
  addSessionStartedNotification,
  addSessionCompletedNotification,
  addSystemNotification
} from '../store/slices/notificationsSlice';
import { updateExecutionFromSignalR, addLiveExecution, removeLiveExecution } from '../store/slices/testExecutionSlice';

interface SignalRContextType {
  connection: HubConnection | null;
  isConnected: boolean;
  joinExecution: (executionId: string) => Promise<void>;
  leaveExecution: (executionId: string) => Promise<void>;
  joinSession: (sessionId: string) => Promise<void>;
  leaveSession: (sessionId: string) => Promise<void>;
  joinGlobalNotifications: () => Promise<void>;
  leaveGlobalNotifications: () => Promise<void>;
}

const SignalRContext = createContext<SignalRContextType | null>(null);

interface SignalRProviderProps {
  children: ReactNode;
}

export const SignalRProvider: React.FC<SignalRProviderProps> = ({ children }) => {
  const [connection, setConnection] = React.useState<HubConnection | null>(null);
  const [isConnected, setIsConnected] = React.useState(false);
  const dispatch = useDispatch();

  useEffect(() => {
    const newConnection = new HubConnectionBuilder()
      .withUrl(process.env.REACT_APP_SIGNALR_URL || 'http://localhost:5000/testExecutionHub')
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Information)
      .build();

    // Set up event handlers
    newConnection.on('TestStarted', (data) => {
      console.log('TestStarted:', data);
      dispatch(addTestStartedNotification(data));
      dispatch(addLiveExecution(data.executionId));
      dispatch(updateExecutionFromSignalR({
        id: data.executionId,
        status: 'Running' as any,
        startTime: data.timestamp,
      }));
    });

    newConnection.on('TestCompleted', (data) => {
      console.log('TestCompleted:', data);
      dispatch(addTestCompletedNotification(data));
      dispatch(removeLiveExecution(data.executionId));
      dispatch(updateExecutionFromSignalR({
        id: data.executionId,
        status: data.status as any,
        endTime: data.timestamp,
        duration: data.duration,
      }));
    });

    newConnection.on('TestProgress', (data) => {
      console.log('TestProgress:', data);
      dispatch(addTestProgressNotification(data));
    });

    newConnection.on('TestError', (data) => {
      console.log('TestError:', data);
      dispatch(addTestErrorNotification(data));
      dispatch(updateExecutionFromSignalR({
        id: data.executionId,
        status: 'Failed' as any,
        errorMessage: data.errorMessage,
      }));
    });

    newConnection.on('SessionStarted', (data) => {
      console.log('SessionStarted:', data);
      dispatch(addSessionStartedNotification(data));
    });

    newConnection.on('SessionCompleted', (data) => {
      console.log('SessionCompleted:', data);
      dispatch(addSessionCompletedNotification(data));
    });

    newConnection.on('SystemNotification', (data) => {
      console.log('SystemNotification:', data);
      dispatch(addSystemNotification(data));
    });

    // Connection state handlers
    newConnection.onreconnecting((error) => {
      console.log('Connection lost due to error:', error);
      setIsConnected(false);
    });

    newConnection.onreconnected((connectionId) => {
      console.log('Connection reestablished. Connected with connectionId:', connectionId);
      setIsConnected(true);
    });

    newConnection.onclose((error) => {
      console.log('Connection closed due to error:', error);
      setIsConnected(false);
    });

    setConnection(newConnection);

    // Start the connection
    const startConnection = async () => {
      try {
        await newConnection.start();
        console.log('SignalR connection established');
        setIsConnected(true);
        // Auto-join global notifications
        await newConnection.invoke('JoinGlobalNotifications');
      } catch (error) {
        console.error('Failed to start SignalR connection:', error);
        setIsConnected(false);
      }
    };

    startConnection();

    // Cleanup on unmount
    return () => {
      if (newConnection) {
        newConnection.stop();
      }
    };
  }, [dispatch]);

  const joinExecution = async (executionId: string) => {
    if (connection && isConnected) {
      try {
        await connection.invoke('JoinExecution', executionId);
        console.log(`Joined execution group: ${executionId}`);
      } catch (error) {
        console.error('Failed to join execution group:', error);
      }
    }
  };

  const leaveExecution = async (executionId: string) => {
    if (connection && isConnected) {
      try {
        await connection.invoke('LeaveExecution', executionId);
        console.log(`Left execution group: ${executionId}`);
      } catch (error) {
        console.error('Failed to leave execution group:', error);
      }
    }
  };

  const joinSession = async (sessionId: string) => {
    if (connection && isConnected) {
      try {
        await connection.invoke('JoinSession', sessionId);
        console.log(`Joined session group: ${sessionId}`);
      } catch (error) {
        console.error('Failed to join session group:', error);
      }
    }
  };

  const leaveSession = async (sessionId: string) => {
    if (connection && isConnected) {
      try {
        await connection.invoke('LeaveSession', sessionId);
        console.log(`Left session group: ${sessionId}`);
      } catch (error) {
        console.error('Failed to leave session group:', error);
      }
    }
  };

  const joinGlobalNotifications = async () => {
    if (connection && isConnected) {
      try {
        await connection.invoke('JoinGlobalNotifications');
        console.log('Joined global notifications');
      } catch (error) {
        console.error('Failed to join global notifications:', error);
      }
    }
  };

  const leaveGlobalNotifications = async () => {
    if (connection && isConnected) {
      try {
        await connection.invoke('LeaveGlobalNotifications');
        console.log('Left global notifications');
      } catch (error) {
        console.error('Failed to leave global notifications:', error);
      }
    }
  };

  const value: SignalRContextType = {
    connection,
    isConnected,
    joinExecution,
    leaveExecution,
    joinSession,
    leaveSession,
    joinGlobalNotifications,
    leaveGlobalNotifications,
  };

  return (
    <SignalRContext.Provider value={value}>
      {children}
    </SignalRContext.Provider>
  );
};

export const useSignalR = (): SignalRContextType => {
  const context = useContext(SignalRContext);
  if (!context) {
    throw new Error('useSignalR must be used within a SignalRProvider');
  }
  return context;
};