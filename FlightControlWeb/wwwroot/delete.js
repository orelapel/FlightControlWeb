let mark;

// DELETE/api/Flights
function deleteFlightPlan(idFlight) {
    let url = "/api/Flights/" + idFlight;
    $.ajax({
        url: url,
        type: 'DELETE',
        success: function (result) {
            console.log(result);
        }
    });
}

// After clicking the Delete button, implement this.
function deleteRow(idFlight, button) {
    // Remove plane path from map.
    if (mark == idFlight) {
        removeRoute();
    }

    // Remove flight details.
    let i = button.parentNode.parentNode.rowIndex;
    document.getElementById("t01").deleteRow(i);
    let idTable = document.getElementById("flightId").innerText;
    if (idFlight == idTable) {
        removeFlightDetails();
    }

    // Remove flight from server.
    deleteFlightPlan(idFlight);

    // Remove plane from map.
    removePlane(idFlight);

    // Remove flight from planesMap
    planesMap.delete(idFlight);
}