$(document).ready(function () {
    $("#SearchBar").on("keyup", function () {
        var value = $(this).val().toLowerCase();
        $("#myproductList #items").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });
});