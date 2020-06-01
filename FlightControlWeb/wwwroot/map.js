let map = L.map('map').setView([0, 0], 1);

// TODO: MORE THAN 100
L.tileLayer('https://api.maptiler.com/maps/streets/{z}/{x}/{y}.png?key=JZeDrYmO2BGmKe83fmkO', {
    attribiution: '<a href="https://www.maptiler.com/copyright/" target="_blank">&copy; MapTiler</a> <a href="https://www.openstreetmap.org/copyright" target="_blank">&copy; OpenStreetMap contributors</a>',
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
            clearMap();
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
    layer = planesMap.get(idFlight);
    layer.remove();
    layer1 = planesMap.get(idFlight + "@");
    layer1.remove();
}

// Add flight rout to plane.
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

// Remove flight rout of plane.
function clearMap() {
    for (i in map._layers) {
        if (map._layers[i]._path != undefined) {
            try {
                map.removeLayer(map._layers[i]);
            }
            catch (e) {
                console.log("problem with " + e + map._layers[i]);
            }
        }
    }
}

function clearMapWithoutLine() {
    for (i in map._layers) {
        if (map._layers[i]._path != undefined) {
            try {
                if (map._layers[i]._path == undefined) {
                    map.removeLayer(map._layers[i]);
                }
            }
            catch (e) {
                console.log("problem with " + e + map._layers[i]);
            }
        }
    }
}