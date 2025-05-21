const predefinedAssets = {
    "World Stocks": {
        expectedReturn: 7.0,
        stdDevs: { conservative: 10, balanced: 15, aggressive: 20 }
    },
    "US Stocks": {
        expectedReturn: 8.0,
        stdDevs: { conservative: 12, balanced: 17, aggressive: 22 }
    },
    "International Stocks": {
        expectedReturn: 6.0,
        stdDevs: { conservative: 11, balanced: 16, aggressive: 21 }
    },
    "Emerging Markets": {
        expectedReturn: 9.0,
        stdDevs: { conservative: 15, balanced: 20, aggressive: 30 }
    },
    "Government Bonds": {
        expectedReturn: 3.0,
        stdDevs: { conservative: 3, balanced: 5, aggressive: 7 }
    },
    "Corporate Bonds": {
        expectedReturn: 4.0,
        stdDevs: { conservative: 5, balanced: 7, aggressive: 10 }
    },
    "High-yield Bonds": {
        expectedReturn: 6.0,
        stdDevs: { conservative: 8, balanced: 12, aggressive: 18 }
    },
    "Money Market": {
        expectedReturn: 1.0,
        stdDevs: { conservative: 0, balanced: 1, aggressive: 2 }
    }
};


document.addEventListener("DOMContentLoaded", () => {
    const tableBody = document.querySelector("#portfolioTable tbody");
    const addBtn = document.getElementById("addAssetRow");
    const maxRows = 10;


    function recalculatePortfolioStats() {
        const rows = tableBody.querySelectorAll("tr");
        let totalWeight = 0;
        let weightedSum = 0;
        let weightedStdDevSum = 0;

        rows.forEach(row => {
            const weightInput = row.querySelector(".weight");
            const returnInput = row.querySelector(".expected-return");
            const stddevInput = row.querySelector(".stddev");

            const weight = parseFloat(weightInput.value) || 0;
            const expectedReturn = parseFloat(returnInput.value) || 0;
            const stddev = parseFloat(stddevInput.value) || 0;

            totalWeight += weight;
            weightedSum += weight * expectedReturn;
            weightedStdDevSum += weight * stddev;
        });

        const weightedAverage = totalWeight > 0 ? (weightedSum / totalWeight) : 0;
        const weightedVolatility = totalWeight > 0 ? (weightedStdDevSum / totalWeight) : 0;

        document.getElementById("totalWeight").innerText = totalWeight.toFixed(2);
        document.getElementById("averageReturn").innerText = weightedAverage.toFixed(2);
        document.getElementById("calculatedVolatility").innerText = weightedVolatility.toFixed(2);

        const statsBox = document.getElementById("portfolioStats");
        const tolerance = 0.05;
        if (Math.abs(totalWeight - 100) > tolerance) {
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
        const assetInput = row.querySelector(".asset-name");
        const assetSelector = row.querySelector(".asset-selector");
        const volatilitySelector = row.querySelector(".volatility-selector");
        const expectedReturnInput = row.querySelector(".expected-return");
        const stddevInput = row.querySelector(".stddev");
        if (removeBtn) {
            removeBtn.addEventListener("click", () => {
                row.remove();
                updateInputNames();
                recalculatePortfolioStats();
            });
        }

        function updateFieldsFromSelection() {
            const assetKey = assetSelector.value;
            const volLevel = volatilitySelector.value;

            if (predefinedAssets[assetKey]) {
                assetInput.value = assetKey;
                expectedReturnInput.value = predefinedAssets[assetKey].expectedReturn;
                stddevInput.value = predefinedAssets[assetKey].stdDevs[volLevel];
                recalculatePortfolioStats();
            }
        }

        if (assetSelector) {
            assetSelector.addEventListener("change", updateFieldsFromSelection);
        }
        if (volatilitySelector) {
            volatilitySelector.addEventListener("change", updateFieldsFromSelection);
        }
    }

    function updateInputNames() {
        const rows = tableBody.querySelectorAll("tr");
        rows.forEach((row, index) => {
            row.querySelectorAll("input, select").forEach(el => {
                if (!el.name) return;

                if (el.name.includes(".AssetName")) el.name = `PortfolioItems[${index}].AssetName`;
                if (el.name.includes(".ExpectedReturn")) el.name = `PortfolioItems[${index}].ExpectedReturn`;
                if (el.name.includes(".Weight")) el.name = `PortfolioItems[${index}].Weight`;
                if (el.name.includes(".StandardDeviation")) el.name = `PortfolioItems[${index}].StandardDeviation`;
            });
        });
    }

    addBtn.addEventListener("click", () => {
        const rowCount = tableBody.querySelectorAll("tr").length;
        if (rowCount >= maxRows) return;

        const row = document.createElement("tr");
        row.innerHTML = `
        <td><input type="text" name="PortfolioItems[${rowCount}].AssetName" class="form-control asset-name" /></td>
        <td>
            <select class="form-select form-select-sm asset-selector">
                <option value="">-- Select --</option>
                <option value="World Stocks">World Stocks</option>
                <option value="US Stocks">US Stocks</option>
                <option value="International Stocks">International Stocks</option>
                <option value="Emerging Markets">Emerging Markets</option>
                <option value="Government Bonds">Government Bonds</option>
                <option value="Corporate Bonds">Corporate Bonds</option>
                <option value="High-yield Bonds">High-yield Bonds</option>
                <option value="Money Market">Money Market</option>
            </select>
        </td>
        <td>
            <select class="form-select form-select-sm volatility-selector">
                <option value="balanced" selected>Balanced</option>
                <option value="conservative">Conservative</option>
                <option value="aggressive">Aggressive</option>
            </select>
        </td>
        <td><input type="number" step="0.01" name="PortfolioItems[${rowCount}].ExpectedReturn" class="form-control expected-return" /></td>
        <td><input type="number" step="0.01" name="PortfolioItems[${rowCount}].Weight" class="form-control weight" /></td>
        <td><input type="number" step="0.01" name="PortfolioItems[${rowCount}].StandardDeviation" class="form-control stddev" /></td>
        <td><button type="button" class="btn btn-danger btn-sm remove-row"><i class="bi bi-trash"></i></button></td>
    `;
        tableBody.appendChild(row);
        updateInputNames();
        bindRowEvents(row);
        recalculatePortfolioStats();
    });

    tableBody.querySelectorAll("tr").forEach(bindRowEvents);
    recalculatePortfolioStats();

    tableBody.querySelectorAll("tr").forEach(row => {
        bindRowEvents(row);
        const assetInput = row.querySelector(".asset-name");
        const assetSelector = row.querySelector(".asset-selector");

        if (assetInput && assetSelector) {
            assetSelector.value = assetInput.value;
        }
    });
});

