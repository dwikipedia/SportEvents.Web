const url = '/Organizer/GetAll'

var table = $('#dataTable').DataTable({
    processing: true,
    serverSide: true,
    searching: true,
    ajax: url,
    columns: [
        { data: 'id' },
        { data: 'organizerName' },
        {
            data: 'imageLocation',
            render: src => `<img src="${src}" style="max-height:50px;" />`
        }
    ],
    pageLength: 10,
    lengthMenu: [[5, 10, 25, -1], [5, 10, 25, "All"]],
    pagingType: 'simple_numbers'
});
