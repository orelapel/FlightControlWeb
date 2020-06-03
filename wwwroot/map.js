let map = L.map('map').setView([0, 0], 1);

L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution:
        '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
}).addTo(map);

// Create airplane icon.
let airplaneIcon = L.Icon.extend({
    options: {
        // Size of the icon.
        iconSize: [35, 50],
    }
});

let airplane1 = new airplaneIcon({ iconUrl: 'airplane.png' }),
    airplane2 = new airplaneIcon({ iconUrl: 'purpleAirplane.png' });

function addNewPlaneToMap(layer, layer1) {
    layer.addTo(map);

    // After clicking on the airplane, change it's color.
    layer.on('click', function () {
        returnPlanesToOriginalColor();
        showChosenFlight(layer.layerID);
    });

    // After clicking on the map, change the airplane color back.
    map.on('click', function () {
        returnPlanesToOriginalColor();
        removeShownChosenFlight(layer1.layerID);
    });
}

// Change all the airplane's color to blue.
function returnPlanesToOriginalColor() {
    let listLayer = planesMap.values();
    for (let layer of listLayer) {
        let id = layer.layerID;
        let isIn = id.indexOf("@");
        if (isIn == -1) {
            let id1 = id + "@";
            let layer1 = planesMap.get(id1);
            changePlaneColorBack(layer, layer1);

            // Remove flight route.
            removeRoute();
        }
    }
}

// Change plane color to pink.
function changePlaneColor(layer, layer1) {
    layer.remove();
    // The same position, color change.
    layer1.addTo(map);
}

// Change plane color to blue.
function changePlaneColorBack(layer, layer1) {
    layer1.remove();
    layer.addTo(map);
}

//Remove plane from map.
function removePlane(idFlight) {
    let layer = planesMap.get(idFlight);
    layer.remove();
    let layer1 = planesMap.get(idFlight + "@");
    layer1.remove();
}

// Add flight route to plane.
function addRoute(flightPlan) {
    let latlngs = [
        [flightPlan.initial_Location.latitude, flightPlan.initial_Location.longitude]];
    for (let i = 0, seg; seg = flightPlan.segments[i]; i++) {
        let coordinate = [seg.latitude, seg.longitude];
        latlngs.push(coordinate);
    }
    let polyline = L.polyline(latlngs, { color: 'plum' }).addTo(map);
    map.fitBounds(polyline.getBounds());
}

// Remove flight route of plane.
function removeRoute() {
    for (let i in map._layers) {
        if (map._layers[i]._path != undefined) {
            map.removeLayer(map._layers[i]);
        }
    }
}