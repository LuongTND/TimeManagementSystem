// Call the dataTables jQuery plugin
$(document).ready(function() {
    $('#dataTable').DataTable({
        lengthMenu: [
            [10, 15, 20, -1],
            [10, 15, 20, 'All'],
        ],
        order: []
    });
});
