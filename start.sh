#!/bin/bash

# Build and run the complete test automation suite

set -e

echo "🚀 Starting Forex Test Automation Suite setup..."

# Check prerequisites
check_prerequisites() {
    echo "📋 Checking prerequisites..."
    
    if ! command -v docker &> /dev/null; then
        echo "❌ Docker is not installed"
        exit 1
    fi
    
    if ! command -v docker-compose &> /dev/null; then
        echo "❌ Docker Compose is not installed" 
        exit 1
    fi
    
    if ! command -v dotnet &> /dev/null; then
        echo "❌ .NET SDK is not installed"
        exit 1
    fi
    
    if ! command -v node &> /dev/null; then
        echo "❌ Node.js is not installed"
        exit 1
    fi
    
    echo "✅ All prerequisites are installed"
}

# Start infrastructure services
start_infrastructure() {
    echo "🔧 Starting infrastructure services..."
    docker-compose up -d sqlserver redis minio
    
    echo "⏳ Waiting for services to be ready..."
    sleep 30
    
    # Check if services are healthy
    if ! docker-compose ps | grep -q "Up"; then
        echo "❌ Some infrastructure services failed to start"
        docker-compose logs
        exit 1
    fi
    
    echo "✅ Infrastructure services are running"
}

# Build and start backend
start_backend() {
    echo "🏗️ Building and starting backend..."
    
    cd backend
    
    # Restore packages
    echo "📦 Restoring NuGet packages..."
    dotnet restore
    
    # Build solution
    echo "🔨 Building solution..."
    dotnet build --configuration Release
    
    # Run tests
    echo "🧪 Running backend tests..."
    dotnet test --configuration Release --logger "console;verbosity=detailed"
    
    # Start API in background
    echo "🚀 Starting API server..."
    cd src/ForexTestSuite.Api
    nohup dotnet run --configuration Release --urls http://0.0.0.0:5000 > ../../api.log 2>&1 &
    API_PID=$!
    echo $API_PID > ../../api.pid
    
    cd ../..
    
    echo "⏳ Waiting for API to start..."
    sleep 15
    
    # Health check
    if curl -f http://localhost:5000/health > /dev/null 2>&1; then
        echo "✅ Backend API is running (PID: $API_PID)"
    else
        echo "❌ Backend API failed to start"
        cat api.log
        exit 1
    fi
    
    cd ..
}

# Build and start frontend  
start_frontend() {
    echo "🎨 Building and starting frontend..."
    
    cd frontend
    
    # Install dependencies
    echo "📦 Installing npm dependencies..."
    npm ci
    
    # Run tests
    echo "🧪 Running frontend tests..."
    npm test -- --coverage --watchAll=false
    
    # Build for production
    echo "🔨 Building frontend..."
    npm run build
    
    # Start development server in background
    echo "🚀 Starting frontend server..."
    nohup npm start > ../frontend.log 2>&1 &
    FRONTEND_PID=$!
    echo $FRONTEND_PID > ../frontend.pid
    
    echo "⏳ Waiting for frontend to start..."
    sleep 20
    
    # Health check
    if curl -f http://localhost:3000 > /dev/null 2>&1; then
        echo "✅ Frontend is running (PID: $FRONTEND_PID)"
    else
        echo "❌ Frontend failed to start"
        cat ../frontend.log
        exit 1
    fi
    
    cd ..
}

# Build test framework
build_test_framework() {
    echo "🧪 Building test automation framework..."
    
    cd TestFramework
    
    # Build test framework
    dotnet build --configuration Release
    
    # Run sample tests
    echo "🎯 Running sample tests..."
    dotnet test --configuration Release --logger "console;verbosity=detailed"
    
    cd ..
    
    echo "✅ Test framework is ready"
}

# Show running services
show_status() {
    echo ""
    echo "🎉 Forex Test Automation Suite is running!"
    echo ""
    echo "📊 Services Status:"
    echo "   • Web UI:              http://localhost:3000"
    echo "   • API Documentation:   http://localhost:5000"
    echo "   • Hangfire Dashboard:  http://localhost:5000/hangfire"
    echo "   • MinIO Console:       http://localhost:9001 (admin/password123)"
    echo ""
    echo "🔍 Service Health:"
    docker-compose ps
    echo ""
    echo "📁 Log Files:"
    echo "   • API Logs:      backend/api.log"
    echo "   • Frontend Logs: frontend.log"
    echo ""
    echo "🛑 To stop all services, run: ./stop.sh"
}

# Cleanup on exit
cleanup() {
    echo ""
    echo "🧹 Cleaning up on exit..."
    
    # Kill background processes
    if [ -f backend/api.pid ]; then
        kill $(cat backend/api.pid) 2>/dev/null || true
        rm backend/api.pid
    fi
    
    if [ -f frontend.pid ]; then
        kill $(cat frontend.pid) 2>/dev/null || true
        rm frontend.pid
    fi
}

# Set trap for cleanup
trap cleanup EXIT INT TERM

# Main execution
main() {
    check_prerequisites
    start_infrastructure
    start_backend
    start_frontend
    build_test_framework
    show_status
    
    echo "Press Ctrl+C to stop all services"
    
    # Keep script running
    while true; do
        sleep 60
        
        # Health checks
        if ! curl -f http://localhost:5000/health > /dev/null 2>&1; then
            echo "⚠️  Backend API health check failed"
        fi
        
        if ! curl -f http://localhost:3000 > /dev/null 2>&1; then
            echo "⚠️  Frontend health check failed"  
        fi
    done
}

# Run main function
main "$@"