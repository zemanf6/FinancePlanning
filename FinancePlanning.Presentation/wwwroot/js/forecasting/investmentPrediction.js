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

    const principal = predictionResult.principal || 0;
    const monthlyContribution = predictionResult.monthlyContribution || 0;

    const ownContributions = [];
    for (let i = 0; i < labels.length; i++) {
        const year = i + 1;
        const value = principal + monthlyContribution * year * 12;
        ownContributions.push(value);
    }

    datasets.push({
        label: "Own Contributions",
        data: ownContributions,
        borderColor: "#0d6efd",
        borderWidth: 2,
        borderDash: [5, 5],
        tension: 0,
        fill: false,
        pointRadius: 0,
        pointHoverRadius: 0
    });

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
function onSimulateClick() {
    const form = document.getElementById('predictionForm');
    const btn = document.getElementById('simulateBtn');
    const spinner = document.getElementById('simulateSpinner');

    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    btn.disabled = true;
    spinner.classList.remove('d-none');
    setTimeout(() => form.submit(), 50);
}

function updateSimulationValue(value) {
    document.getElementById('simCountValue').innerText = value;
    document.getElementById('simulationWarning').style.display = value > 5000 ? 'block' : 'none';
}

function scrollToResultsIfAvailable() {
    const resultsSection = document.getElementById("results");
    if (resultsSection) {
        resultsSection.scrollIntoView({ behavior: "smooth", block: "start" });
    }
}
function initTooltips() {
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.forEach(el => new bootstrap.Tooltip(el));
}