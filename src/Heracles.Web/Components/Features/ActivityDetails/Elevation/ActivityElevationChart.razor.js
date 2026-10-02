let chartJsPromise = null;

const crosshairPlugin = {
    id: "activityCrosshair",

    afterDatasetsDraw(chart) {
        const active = chart.getActiveElements();

        if (active.length === 0)
            return;

        const point = active[0].element;
        const area = chart.chartArea;

        const context = chart.ctx;

        context.save();

        context.beginPath();
        context.moveTo(
            point.x,
            area.top
        );
        context.lineTo(
            point.x,
            area.bottom
        );

        context.lineWidth = 1;
        context.strokeStyle = getThemeColours().text;

        context.globalAlpha = 0.35;
        context.stroke();

        context.restore();
    }
};

export async function createElevationChart(canvas, data, dotNetReference) {
    await ensureChartJs();

    let disposed = false;
    let selectedTrackPointId = null;
    let hoveredTrackPointId = null;

    const chart = new Chart(
        canvas,
        createConfiguration(
            data,
            dotNetReference,
            () => disposed,
            trackPointId => { hoveredTrackPointId = trackPointId; },
            () => hoveredTrackPointId
        )
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
            chart.options.scales.y.min = data.minimumElevation;
            chart.options.scales.y.max = data.maximumElevation;
            selectTrackPoint(chart, selectedTrackPointId);
            chart.update();
        },

        selectTrackPoint(trackPointId) {
            selectedTrackPointId = trackPointId;
            selectTrackPoint(chart, trackPointId);
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

function createConfiguration(
    data,
    dotNetReference,
    isDisposed,
    setHoveredTrackPointId,
    getHoveredTrackPointId) {
    
    const configuration = {
        type: "line",

        plugins: [
            crosshairPlugin
        ],

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

            onHover(event, activeElements, chart) {
                if (isDisposed())
                    return;

                if (activeElements.length === 0) {
                    if (getHoveredTrackPointId() !== null) {
                        setHoveredTrackPointId(null);
                        void dotNetReference.invokeMethodAsync("ClearTrackPoint");
                    }

                    return;
                }

                const point = chart.data.datasets[0].data[activeElements[0].index];

                if (point.trackPointId === getHoveredTrackPointId())
                    return;

                setHoveredTrackPointId(point.trackPointId);

                void dotNetReference.invokeMethodAsync("SelectTrackPoint", point.trackPointId);
            },

            onLeave() {
                if (isDisposed() || getHoveredTrackPointId() === null)
                    return;

                setHoveredTrackPointId(null);

                void dotNetReference.invokeMethodAsync("ClearTrackPoint");
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
                            return `${formatElevation(
                                context.parsed.y
                            )} m`;
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
                    min: data.minimumElevation,
                    max: data.maximumElevation,

                    title: {
                        display: true,
                        text: "Elevation (m)"
                    },

                    ticks: {
                        callback(value) {
                            return formatElevation(value);
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
        trackPointId: point.trackPointId,
        x: point.distance,
        y: point.elevation
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

function formatElevation(elevation) {
    return Math.round(Number(elevation));
}

function selectTrackPoint(chart, trackPointId) {
    if (trackPointId === null) {
        chart.setActiveElements([]);
        chart.tooltip.setActiveElements(
            [],
            { x: 0, y: 0 }
        );
        chart.update("none");
        return;
    }

    const index = chart.data.datasets[0].data.findIndex(point => point.trackPointId === trackPointId);

    if (index < 0) {
        chart.setActiveElements([]);
        chart.tooltip.setActiveElements(
            [],
            { x: 0, y: 0 }
        );
        chart.update("none");
        return;
    }

    const activeElement = {
        datasetIndex: 0,
        index
    };

    const point = chart.getDatasetMeta(0).data[index];

    chart.setActiveElements([
        activeElement
    ]);

    chart.tooltip.setActiveElements(
        [activeElement],
        {
            x: point.x,
            y: point.y
        }
    );

    chart.update("none");
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