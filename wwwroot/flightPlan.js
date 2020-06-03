// GET/api/FlightPlan/{id}
function getFlightPlan(idFlight) {
    let flightId = document.getElementById("flightId");
    flightId.innerText = idFlight;
    mark = idFlight;

    let url = "/api/FlightPlan/" + idFlight;
    $.ajax({
        type: "GET",
        url: url,
        contentType: "applicition/json",
        success: successFlightPlan,
        error: function () {
            showError("Error in getting flight Plan");
        }
    });
}

function successFlightPlan(flightPlan) {
    // Show flight details.
    let companyName = document.getElementById("companyName");
    companyName.innerText = flightPlan.company_Name;

    let numPassengers = document.getElementById("passengers");
    numPassengers.innerText = flightPlan.passengers;

    let initialLocation = document.getElementById("initialLocation");
    initialLocation.innerText = flightPlan.initial_Location.latitude.toString() +
                                ', ' + flightPlan.initial_Location.longitude.toString();

    let dateTime = new Date(flightPlan.initial_Location.date_Time);

    let endLocation = document.getElementById("endLocation");
    // Calculate the end latitude and longitude.
    for (let i = 0, seg; seg = flightPlan.segments[i]; i++) {
        dateTime.setSeconds(dateTime.getSeconds() + seg.timespan_Seconds);
        if (i == flightPlan.segments.length - 1) {
            endLocation.innerText = flightPlan.segments[i].latitude.toString() +
                                    ', ' + flightPlan.segments[i].longitude.toString();
        }
    }

    let initialDateTime = document.getElementById("initialDateTime");
    initialDateTime.innerText = dateTime.toUTCString();

    let endDateTime = document.getElementById("endDateTime");
    endDateTime.innerText = dateTime.toUTCString();

    // Add route to plane of this flightPlan.
    addRoute(flightPlan);
}

function removeFlightDetails() {
    // Remove flight details.
    $('label[id*="flightId"]').text('');
    $('label[id*="companyName"]').text('');
    $('label[id*="passengers"]').text('');
    $('label[id*="initialLocation"]').text('');
    $('label[id*="endLocation"]').text('');
    $('label[id*="initialDateTime"]').text('');
    $('label[id*="endDateTime"]').text('');
}