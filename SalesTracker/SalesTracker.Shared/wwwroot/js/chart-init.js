// Chart.js initialization for Dashboard

console.log('[Chart Init] chart-init.js script loaded');

window.renderSalesChart = function(canvasId, labels, salesData) {
    console.log('[Chart Init] Starting renderSalesChart with canvasId:', canvasId);
    console.log('[Chart Init] Labels count:', labels ? labels.length : 'null');
    console.log('[Chart Init] Sales data count:', salesData ? salesData.length : 'null');
    console.log('[Chart Init] Sales data sample:', salesData ? salesData.slice(0, 3) : 'null');
    
    window.chartCanvasId = canvasId;
    window.chartLabels = labels;
    window.chartSalesData = salesData;
    
    const ctx = document.getElementById(canvasId);
    if (!ctx) {
        console.error('[Chart Init] Canvas element not found:', canvasId);
        console.log('[Chart Init] Available canvas elements:', document.querySelectorAll('canvas').length);
        document.querySelectorAll('canvas').forEach((el, idx) => {
            console.log(`  Canvas ${idx}: id="${el.id}", class="${el.className}"`);
        });
        return;
    }
    
    console.log('[Chart Init] Canvas element found:', ctx);

    // Check if Chart.js is available
    if (typeof Chart === 'undefined') {
        console.error('[Chart Init] Chart.js library is not loaded. Please add Chart.js to your HTML.');
        console.log('[Chart Init] Window keys containing "chart":', Object.keys(window).filter(k => k.toLowerCase().includes('chart')));
        return;
    }

    console.log('[Chart Init] Chart.js is available. Version:', Chart.version || 'unknown');

    // Destroy existing chart if it exists
    if (window.mysalesChart) {
        console.log('[Chart Init] Destroying existing chart');
        window.mysalesChart.destroy();
    }
    
    
    // Set up resize listener (only once)
    if (!window.chartResizeListenerAttached) {
        console.log('[Chart Init] Setting up resize listener');
        window.chartResizeListenerAttached = true;
        window.addEventListener('resize', function() {
            clearTimeout(window.chartResizeTimeout);
            window.chartResizeTimeout = setTimeout(function() {
                if (window.mysalesChart && window.chartCanvasId && window.chartLabels && window.chartSalesData) {
                    console.log('[Chart Init] Redrawing chart on window resize');
                    window.renderSalesChart(chartCanvasId, chartLabels, window.chartSalesData);
                }
            }, 250);
        });
    }

    // Create gradient
    console.log('[Chart Init] Creating gradient');
    const gradient = ctx.getContext('2d').createLinearGradient(0, 0, 0, 400);
    gradient.addColorStop(0, 'rgba(78, 176, 54, 0.4)');
    gradient.addColorStop(1, 'rgba(78, 176, 54, 0.05)');

    // Create chart
    try {
        console.log('[Chart Init] Creating new Chart instance');
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
        console.log('[Chart Init] Chart created successfully:', window.mysalesChart);
    } catch (error) {
        console.error('[Chart Init] Error creating chart:', error);
        console.error('[Chart Init] Error details:', error.message, error.stack);
    }
};
