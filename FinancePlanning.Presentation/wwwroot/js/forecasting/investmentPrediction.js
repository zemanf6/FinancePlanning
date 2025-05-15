document.addEventListener("DOMContentLoaded", () => {
    initCurrencyDropdown();
    renderHistogram();
    renderTrajectories();
});

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
    if (!container || !window.predictionResult || !predictionResult.sampleTrajectories?.length) return;

    container.innerHTML = "";
    const canvas = document.createElement("canvas");
    container.appendChild(canvas);

    const datasets = predictionResult.sampleTrajectories.map((trajectory, index) => ({
        label: `Simulation ${index + 1}`,
        data: trajectory,
        fill: false,
        borderColor: getColor(index),
        tension: 0.2
    }));

    if (predictionResult.percentile10 && predictionResult.percentile50 && predictionResult.percentile90) {
        const years = predictionResult.sampleTrajectories[0]?.length ?? 0;
        const line = (value) => Array.from({ length: years }, () => value);

        datasets.push({
            label: "10th Percentile (Pessimistic)",
            data: line(predictionResult.percentile10),
            borderColor: "#dc3545",
            borderWidth: 2,
            borderDash: [5, 5],
            fill: false,
            tension: 0.1,
            pointRadius: 0
        });

        datasets.push({
            label: "50th Percentile (Median)",
            data: line(predictionResult.percentile50),
            borderColor: "#fd7e14",
            borderWidth: 2,
            borderDash: [5, 5],
            fill: false,
            tension: 0.1,
            pointRadius: 0
        });

        datasets.push({
            label: "90th Percentile (Optimistic)",
            data: line(predictionResult.percentile90),
            borderColor: "#198754",
            borderWidth: 2,
            borderDash: [5, 5],
            fill: false,
            tension: 0.1,
            pointRadius: 0
        });
    }

    const labels = predictionResult.sampleTrajectories[0].map((_, i) => `Year ${i + 1}`);

    new Chart(canvas.getContext("2d"), {
        type: "line",
        data: {
            labels: labels,
            datasets: datasets
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            interaction: {
                mode: 'nearest',
                axis: 'x',
                intersect: false
            },
            plugins: {
                legend: {
                    display: true,
                    position: 'bottom'
                }
            },
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });
}

function formatCurrency(value) {
    return value.toLocaleString(undefined, { minimumFractionDigits: 0, maximumFractionDigits: 0 });
}

function getColor(index) {
    const colors = [
        "#0d6efd", "#dc3545", "#198754", "#ffc107", "#6f42c1",
        "#fd7e14", "#20c997", "#6610f2", "#d63384", "#0dcaf0"
    ];
    return colors[index % colors.length];
}
