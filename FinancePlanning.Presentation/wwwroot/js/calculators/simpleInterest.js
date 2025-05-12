document.addEventListener("DOMContentLoaded", () => {
    initCurrencyDropdown();
    scrollToResultsIfAvailable();
    renderCharts();
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
    if (!data) return;

    const { principal, interest, chartData } = data;
    if (!principal || !interest || !chartData) return;

    renderPieChart(principal, interest);
    renderBarChart(principal, chartData);
}

function renderPieChart(principal, interest) {
    const ctx = document.getElementById('pieChart')?.getContext('2d');
    if (!ctx) return;

    new Chart(ctx, {
        type: 'pie',
        data: {
            labels: ['Principal', 'Interest'],
            datasets: [{
                data: [principal, interest],
                backgroundColor: ['#0d6efd', '#dc3545']
            }]
        }
    });
}

function renderBarChart(principal, chartData) {
    const ctx = document.getElementById('barChart')?.getContext('2d');
    if (!ctx) return;

    const labels = chartData.map(x => "Period " + x.period);
    const principalData = chartData.map(() => principal);
    const interestData = chartData.map(x => x.interestAccumulated);

    new Chart(ctx, {
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
            scales: {
                x: { stacked: true },
                y: { stacked: true, beginAtZero: true }
            }
        }
    });
}
