// Create map for planes and their ID's.
let planesMap = new Map();

// Start running program.
$(document).ready(function () {
    //getFlights();
    getAllFlights();
    setInterval(function () {
        //getFlights();
        getAllFlights();
    }, 3000);
});