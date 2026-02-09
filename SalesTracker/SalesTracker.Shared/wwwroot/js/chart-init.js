// Chart.js initialization for Dashboard
window.renderSalesChart = function(canvasId, labels, salesData) {
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

    // Create gradient
    const gradient = ctx.getContext('2d').createLinearGradient(0, 0, 0, 400);
    gradient.addColorStop(0, 'rgba(102, 126, 234, 0.4)');
    gradient.addColorStop(1, 'rgba(102, 126, 234, 0.05)');

    // Create chart
    window.mysalesChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Daily Sales',
                data: salesData,
                borderColor: '#667eea',
                backgroundColor: gradient,
                borderWidth: 3,
                borderCapStyle: 'round',
                borderJoinStyle: 'round',
                fill: true,
                tension: 0.4,
                pointRadius: 4,
                pointBackgroundColor: '#667eea',
                pointBorderColor: '#fff',
                pointBorderWidth: 2,
                pointHoverRadius: 6,
                pointHoverBackgroundColor: '#667eea'
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: true,
            plugins: {
                legend: {
                    display: true,
                    labels: {
                        font: {
                            size: 12,
                            weight: '500'
                        },
                        padding: 15,
                        usePointStyle: true,
                        color: '#212529'
                    }
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
