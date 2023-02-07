$(() => {
    $('#ReserveSeatModal').on('show.bs.modal', evt => {
        let $related = $(evt.relatedTarget);
        let $tr = $related.closest('tr');
        let name = $tr.data('workshopName');
        let id = $tr.data('workshopId');
        $('#ReserveSeatWorkshopTitle').text(name);
        $('#ReserveSeatWorkshopId').val(id);
    })
})