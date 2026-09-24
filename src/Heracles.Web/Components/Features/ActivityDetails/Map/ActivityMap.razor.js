const routeSourceId = "activity-route";
const routeLayerId = "activity-route-line";

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

        darkTheme = isDarkTheme();
        updateTheme(map);

        updateMap();
    });

    function updateMap() {
        if (disposed || !map.getSource(routeSourceId))
            return;

        updateRoute(map, currentData);

        removeMarkers(markers);
        markers = createMarkers(map, currentData);

        fitActivity(map, currentData);
    }

    return {
        update(data) {
            currentData = data;
            updateMap();
        },

        dispose() {
            if (disposed)
                return;

            disposed = true;

            removeMarkers(markers);
            markers = [];

            themeObserver.disconnect();
            
            map.remove();
        }
    };
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