document.addEventListener("DOMContentLoaded", () => {
    const tableBody = document.querySelector("#portfolioTable tbody");
    const addBtn = document.getElementById("addAssetRow");
    const maxRows = 10;

    function recalculatePortfolioStats() {
        const rows = tableBody.querySelectorAll("tr");
        let totalWeight = 0;
        let weightedSum = 0;

        rows.forEach(row => {
            const weightInput = row.querySelector(".weight");
            const returnInput = row.querySelector(".expected-return");

            const weight = parseFloat(weightInput.value) || 0;
            const expectedReturn = parseFloat(returnInput.value) || 0;

            totalWeight += weight;
            weightedSum += weight * expectedReturn;
        });

        const weightedAverage = totalWeight > 0 ? (weightedSum / totalWeight) : 0;

        document.getElementById("totalWeight").innerText = totalWeight.toFixed(2);
        document.getElementById("averageReturn").innerText = weightedAverage.toFixed(2);
        document.getElementById("CalculatedExpectedReturn").value = weightedAverage.toFixed(6);

        const statsBox = document.getElementById("portfolioStats");
        if (Math.round(totalWeight) !== 100) {
            statsBox.classList.remove("alert-secondary");
            statsBox.classList.add("alert-warning");
        } else {
            statsBox.classList.remove("alert-warning");
            statsBox.classList.add("alert-secondary");
        }
    }

    function bindRowEvents(row) {
        const inputs = row.querySelectorAll("input");
        inputs.forEach(input => input.addEventListener("input", recalculatePortfolioStats));

        const removeBtn = row.querySelector(".remove-row");
        if (removeBtn) {
            removeBtn.addEventListener("click", () => {
                row.remove();
                recalculatePortfolioStats();
                updateInputNames();
            });
        }
    }

    function updateInputNames() {
        const rows = tableBody.querySelectorAll("tr");
        rows.forEach((row, index) => {
            row.querySelectorAll("input").forEach(input => {
                if (input.name.includes(".AssetName")) input.name = `PortfolioItems[${index}].AssetName`;
                if (input.name.includes(".ExpectedReturn")) input.name = `PortfolioItems[${index}].ExpectedReturn`;
                if (input.name.includes(".Weight")) input.name = `PortfolioItems[${index}].Weight`;
            });
        });
    }

    addBtn.addEventListener("click", () => {
        const rowCount = tableBody.querySelectorAll("tr").length;
        if (rowCount >= maxRows) return;

        const row = document.createElement("tr");
        row.innerHTML = `
            <td><input type="text" name="PortfolioItems[${rowCount}].AssetName" class="form-control asset-name" /></td>
            <td><input type="number" step="0.01" name="PortfolioItems[${rowCount}].ExpectedReturn" class="form-control expected-return" /></td>
            <td><input type="number" step="0.01" name="PortfolioItems[${rowCount}].Weight" class="form-control weight" /></td>
            <td><button type="button" class="btn btn-danger btn-sm remove-row"><i class="bi bi-trash"></i></button></td>
        `;
        tableBody.appendChild(row);
        bindRowEvents(row);
        recalculatePortfolioStats();
    });

    tableBody.querySelectorAll("tr").forEach(bindRowEvents);
    recalculatePortfolioStats();
});
