// POST/api/FlightPlan
function postFlight(jsonString) {
    let xhr = new XMLHttpRequest();
    xhr.open("POST", 'api/FlightPlan', true);

    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE) {
            if (this.status == 400 || this.status == 404 ||
                this.status == 409 || this.status == 500) {
                showError("File not a valid json");
            }
            // Request finished.
        }
    }
    xhr.send(jsonString);
}

// After opening file, read it.
function readFiles() {
    let file = document.getElementById("fileItem").files[0];
    let reader = new FileReader();

    try {
        reader.readAsText(file);
    } catch {
        showError("Cannot read file, try submit file again");
    }

    let dataString;
    reader.onload = function () {
        dataString = reader.result.replace('/r', '');
        postFlight(dataString);
    };

    reader.onerror = function () {
        showError(reader.error);
    };
}