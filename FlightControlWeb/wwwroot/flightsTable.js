// Highlight row after clicking it
let preEl;
let orgBColor;
let orgTColor;
function HighLightTR(el, backColor) {
    let row = document.getElementById(el);
    if (backColor == "#d25dbb") {
        orgBColor = row.bgColor;
        orgTColor = row.style.color;
        row.bgColor = backColor;
        preEl = row
    } else if (typeof (preEl) != 'undefined') {
        preEl.bgColor = orgBColor;
    }
}

// Return all rows to their original color.
function returnRowsToOriginalColor() {
    let listLayer = planesMap.values();
    for (let layer of listLayer) {
        let id = layer.layerID;
        let isIn = id.indexOf("@");
        if (isIn == -1) {
            HighLightTR(id, '#fff');
        }
    }
}

function showChosenFlight(idFlight) {
    // When click on row of flight - change color.
    returnRowsToOriginalColor();
    HighLightTR(idFlight, '#d25dbb');
    getFlightPlan(idFlight);

    // Get plane from map and change its color.
    returnPlanesToOriginalColor();
    layer = planesMap.get(idFlight);
    layer1 = planesMap.get(idFlight + "@");

    changePlaneColor(layer, layer1);
}

function removeShownChosenFlight(idFlight) {
    removeFlightDetails();
    HighLightTR(idFlight, '#fff');
    if (mark != idFlight) {
        clearMapWithoutLine();
    } else {
        clearMap();
    }
}