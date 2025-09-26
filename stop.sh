#!/bin/bash

# Stop all services for the Forex Test Automation Suite

set -e

echo "🛑 Stopping Forex Test Automation Suite..."

# Stop background processes
stop_processes() {
    echo "🔄 Stopping application processes..."
    
    # Stop API
    if [ -f backend/api.pid ]; then
        echo "   Stopping API server..."
        kill $(cat backend/api.pid) 2>/dev/null || true
        rm backend/api.pid
        echo "   ✅ API server stopped"
    fi
    
    # Stop Frontend
    if [ -f frontend.pid ]; then
        echo "   Stopping frontend server..."
        kill $(cat frontend.pid) 2>/dev/null || true
        rm frontend.pid
        echo "   ✅ Frontend server stopped"
    fi
}

# Stop Docker services
stop_infrastructure() {
    echo "🐳 Stopping infrastructure services..."
    
    docker-compose down
    
    echo "   ✅ Infrastructure services stopped"
}

# Clean up logs and temporary files
cleanup() {
    echo "🧹 Cleaning up temporary files..."
    
    # Remove log files
    rm -f backend/api.log
    rm -f frontend.log
    
    # Clean build artifacts (optional)
    if [ "$1" == "--clean-build" ]; then
        echo "   Cleaning build artifacts..."
        find . -name "bin" -type d -exec rm -rf {} + 2>/dev/null || true
        find . -name "obj" -type d -exec rm -rf {} + 2>/dev/null || true
        rm -rf frontend/build
        rm -rf frontend/node_modules/.cache
    fi
    
    echo "   ✅ Cleanup completed"
}

# Show final status
show_status() {
    echo ""
    echo "✅ Forex Test Automation Suite has been stopped"
    echo ""
    echo "📊 Final Status:"
    docker-compose ps
    echo ""
}

# Main execution
main() {
    stop_processes
    stop_infrastructure
    cleanup "$1"
    show_status
    
    echo "🎯 To start the suite again, run: ./start.sh"
}

# Run main function with all arguments
main "$@"