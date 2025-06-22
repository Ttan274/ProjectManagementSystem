function initializeDataTables(selector) {
    $(selector).DataTable({
        responsive: true,
        language: {
            search: '',
            searchPlaceholder: "Search...",
            lengthMenu: "Show _MENU_ entries",
            info: "Showing _START_ to _END_ of _TOTAL_ entries",
            paginate: {
                first: "First",
                last: "Last",
                next: "Next",
                previous: "Previous"
            }
        },
        pageLength: 10,
        lengthMenu: [[5, 10, 25, 50, 100], [5, 10, 25, 50, 100]],
        dom:
            '<"datatable-header p-2 mb-2"<"d-flex justify-content-start"f>>' +
            '<"datatable-body"rt>' +
            '<"datatable-footer d-flex justify-content-between align-items-center mt-2"' +
            '<"d-flex flex-grow-1 justify-content-start"p>' +
            '<"d-flex flex-grow-1 justify-content-end"l>' +
            '>',
        initComplete: function () {
            $(selector + '_wrapper .datatable-header').css({
                'background-color': '#f0f7ff',
                'margin-bottom': '10px'
            });

            // Search input styling
            $(selector + '_wrapper .dataTables_filter input').css({
                'background-color': '#f8fbff',
                'color': '#000000',
                'padding': '6px 12px',
                'width': '100%',
                'max-width': '300px'
            });

            $(selector + '_wrapper .datatable-footer').css({
                'padding-top': '10px'
            });

            $(selector + ' thead th').css({
                'background-color': '#ffffff',
                'color': '#000000',
                'border-bottom': '2px solid #4a90e2',
                'border-right': '1px solid #e0e0e0',
                'padding': '10px 15px',
                'font-weight': '600'
            });

            $(selector + ' thead th:last-child').css('border-right', 'none');
        },
        createdRow: function (row, data, dataIndex) {
            $(row).css({
                'background-color': dataIndex % 2 === 0 ? '#ffffff' : '#e8f2fc',
                'border-bottom': '1px solid #e0e0e0'
            });

            $(row).find('td').css({
                'border-right': '1px solid #e0e0e0',
                'padding': '8px 15px'
            });

            $(row).find('td:last-child').css('border-right', 'none');
        },
        columnDefs: [
            { responsivePriority: 1, targets: 0 },
            { responsivePriority: 2, targets: 3 },
            { responsivePriority: 3, targets: 4 }
        ],
        drawCallback: function () {
            $(selector + ' tbody tr').each(function (index) {
                $(this).css('background-color', index % 2 === 0 ? '#ffffff' : '#e8f2fc');
            });
        }
    });
}