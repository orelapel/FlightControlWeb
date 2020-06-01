// Create map for planes and their ID's.
let planesMap = new Map();

$(document).ready(function () {
    //getFlights();
    getAllFlights();
    setInterval(function () {
        //getFlights();
        getAllFlights();
    }, 3000);
});