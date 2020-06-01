// GET/api/Flights?relative_to=<DATE_TIME>&sync_all
function getAllFlights() {
    // Change time to correct format.
    let d = new Date(Date().toString('en-US', { timeZone: "Etc/GMT-0" }));
    let piece = d.toISOString();
    let end = piece.length - 5;
    let date = piece.substring(0, end);
    date = date + 'Z';
    let url = "/api/Flights?relative_to=" + date + "&sync_all";

    $.ajax({
        type: "GET",
        url: url,
        datatype: 'json',
        success: successGetAllFlights,
        error: function () {
            showError("Error getAllFlights");
        }
    });
}

let iStillFlying = false;

function successGetAllFlights(flights) {
    // Check if the flight has finished.
    // Go over the map and check if we still get the flights that on the map from the server.
    // If not, it means the flight has finished and we can delete it.
    let listLayer = planesMap.values();
    for (let layer of listLayer) {
        let id = layer.layerID;
        let isIn = id.indexOf("@");
        if (isIn != -1) {
            let end = id.length - 1;
            id = id.substring(0, end);
        }
        iStillFlying = false;
        isFinished(flights, id);

        // If filght has finished, delete it.
        if (iStillFlying == false) {
            // Delete both - flight and @.
            let x = document.getElementById("deleteRow" + id);
            let i = x.parentNode.parentNode.rowIndex;
            document.getElementById("t01").deleteRow(i);

            // Remove plane from map.
            removePlane(id);
            planesMap.delete(id);
            planesMap.delete(id + "@");

            // Remove plane path from map.
            if (mark == id) {
                removeRoute();
            }
        }
    }

    // Update or/and add flighte to the tables.
    flights.forEach(function (flight, i) {
        // Get flight id.
        let idFlight = flight.flight_Id;

        // Get flight current location.
        let longitude = flight.longitude;
        let latitude = flight.latitude;

        if (!planesMap.has(idFlight)) {
            // Create airplains icons.
            let layer = L.marker([latitude, longitude], { icon: airplane1 });
            layer.layerID = idFlight;
            let layer1 = L.marker([latitude, longitude], { icon: airplane2 });
            layer1.layerID = idFlight + "@";

            // Add to planesMap.
            planesMap.set(idFlight, layer);
            planesMap.set(idFlight + "@", layer1);

            // Add rows to tables.
            if (!flight.is_External) {
                $('#t01').append(
                 '<tr id="' + idFlight + '"><td onClick="showChosenFlight(id)" id="'
                 + idFlight + '">' + idFlight + '</td><td onClick="showChosenFlight(id)" id="'
                 + idFlight + '"> ' + flight.company_Name +
                 '</td><td id="deleteButton"><button type="button" class="btn" value="Delete" id="'
                    + "deleteRow" + idFlight + '"><span class="fa fa-close"></span>'
                    + '<span class="submit - text"> Delete</span></button></td></tr>');
            } else {
                $('#t02').append('<tr onClick="showChosenFlight(id)" id="' + idFlight + '"><td>'
                    + idFlight + '</td><td> ' + flight.company_Name + '</td></tr>');
            }

            // When click on Delete, delete row.
            let del = document.getElementById("deleteRow" + idFlight);
            if (del != null) {
                del.addEventListener("click", function () {
                    deleteRow(idFlight, del);
                });
            }

            // add plane to map.
            addNewPlaneToMap(layer, layer1);
        } else {
            // Update plane location.
            let layer = planesMap.get(idFlight);
            let layer1 = planesMap.get(idFlight + "@");
            layer.setLatLng([latitude, longitude]);
            layer1.setLatLng([latitude, longitude]);
        }
    });
}

// If flight is not finished, change iStillFlying to be true .
function isFinished(flights, id) {
    for (let flight of flights) {
        //Get flight id.
        let idFlight = flight.flight_Id;
        if (id == idFlight) {
            iStillFlying = true;
            break;
        }
    }
}