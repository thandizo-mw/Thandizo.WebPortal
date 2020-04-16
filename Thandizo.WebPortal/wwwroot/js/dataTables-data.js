/*DataTable Init*/

$(document).ready(function() {
	"use strict";
	
    $('#datable_1').DataTable();

    $('#datable_2').DataTable({ "lengthChange": false });

    $('#dtDataTable').DataTable({
        "lengthMenu": [6, 8]
    });

    $('#dtScrollableDynamicTable').DataTable({
        scrollY: '45vh',
        scrollCollapse: true,
        paging: false
    });

    $('#dtScrollableFixedTable').DataTable({
        scrollY: '200px',
        scrollCollapse: true,
        paging: false
    });

    $('#dtScrollableNoDefaults').DataTable({
        "searching": false,
        "lengthMenu": [6, 8],
        "ordering": false,
        scrollY: '45vh',
        scrollCollapse: true,
        paging: false
    });
});
