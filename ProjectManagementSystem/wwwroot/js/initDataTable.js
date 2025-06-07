function initializeDataTables(selector) {
    $(selector).DataTable({
        responsive: true,
        language: {
            search: '',
            searchPlaceholder: "Search..."
        },
        pageLength: 10,
        lengthMenu: [[5, 10, 25, 50, 100], [5, 10, 25, 50, 100]],
        dom:
            '<"datatable-header bg-light p-2 mb-2"<"d-flex justify-content-start"f>>' +
            '<"datatable-body"rt>' +
            '<"datatable-footer d-flex justify-content-between align-items-center mt-2"' +
            '<"d-flex flex-grow-1 justify-content-start"p>' +
            '<"d-flex flex-grow-1 justify-content-end"l>' +
            '>',
        columnDefs: [
            { responsivePriority: 1, targets: 0 },
            { responsivePriority: 2, targets: 3 },
            { responsivePriority: 3, targets: 4 }
        ],
    });
}
