let chartJsPromise = null;

export async function createPaceChart(canvas, data) {
    
    await ensureChartJs();

    let disposed = false;

    const chart = new Chart(
        canvas,
        createConfiguration(data)
    );

    let darkTheme = isDarkTheme();

    const themeObserver =
        new MutationObserver(() => {
            if (disposed)
                return;

            const currentTheme = isDarkTheme();

            if (currentTheme === darkTheme)
                return;

            darkTheme = currentTheme;

            applyTheme(chart);
            chart.update("none");
        });

    themeObserver.observe(
        document.head,
        {
            childList: true,
            subtree: true,
            characterData: true
        });

    themeObserver.observe(
        document.body,
        {
            attributes: true,
            attributeFilter: ["class", "style"]
        });

    themeObserver.observe(
        document.documentElement,
        {
            attributes: true,
            attributeFilter: ["class", "style"]
        });

    return {
        update(data) {
            chart.data.datasets[0].data = createPoints(data);
            chart.options.scales.x.max = getMaximumDistance(data);
            chart.update();
        },

        dispose() {
            if (disposed)
                return;

            disposed = true;

            themeObserver.disconnect();
            chart.destroy();
        }
    };
}

function createConfiguration(data) {
    const configuration = {
        type: "line",

        data: {
            datasets: [
                {
                    data: createPoints(data),
                    borderWidth: 2,
                    pointRadius: 0,
                    pointHoverRadius: 4,
                    tension: 0
                }
            ]
        },

        options: {
            responsive: true,
            maintainAspectRatio: false,
            parsing: false,
            animation: false,

            interaction: {
                mode: "nearest",
                intersect: false
            },

            plugins: {
                legend: {
                    display: false
                },

                tooltip: {
                    callbacks: {
                        title(items) {
                            if (items.length === 0)
                                return "";

                            return `${formatDistance(
                                items[0].parsed.x
                            )} km`;
                        },

                        label(context) {
                            return `${formatPace(
                                context.parsed.y
                            )} /km`;
                        }
                    }
                }
            },

            scales: {
                x: {
                    type: "linear",
                    min: 0,
                    max: getMaximumDistance(data),

                    title: {
                        display: true,
                        text: "Distance (km)"
                    },

                    ticks: {
                        callback(value) {
                            return formatDistance(value);
                        }
                    }
                },

                y: {
                    reverse: true,

                    title: {
                        display: true,
                        text: "Pace (/km)"
                    },

                    ticks: {
                        callback(value) {
                            return formatPace(value);
                        }
                    }
                }
            }
        }
    };

    applyThemeToConfiguration(configuration);

    return configuration;
}

function createPoints(data) {
    return data.points.map(point => ({
        x: point.distance,
        y: point.pace
    }));
}

function getMaximumDistance(data) {
    if (data.points.length === 0)
        return 0;

    return data.points[data.points.length - 1].distance;
}

function formatDistance(distance) {
    return Number(distance).toFixed(1);
}

function formatPace(secondsPerKilometre) {
    if (!Number.isFinite(secondsPerKilometre))
        return "";

    const totalSeconds = Math.round(secondsPerKilometre);
    const minutes = Math.floor(totalSeconds / 60);
    const seconds = totalSeconds % 60;

    return `${minutes}:${seconds
        .toString()
        .padStart(2, "0")}`;
}

function applyTheme(chart) {
    const colours = getThemeColours();

    chart.options.scales.x.grid.color = colours.grid;
    chart.options.scales.x.ticks.color = colours.text;
    chart.options.scales.x.title.color = colours.text;
    chart.options.scales.y.grid.color = colours.grid;
    chart.options.scales.y.ticks.color = colours.text;
    chart.options.scales.y.title.color = colours.text;
    chart.data.datasets[0].borderColor = colours.line;
}

function applyThemeToConfiguration(configuration) {
    const colours = getThemeColours();

    configuration.options.scales.x.grid = {
        color: colours.grid
    };

    configuration.options.scales.x.ticks.color = colours.text;
    configuration.options.scales.x.title.color = colours.text;
    configuration.options.scales.y.grid = {
        color: colours.grid
    };

    configuration.options.scales.y.ticks.color = colours.text;
    configuration.options.scales.y.title.color = colours.text;
    configuration.data.datasets[0].borderColor = colours.line;
}

function getThemeColours() {
    const styles = getComputedStyle(document.body);

    return {
        text:
            styles
                .getPropertyValue("--mud-palette-text-primary")
                .trim()
            || "#424242",

        grid:
            styles
                .getPropertyValue("--mud-palette-lines-default")
                .trim()
            || "rgba(0, 0, 0, 0.12)",

        line:
            styles
                .getPropertyValue("--mud-palette-primary")
                .trim()
            || "#163A63"
    };
}

function isDarkTheme() {
    const value =
        getComputedStyle(document.body)
            .getPropertyValue("--mud-palette-background")
            .trim();

    const match =
        value.match(/^#?([0-9a-f]{6})$/i);

    if (!match)
        return false;

    const colour = match[1];

    const red = parseInt(colour.substring(0, 2), 16);
    const green = parseInt(colour.substring(2, 4), 16);
    const blue = parseInt(colour.substring(4, 6), 16);

    const brightness =
        red * 0.299
        + green * 0.587
        + blue * 0.114;

    return brightness < 128;
}

async function ensureChartJs() {
    if (window.Chart)
        return;

    chartJsPromise ??= loadScript("https://cdn.jsdelivr.net/npm/chart.js@4.5.0/dist/chart.umd.min.js");

    await chartJsPromise;
}

function loadScript(source) {
    return new Promise((resolve, reject) => {
        const existing = document.querySelector(`script[src="${source}"]`);

        if (existing) {
            if (window.Chart) {
                resolve();
                return;
            }

            existing.addEventListener("load", resolve, { once: true });
            existing.addEventListener("error", reject, { once: true });

            return;
        }

        const script = document.createElement("script");

        script.src = source;
        script.async = true;

        script.onload = resolve;
        script.onerror = reject;

        document.head.appendChild(script);
    });
}