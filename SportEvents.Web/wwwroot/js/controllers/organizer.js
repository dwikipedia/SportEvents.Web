var table = $('#dataTable').DataTable({
    processing: true,
    serverSide: true,
    searching: true,
    dom: 'ftrip',
    ajax: {
        url: '/Organizer/GetAll',
        type: 'GET',
        data: function (d) {
            return {
                draw: d.draw,
                start: $('#txtPages').val(),
                length: $('#txtPerPage').val(),
                searchValue: d.search.value,
                orderColumn: d.order[0].column,
                orderDir: d.order[0].dir
            };
        },
        error: function (xhr, error, thrown) {
            console.error('DataTables AJAX error:', xhr.responseText);
        },
        dataSrc: 'data'
    },
    columns: [
        { data: 'id' },
        { data: 'organizerName' },
        {
            data: 'imageLocation',
            render: src => `<img src="${src}" style="max-height:50px;" />`
        }
    ],
    pageLength: 10,
    lengthMenu: [[5, 10, 25, -1],
    [5, 10, 25, "All"]],
    pagingType: 'simple_numbers'
});

// 2) Debounce helper: returns a debounced version of fn
function debounce(fn, delay) {
    var timer = null;
    return function () {
        clearTimeout(timer);
        timer = setTimeout(() => fn.apply(this, arguments), delay);
    };
}

// 3) On input change (with debounce), reload table
var reloadTable = debounce(function () {
    table.ajax.reload(null, false);  // false = do not reset paging :contentReference[oaicite:6]{index=6}
}, 500);                          // 500 ms pause after typing :contentReference[oaicite:7]{index=7}

$('#txtPages, #txtPerPage').on('input', reloadTable);