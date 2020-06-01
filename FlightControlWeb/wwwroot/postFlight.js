// POST/api/FlightPlan
function postFlight(jsonString) {
    let xhr = new XMLHttpRequest();
    xhr.open("POST", 'api/FlightPlan', true);

    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE && this.status === 200) {
            // Request finished.
        }
    }
    xhr.send(jsonString);
}

function readFiles() {
    let file = document.getElementById("fileItem").files[0];
    let reader = new FileReader();

    reader.readAsText(file);

    let dataString;
    reader.onload = function () {
        dataString = reader.result.replace('/r', '');
        postFlight(dataString);
    };

    reader.onerror = function () {
        showError(reader.error);
    };
}