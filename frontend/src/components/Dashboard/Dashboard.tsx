import React, { useEffect } from 'react';
import { 
  Grid, 
  Paper, 
  Typography, 
  Box, 
  Card, 
  CardContent, 
  CardActions,
  Button,
  Chip,
  LinearProgress,
  Alert,
  Fade
} from '@mui/material';
import {
  PlayArrow as PlayIcon,
  Stop as StopIcon,
  Assessment as ReportIcon,
  TrendingUp as TrendingUpIcon,
  Warning as WarningIcon,
  CheckCircle as SuccessIcon,
  Error as ErrorIcon,
  Schedule as ScheduleIcon
} from '@mui/icons-material';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from '../../store/store';
import { fetchTestSuites } from '../../store/slices/testSuitesSlice';
import { Line } from 'react-chartjs-2';
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
} from 'chart.js';

// Register Chart.js components
ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend
);

const Dashboard: React.FC = () => {
  const dispatch = useDispatch();
  const { testSuites, isLoading, error } = useSelector((state: RootState) => state.testSuites);
  const { executionResults, liveExecutions } = useSelector((state: RootState) => state.testExecution);
  const { notifications, unreadCount } = useSelector((state: RootState) => state.notifications);
  const { isConnected } = useSelector((state: RootState) => ({
    isConnected: true // This should come from SignalR context
  }));

  useEffect(() => {
    dispatch(fetchTestSuites({ activeOnly: true }) as any);
  }, [dispatch]);

  // Calculate statistics
  const totalTestSuites = testSuites.length;
  const activeTestSuites = testSuites.filter(ts => ts.isActive).length;
  const totalTestCases = testSuites.reduce((acc, ts) => acc + (ts.testCases?.length || 0), 0);
  const runningExecutions = liveExecutions.size;

  // Recent executions
  const recentExecutions = Object.values(executionResults).slice(0, 5);

  // Sample chart data (in a real app, this would come from your API)
  const chartData = {
    labels: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
    datasets: [
      {
        label: 'Passed Tests',
        data: [65, 59, 80, 81, 56, 55, 40],
        borderColor: 'rgb(75, 192, 192)',
        backgroundColor: 'rgba(75, 192, 192, 0.2)',
        tension: 0.1,
      },
      {
        label: 'Failed Tests',
        data: [28, 48, 40, 19, 86, 27, 90],
        borderColor: 'rgb(255, 99, 132)',
        backgroundColor: 'rgba(255, 99, 132, 0.2)',
        tension: 0.1,
      },
    ],
  };

  const chartOptions = {
    responsive: true,
    plugins: {
      legend: {
        position: 'top' as const,
      },
      title: {
        display: true,
        text: 'Test Execution Trends',
      },
    },
    scales: {
      y: {
        beginAtZero: true,
      },
    },
  };

  if (error) {
    return (
      <Fade in timeout={300}>
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      </Fade>
    );
  }

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Dashboard
      </Typography>

      {/* Connection Status */}
      <Box sx={{ mb: 3 }}>
        <Chip 
          icon={isConnected ? <CheckCircle /> : <ErrorIcon />}
          label={`SignalR: ${isConnected ? 'Connected' : 'Disconnected'}`}
          color={isConnected ? 'success' : 'error'}
          variant="outlined"
        />
      </Box>

      {/* Statistics Cards */}
      <Grid container spacing={3} sx={{ mb: 3 }}>
        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                <Box>
                  <Typography color="textSecondary" gutterBottom>
                    Test Suites
                  </Typography>
                  <Typography variant="h4">
                    {totalTestSuites}
                  </Typography>
                  <Typography color="textSecondary" variant="body2">
                    {activeTestSuites} active
                  </Typography>
                </Box>
                <ReportIcon color="primary" sx={{ fontSize: 40 }} />
              </Box>
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                <Box>
                  <Typography color="textSecondary" gutterBottom>
                    Test Cases
                  </Typography>
                  <Typography variant="h4">
                    {totalTestCases}
                  </Typography>
                  <Typography color="textSecondary" variant="body2">
                    Total available
                  </Typography>
                </Box>
                <TrendingUpIcon color="info" sx={{ fontSize: 40 }} />
              </Box>
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                <Box>
                  <Typography color="textSecondary" gutterBottom>
                    Running Tests
                  </Typography>
                  <Typography variant="h4">
                    {runningExecutions}
                  </Typography>
                  <Typography color="textSecondary" variant="body2">
                    Currently executing
                  </Typography>
                </Box>
                <ScheduleIcon color="warning" sx={{ fontSize: 40 }} />
              </Box>
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                <Box>
                  <Typography color="textSecondary" gutterBottom>
                    Notifications
                  </Typography>
                  <Typography variant="h4">
                    {unreadCount}
                  </Typography>
                  <Typography color="textSecondary" variant="body2">
                    Unread alerts
                  </Typography>
                </Box>
                <WarningIcon color="error" sx={{ fontSize: 40 }} />
              </Box>
            </CardContent>
          </Card>
        </Grid>
      </Grid>

      <Grid container spacing={3}>
        {/* Chart */}
        <Grid item xs={12} md={8}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Test Execution Trends
            </Typography>
            <Line data={chartData} options={chartOptions} />
          </Paper>
        </Grid>

        {/* Recent Activity */}
        <Grid item xs={12} md={4}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Recent Test Executions
            </Typography>
            {recentExecutions.length === 0 ? (
              <Typography color="textSecondary" variant="body2">
                No recent executions
              </Typography>
            ) : (
              recentExecutions.map((execution, index) => (
                <Card key={index} sx={{ mb: 2, bgcolor: 'action.hover' }}>
                  <CardContent sx={{ py: 1 }}>
                    <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                      <Box>
                        <Typography variant="body2" fontWeight="medium">
                          Execution {execution.executionId.substring(0, 8)}
                        </Typography>
                        <Typography variant="caption" color="textSecondary">
                          Status: {execution.status}
                        </Typography>
                      </Box>
                      <Chip 
                        size="small"
                        label={execution.status}
                        color={
                          execution.status === 'Passed' ? 'success' :
                          execution.status === 'Failed' ? 'error' :
                          execution.status === 'Running' ? 'warning' : 'default'
                        }
                      />
                    </Box>
                  </CardContent>
                </Card>
              ))
            )}
          </Paper>
        </Grid>

        {/* Quick Actions */}
        <Grid item xs={12}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Quick Actions
            </Typography>
            <Box sx={{ display: 'flex', gap: 2, flexWrap: 'wrap' }}>
              <Button
                variant="contained"
                startIcon={<PlayIcon />}
                color="primary"
                size="large"
              >
                Run Test Suite
              </Button>
              <Button
                variant="outlined"
                startIcon={<StopIcon />}
                color="error"
                size="large"
              >
                Stop All Tests
              </Button>
              <Button
                variant="outlined"
                startIcon={<ReportIcon />}
                color="info"
                size="large"
              >
                Generate Report
              </Button>
            </Box>
          </Paper>
        </Grid>
      </Grid>

      {isLoading && (
        <Box sx={{ mt: 3 }}>
          <LinearProgress />
        </Box>
      )}
    </Box>
  );
};

export default Dashboard;