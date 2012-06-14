$(document).ready(function () {
    var currentTime = new Date();
    
    // Initialize Start date        
    $("#StartDate").datepicker({
        maxDate: "+0m +0y +0d",
        minDate: "1/1/1999"
    });

    // Initialize end date
    $("#EndDate").datepicker({
        maxDate: "+0m +0y +0d"
    });

    // Add event handler for StartDate date picker's change event
    $("#StartDate").change(function () {
        // Set min date for EndDate datepicker
        var minDate = new Date($(this).val());        
        $("#EndDate").datepicker("option", "minDate", minDate);

        // Caculate and set maxdate for End datepicker
        var maxDate = new Date($(this).val());
        maxDate.setMonth(maxDate.getMonth() + 2);
        var currentDate = new Date;
        maxDate = maxDate > currentDate ? currentDate : maxDate;
        $("#EndDate").datepicker("option", "maxDate", maxDate);

        // Reset End datepicker date
        var currentEndDate = new Date($("#EndDate").val());
        if (currentEndDate > maxDate) $("#EndDate").val(maxDate);
        if (currentEndDate < minDate) $("#EndDate").val(minDate);
    });
});