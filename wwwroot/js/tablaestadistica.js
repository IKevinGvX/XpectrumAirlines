// tablaestadistica.js
$(document).ready(function () {
    $('#vuelosTable').DataTable({
        "paging": true,
        "pageLength": 5,
        "searching": true,
        "ordering": true,
        "info": true,
        "lengthChange": false,
        "language": {
            "sSearch": "Buscar:",
            "sLengthMenu": "Mostrar _MENU_ registros",
            "sInfo": "Mostrando _START_ a _END_ de _TOTAL_ registros",
            "oPaginate": {
                "sPrevious": "Anterior",
                "sNext": "Siguiente"
            }
        }
    });
});
