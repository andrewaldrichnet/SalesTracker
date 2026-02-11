// Chart.js initialization for Dashboard

window.renderSalesChart = function(canvasId, labels, salesData) {
    window.chartCanvasId = canvasId;
    window.chartLabels = labels;
    window.chartSalesData = salesData;
    
    const ctx = document.getElementById(canvasId);
    if (!ctx) {
        console.error('Canvas element not found:', canvasId);
        return;
    }

    // Check if Chart.js is available
    if (typeof Chart === 'undefined') {
        console.warn('Chart.js library is not loaded. Please add Chart.js to your HTML.');
        return;
    }

    // Destroy existing chart if it exists
    if (window.mysalesChart) {
        window.mysalesChart.destroy();
    }
    
    // Set up resize listener (only once)
    if (!window.chartResizeListenerAttached) {
        window.chartResizeListenerAttached = true;
        window.addEventListener('resize', function() {
            clearTimeout(window.chartResizeTimeout);
            window.chartResizeTimeout = setTimeout(function() {
                if (window.mysalesChart && window.chartCanvasId && window.chartLabels && window.chartSalesData) {
                    window.renderSalesChart(chartCanvasId, chartLabels, window.chartSalesData);
                }
            }, 250);
        });
    }

    // Create gradient
    const gradient = ctx.getContext('2d').createLinearGradient(0, 0, 0, 400);
    gradient.addColorStop(0, 'rgba(78, 176, 54, 0.4)');
    gradient.addColorStop(1, 'rgba(78, 176, 54, 0.05)');

    // Create chart
    window.mysalesChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Sales',
                data: salesData,
                borderColor: '#4EB036',
                backgroundColor: gradient,
                borderWidth: 3,
                borderCapStyle: 'round',
                borderJoinStyle: 'round',
                fill: true,
                tension: 0.4,
                pointRadius: 4,
                pointBackgroundColor: '#4EB036',
                pointBorderColor: '#fff',
                pointBorderWidth: 2,
                pointHoverRadius: 6,
                pointHoverBackgroundColor: '#4EB036'
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: false
                },
                tooltip: {
                    backgroundColor: 'rgba(0, 0, 0, 0.8)',
                    titleFont: {
                        size: 14,
                        weight: 'bold'
                    },
                    bodyFont: {
                        size: 13
                    },
                    padding: 12,
                    displayColors: false,
                    callbacks: {
                        label: function(context) {
                            return 'Sales: $' + context.parsed.y.toFixed(2);
                        }
                    }
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    grid: {
                        color: 'rgba(0, 0, 0, 0.05)',
                        drawBorder: false
                    },
                    ticks: {
                        color: '#6c757d',
                        font: {
                            size: 12
                        },
                        callback: function(value) {
                            return '$' + value.toFixed(0);
                        }
                    }
                },
                x: {
                    grid: {
                        display: false,
                        drawBorder: false
                    },
                    ticks: {
                        color: '#6c757d',
                        font: {
                            size: 12
                        }
                    }
                }
            }
        }
    });
};
