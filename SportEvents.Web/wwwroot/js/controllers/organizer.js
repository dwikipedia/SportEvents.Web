var table = $('#dataTable').DataTable({
    processing: true,
    serverSide: true,
    searching: true,
    paging:false,
    dom: 'ftrip',
    ajax: {
        url: '/Organizer/GetAll',
        type: 'GET',
        data: function (d) {
            var page = parseInt($('#txtPages').val(), 10) || 1;
            var perPage = parseInt($('#txtPerPage').val(), 10) || d.length;

            return {
                draw: d.draw,
                start: (page - 1) * perPage,
                length: perPage,
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

let currentUrl = 'https://api-sport-events.test.voxteneo.com/api/v1/organizers?page=1';
function fetchData(url) {
    $.ajax({
        url: url,
        type: 'GET',
        success: function (response) {
            // Update the table with new data
            const table = $('#dataTable').DataTable();
            table.clear();
            table.rows.add(response.data);
            table.draw();

            // Update currentUrl
            currentUrl = url;

            // Update button states or URLs
            $('#prevBtn').data('url', response.meta.pagination.links.previous);
            $('#nextBtn').data('url', response.meta.pagination.links.next);
        },
        error: function (xhr, status, error) {
            console.error('Error fetching data:', error);
        }
    });
}

function debounce(fn, delay) {
    var timer = null;
    return function () {
        clearTimeout(timer);
        timer = setTimeout(() => fn.apply(this, arguments), delay);
    };
}

var reloadTable = debounce(function () {
    table.ajax.reload(null, false); 
}, 500);

$('#txtPages, #txtPerPage').on('input', reloadTable);

fetchData(currentUrl);

// Event handlers
$('#prevBtn').on('click', function () {
    const url = $(this).data('url');
    if (url) {
        fetchData(url);
    }
});

$('#nextBtn').on('click', function () {
    const url = $(this).data('url');
    if (url) {
        fetchData(url);
    }
});