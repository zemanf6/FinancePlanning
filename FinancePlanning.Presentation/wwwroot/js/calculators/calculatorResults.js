document.addEventListener("DOMContentLoaded", () => {
    initCurrencyDropdown();
    scrollToResultsIfAvailable();
    renderCharts();
    initTooltips();
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

function scrollToResultsIfAvailable() {
    const results = document.getElementById("results");
    if (results) {
        results.scrollIntoView({ behavior: "smooth" });
    }
}

function renderCharts() {
    const data = window.calculationData;
    if (!data || !data.chartData || data.chartData.length === 0) return;

    const { principal, interest, chartData } = data;

    if (!principal || !interest || !Array.isArray(chartData)) return;

    renderPieChart(principal, interest);
    renderBarChart(principal, chartData);
}

function renderPieChart(principal, interest) {
    const container = document.getElementById('pieChartContainer');
    if (!container) return;

    container.innerHTML = '';
    const canvas = document.createElement('canvas');
    canvas.height = 300;
    container.appendChild(canvas);

    new Chart(canvas.getContext('2d'), {
        type: 'pie',
        data: {
            labels: ['Principal', 'Interest'],
            datasets: [{
                data: [principal, interest],
                backgroundColor: ['#0d6efd', '#dc3545']
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false
        }
    });
}

function renderBarChart(principal, chartData) {
    const container = document.getElementById('barChartContainer');
    if (!container) return;

    container.innerHTML = '';
    const canvas = document.createElement('canvas');
    canvas.height = 300;
    container.appendChild(canvas);

    const labels = chartData.map(x => `Period ${x.period}`);
    const principalData = chartData.map(() => principal);
    const interestData = chartData.map(x => x.interestAccumulated);

    new Chart(canvas.getContext('2d'), {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [
                {
                    label: 'Principal',
                    data: principalData,
                    backgroundColor: '#0d6efd',
                    stack: 'total'
                },
                {
                    label: 'Accumulated Interest',
                    data: interestData,
                    backgroundColor: '#dc3545',
                    stack: 'total'
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                x: { stacked: true },
                y: { stacked: true, beginAtZero: true }
            }
        }
    });
}

function initTooltips() {
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.forEach(tooltipTriggerEl => {
        new bootstrap.Tooltip(tooltipTriggerEl);
    });
}
