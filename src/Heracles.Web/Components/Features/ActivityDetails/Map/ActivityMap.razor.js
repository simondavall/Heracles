const routeSourceId = "activity-route";
const routeLayerId = "activity-route-line";

const distanceSourceId = "activity-distance-markers";
const distanceLayerId = "activity-distance-symbols";

const fadeDuration = 250;

const distanceMarkerColour = "#163A63";

export function createMap(container, accessToken, data) {
    const map = new mapboxgl.Map({
        container,
        accessToken,
        style: "mapbox://styles/mapbox/outdoors-v11",
        
        center: [0, 0],
        zoom: 2,

        pitch: 0,
        bearing: 0,

        config: {
            basemap: {
                show3dObjects: false,
                lightPreset: "day"
            }
        }
    });
    
    map.addControl(
        new mapboxgl.NavigationControl({
            showCompass: false
        }),
        "top-right"
    );

    let markers = [];
    let currentData = data;
    let disposed = false;

    let transitionTimer = null;
    let transitionVersion = 0;
    let hasRenderedActivity = false;

    let renderVersion = 0;
    
    function transitionToActivity(data) {
        currentData = data;

        if (!hasRenderedActivity) {
            updateMap();
            return;
        }

        const version = ++transitionVersion;

        clearTimeout(transitionTimer);

        container.classList.add("is-transitioning");

        transitionTimer = setTimeout(async () => {
            if (disposed || version !== transitionVersion)
                return;

            await updateMap();

            if (disposed || version !== transitionVersion)
                return;

            requestAnimationFrame(() => {
                if (disposed || version !== transitionVersion)
                    return;

                container.classList.remove("is-transitioning");
            });
        }, fadeDuration);
    }
    
    let darkTheme = null;

    const themeObserver = new MutationObserver(() => {
        if (disposed)
            return;

        const currentTheme = isDarkTheme();

        if (currentTheme === darkTheme)
            return;

        darkTheme = currentTheme;
        updateTheme(map);
    });

    themeObserver.observe(document.head, {
        childList: true,
        subtree: true,
        characterData: true
    });

    themeObserver.observe(document.body, {
        attributes: true,
        attributeFilter: ["class", "style"]
    });

    themeObserver.observe(document.documentElement, {
        attributes: true,
        attributeFilter: ["class", "style"]
    });

    map.on("style.load", () => {
        if (disposed)
            return;

        addRouteLayer(map);
        addDistanceLayer(map);

        darkTheme = isDarkTheme();
        updateTheme(map);

        void updateMap().then(() => {
            if (!disposed)
                hasRenderedActivity = true;
        });
    });

    async function updateMap() {
        if (
            disposed ||
            !map.getSource(routeSourceId) ||
            !map.getSource(distanceSourceId)
        ) {
            return;
        }

        const version = ++renderVersion;
        const data = currentData;

        await registerDistanceMarkerImages(
            map,
            data,
            () => !disposed && version === renderVersion
        );

        if (disposed || version !== renderVersion)
            return;

        updateRoute(map, data);
        updateDistanceMarkers(map, data);

        removeMarkers(markers);
        markers = createMarkers(map, data);

        fitActivity(map, data);
    }

    return {
        update(data) {
            transitionToActivity(data);
        },

        dispose() {
            if (disposed)
                return;

            disposed = true;
            renderVersion++;

            clearTimeout(transitionTimer);
            themeObserver.disconnect();

            removeMarkers(markers);
            markers = [];

            map.remove();
        }
    };
}

async function registerDistanceMarkerImages(map, data, isCurrent){
    for (const marker of data.distanceMarkers) {
        if (!isCurrent())
            return;

        const imageId = `distance-marker-${marker.distance}`;

        if (map.hasImage(imageId))
            continue;

        const svg = createDistanceMarkerSvg(
            marker.distance,
            distanceMarkerColour
        );

        const image = await loadSvgImage(svg);

        if (!isCurrent())
            return;

        if (!map.hasImage(imageId))
            map.addImage(imageId, image);
    }
}

function createDistanceMarkerSvg(distance, backgroundColour) {
    return `
        <svg xmlns="http://www.w3.org/2000/svg"
             width="28"
             height="36"
             viewBox="0 0 28 36">

            <rect
                x="1"
                y="1"
                width="26"
                height="34"
                rx="3"
                fill="${backgroundColour}"
                stroke="#FFFFFF"
                stroke-width="1.5"
            />

            <text
                x="14"
                y="17"
                text-anchor="middle"
                font-family="Arial, sans-serif"
                font-size="16"
                font-weight="bold"
                fill="#FFFFFF"
            >${distance}</text>

            <text
                x="14"
                y="28"
                text-anchor="middle"
                font-family="Arial, sans-serif"
                font-size="10"
                fill="#FFFFFF"
            >km</text>

        </svg>
    `;
}

function loadSvgImage(svg) {
    return new Promise((resolve, reject) => {
        const image = new Image();

        image.onload = () => resolve(image);
        image.onerror = reject;

        image.src =
            "data:image/svg+xml;charset=utf-8," +
            encodeURIComponent(svg);
    });
}

function addRouteLayer(map) {
    if (!map.getSource(routeSourceId)) {
        map.addSource(routeSourceId, {
            type: "geojson",
            data: emptyRoute()
        });
    }

    if (!map.getLayer(routeLayerId)) {
        map.addLayer({
            id: routeLayerId,
            type: "line",
            source: routeSourceId,
            slot: "top",

            layout: {
                "line-join": "round",
                "line-cap": "round"
            },

            paint: {
                "line-color": "#E53935",
                "line-width": 4,
                "line-opacity": 0.95,
                "line-emissive-strength": 1
            }
        });
    }
}

function emptyRoute() {
    return {
        type: "Feature",
        properties: {},
        geometry: {
            type: "MultiLineString",
            coordinates: []
        }
    };
}

function createRoute(data) {
    const coordinates = data.segments
        .map(segment =>
            segment.coordinates.map(point => [
                point.longitude,
                point.latitude
            ]))
        .filter(segment => segment.length >= 2);

    return {
        type: "Feature",
        properties: {},
        geometry: {
            type: "MultiLineString",
            coordinates
        }
    };
}

function updateRoute(map, data) {
    const source = map.getSource(routeSourceId);

    if (!source)
        return;

    source.setData(createRoute(data));
}

function addDistanceLayer(map) {
    if (!map.getSource(distanceSourceId)) {
        map.addSource(distanceSourceId, {
            type: "geojson",
            data: emptyDistanceMarkers()
        });
    }

    if (!map.getLayer(distanceLayerId)) {
        map.addLayer({
            id: distanceLayerId,
            type: "symbol",
            source: distanceSourceId,
            slot: "top",

            layout: {
                "icon-image": ["get", "icon"],
                "icon-size": 0.8,
                "icon-anchor": "center",
                "icon-allow-overlap": true,
                "icon-ignore-placement": false
            },

            paint: {
                "icon-opacity": 1,
                "icon-emissive-strength": 1
            }
        });
    }
}

function emptyDistanceMarkers() {
    return {
        type: "FeatureCollection",
        features: []
    };
}

function createDistanceMarkers(data) {
    return {
        type: "FeatureCollection",

        features: data.distanceMarkers.map(marker => ({
            type: "Feature",

            properties: {
                distance: marker.distance,
                icon: `distance-marker-${marker.distance}`
            },

            geometry: {
                type: "Point",

                coordinates: [
                    marker.longitude,
                    marker.latitude
                ]
            }
        }))
    };
}

function updateDistanceMarkers(map, data) {
    const source = map.getSource(distanceSourceId);

    if (!source)
        return;

    source.setData(createDistanceMarkers(data));
}

function createMarkers(map, data) {
    const segments = data.segments
        .filter(segment => segment.coordinates.length > 0);

    if (segments.length === 0)
        return [];

    const markers = [];

    const firstSegment = segments[0];
    const lastSegment = segments[segments.length - 1];

    markers.push(
        createMarker(
            map,
            firstSegment.coordinates[0],
            "start",
            "Start"
        )
    );

    for (let index = 0; index < segments.length - 1; index++) {
        const currentSegment = segments[index];
        const nextSegment = segments[index + 1];

        markers.push(
            createMarker(
                map,
                currentSegment.coordinates[
                currentSegment.coordinates.length - 1
                    ],
                "pause",
                "Pause"
            )
        );

        markers.push(
            createMarker(
                map,
                nextSegment.coordinates[0],
                "resume",
                "Resume"
            )
        );
    }

    markers.push(
        createMarker(
            map,
            lastSegment.coordinates[
            lastSegment.coordinates.length - 1
                ],
            "finish",
            "Finish"
        )
    );

    return markers;
}

function createMarker(map, coordinate, type, title) {
    const element = document.createElement("div");

    element.className = `activity-map-marker activity-map-marker-${type}`;
    element.title = title;
    element.setAttribute("aria-label", title);

    return new mapboxgl.Marker({
        element,
        anchor: "center"
    })
        .setLngLat([
            coordinate.longitude,
            coordinate.latitude
        ])
        .addTo(map);
}

function removeMarkers(markers) {
    for (const marker of markers)
        marker.remove();
}

function fitActivity(map, data) {
    const coordinates = data.segments
        .flatMap(segment => segment.coordinates);

    if (coordinates.length === 0)
        return;

    const first = coordinates[0];

    const bounds = new mapboxgl.LngLatBounds(
        [first.longitude, first.latitude],
        [first.longitude, first.latitude]
    );

    for (const coordinate of coordinates) {
        bounds.extend([
            coordinate.longitude,
            coordinate.latitude
        ]);
    }

    map.fitBounds(bounds, {
        padding: 60,
        maxZoom: 16,
        duration: 0
    });
}

function isDarkTheme() {
    const value = getComputedStyle(document.body)
        .getPropertyValue("--mud-palette-background")
        .trim();

    const match = value.match(
        /^#?([0-9a-f]{6})$/i
    );

    if (!match)
        return false;

    const colour = match[1];

    const red = parseInt(colour.substring(0, 2), 16);
    const green = parseInt(colour.substring(2, 4), 16);
    const blue = parseInt(colour.substring(4, 6), 16);

    const brightness =
        red * 0.299 +
        green * 0.587 +
        blue * 0.114;

    return brightness < 128;
}

function updateTheme(map) {
    if (!map.isStyleLoaded())
        return;

    map.setConfigProperty(
        "basemap",
        "lightPreset",
        isDarkTheme() ? "night" : "day"
    );
}