document.addEventListener("DOMContentLoaded", function () {
    const principal = window.calculationData.principal;
    const interest = window.calculationData.interest;
    const chartData = window.calculationData.chartData;

    const pieCtx = document.getElementById('pieChart')?.getContext('2d');
    if (pieCtx) {
        new Chart(pieCtx, {
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

    const barCtx = document.getElementById('barChart')?.getContext('2d');
    if (barCtx && chartData) {
        const labels = chartData.map(x => "Period " + x.period);
        const principalData = chartData.map(() => principal);
        const interestData = chartData.map(x => x.interestAccumulated);

        new Chart(barCtx, {
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
});
