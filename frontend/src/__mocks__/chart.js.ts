// Mock for Chart.js components
export const CategoryScale = {};
export const LinearScale = {};
export const PointElement = {};
export const LineElement = {};
export const Title = {};
export const Tooltip = {};
export const Legend = {};

// Mock Chart class with register method
export const Chart = {
  register: jest.fn(),
};

// Default export for Chart
export default Chart;