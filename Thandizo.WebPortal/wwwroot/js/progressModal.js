$(document).ready(function () {
    $('#progressModal').modal('hide');

    $("form").submit(function (e) {
        if ($(this).hasClass("hide-progress-window")) {
            $('#progressModal').modal('hide');
        } else {
            $('#progressModal').modal('show');
        }
    });
});