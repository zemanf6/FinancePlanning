function initCurrencyDropdown() {
    const currencySelect = document.getElementById("currencySelect");
    if (!currencySelect) return;

    axios.get('/data/currencies.json')
        .then(response => {
            populateCurrencyOptions(currencySelect, response.data);
        })
        .catch(error => {
            console.error("Failed to load currencies:", error);
        });
}

function populateCurrencyOptions(selectElement, currencies) {
    selectElement.innerHTML = '';

    Object.entries(currencies).forEach(([code, currency]) => {
        const option = document.createElement("option");
        option.value = code;
        option.textContent = `${code} - ${currency.name}`;
        selectElement.appendChild(option);
    });

    const selectedValue = selectElement.getAttribute("data-selected-value");
    selectElement.value = selectedValue && currencies[selectedValue] ? selectedValue : "USD";
}

function renderHistogram() {
    const container = document.getElementById("histogramContainer");
    if (!container || !window.predictionResult || !predictionResult.finalValues?.length) return;

    container.innerHTML = "";
    const canvas = document.createElement("canvas");
    container.appendChild(canvas);

    const data = predictionResult.finalValues;
    const binCount = 30;
    const min = Math.min(...data);
    const max = Math.max(...data);
    const step = (max - min) / binCount;

    const bins = Array(binCount).fill(0);
    data.forEach(v => {
        const index = Math.min(Math.floor((v - min) / step), binCount - 1);
        bins[index]++;
    });

    const labels = bins.map((_, i) => {
        const start = min + i * step;
        const end = start + step;
        return `${formatCurrency(start)} - ${formatCurrency(end)}`;
    });

    new Chart(canvas.getContext("2d"), {
        type: "bar",
        data: {
            labels: labels,
            datasets: [{
                label: "Final Value Distribution",
                data: bins,
                backgroundColor: "#0d6efd"
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                x: {
                    ticks: {
                        autoSkip: true,
                        maxRotation: 45,
                        minRotation: 45
                    }
                },
                y: {
                    beginAtZero: true
                }
            }
        }
    });
}

function renderTrajectories() {
    const container = document.getElementById("trajectoryContainer");
    if (!container || !window.predictionResult || !predictionResult.percentileTrajectories) return;

    const trajectories = predictionResult.percentileTrajectories;
    const labels = trajectories.percentile50.map((_, i) => `Year ${i + 1}`);

    const datasets = [
        {
            label: "10th Percentile (Pessimistic)",
            data: trajectories.percentile10,
            borderColor: "#dc3545",
            borderWidth: 2,
            tension: 0.2,
            fill: false,
            pointRadius: 3,
            pointHoverRadius: 6,
            pointBackgroundColor: "white",
            pointBorderWidth: 2
        },
        {
            label: "50th Percentile (Median)",
            data: trajectories.percentile50,
            borderColor: "#fd7e14",
            borderWidth: 2,
            tension: 0.2,
            fill: false,
            pointRadius: 3,
            pointHoverRadius: 6,
            pointBackgroundColor: "white",
            pointBorderWidth: 2
        },
        {
            label: "90th Percentile (Optimistic)",
            data: trajectories.percentile90,
            borderColor: "#198754",
            borderWidth: 2,
            tension: 0.2,
            fill: false,
            pointRadius: 3,
            pointHoverRadius: 6,
            pointBackgroundColor: "white",
            pointBorderWidth: 2
        }
    ];

    container.innerHTML = "";
    const canvas = document.createElement("canvas");
    container.appendChild(canvas);

    new Chart(canvas.getContext("2d"), {
        type: "line",
        data: { labels, datasets },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: true,
                    position: 'bottom'
                },
                tooltip: {
                    enabled: true,
                    mode: 'nearest',
                    intersect: false,
                    callbacks: {
                        label: function (context) {
                            const value = context.parsed.y;
                            return `${context.dataset.label}: ${value.toLocaleString(undefined, {
                                minimumFractionDigits: 0,
                                maximumFractionDigits: 0
                            })}`;
                        }
                    }
                },
                annotation: {
                    annotations: {
                        medianLine: {
                            type: 'line',
                            yMin: trajectories.percentile50[trajectories.percentile50.length - 1],
                            yMax: trajectories.percentile50[trajectories.percentile50.length - 1],
                            borderColor: 'rgba(0, 0, 0, 0.5)',
                            borderWidth: 1,
                            borderDash: [4, 4],
                            label: {
                                display: true,
                                content: 'Final Median',
                                position: 'start',
                                backgroundColor: 'rgba(255,255,255,0.7)',
                                color: 'black',
                                font: { size: 10 }
                            }
                        }
                    }
                },
                zoom: {
                    pan: {
                        enabled: true,
                        mode: 'x'
                    },
                    zoom: {
                        wheel: {
                            enabled: true
                        },
                        pinch: {
                            enabled: true
                        },
                        mode: 'x'
                    }
                }
            },
            interaction: {
                mode: 'nearest',
                axis: 'x',
                intersect: false
            },
            scales: {
                y: {
                    beginAtZero: true
                }
            },
            hover: {
                mode: 'nearest',
                intersect: false
            }
        }
    });
}

function formatCurrency(value) {
    return value.toLocaleString(undefined, { minimumFractionDigits: 0, maximumFractionDigits: 0 });
}

function updateHiddenAsset(select) {
    const row = select.closest('tr');
    const hiddenInput = row.querySelector('.hidden-asset-name');
    if (hiddenInput) {
        hiddenInput.value = select.value;
    }
}

function getColor(index) {
    const colors = [
        "#0d6efd", "#dc3545", "#198754", "#ffc107", "#6f42c1",
        "#fd7e14", "#20c997", "#6610f2", "#d63384", "#0dcaf0"
    ];
    return colors[index % colors.length];
}
