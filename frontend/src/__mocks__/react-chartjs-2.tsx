import React from 'react';

// Mock the Line chart component
export const Line = React.forwardRef<HTMLDivElement, any>((props, ref) => (
  <div ref={ref} data-testid="mock-line-chart">
    Mock Line Chart
  </div>
));

Line.displayName = 'MockLineChart';

// Mock other chart types if needed
export const Bar = React.forwardRef<HTMLDivElement, any>((props, ref) => (
  <div ref={ref} data-testid="mock-bar-chart">
    Mock Bar Chart
  </div>
));

export const Pie = React.forwardRef<HTMLDivElement, any>((props, ref) => (
  <div ref={ref} data-testid="mock-pie-chart">
    Mock Pie Chart
  </div>
));

export const Doughnut = React.forwardRef<HTMLDivElement, any>((props, ref) => (
  <div ref={ref} data-testid="mock-doughnut-chart">
    Mock Doughnut Chart
  </div>
));