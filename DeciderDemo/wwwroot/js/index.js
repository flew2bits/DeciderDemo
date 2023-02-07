$(() => {
    $('#AddWorkshopToConferenceModal').on('show.bs.modal', evt => {
        let $related = $(evt.relatedTarget);
        let $tr = $related.closest('tr');
        let conferenceId = $tr.data('conferenceId');
        $('#workshopConferenceId').val(conferenceId);
    })

    $('#WorkshopList').on('show.bs.modal', evt => {
        let $related = $(evt.relatedTarget);
        let $tr = $related.closest('tr');
        let conferenceId = $tr.data('conferenceId');
        $.ajax({
            type: 'GET',
            url: '/Workshops/' + conferenceId,
        }).then(data => {
            $('#WorkshopListItems')
                .html(data.map(d => `<li class="my-1">${d.id}: ${d.name} @@ ${d.start}-${d.end} on ${d.date} <button class="btn btn-danger btn-sm" type="submit" formaction="/Workshops/${conferenceId}/remove/${d.id}">Remove</button></li>`).join(""));
        })
    });

    $('#ApproveParticipantModal').on('show.bs.modal', evt => {
        let $related = $(evt.relatedTarget);
        let $tr = $related.closest('tr');
        let userName = $tr.data('userName');
        $('#ApproveParticipantUserName').val(userName);
    })

    $('#ConfirmRemoveParticipantModal').on('show.bs.modal', evt => {
        let $related = $(evt.relatedTarget);
        let $tr = $related.closest('tr');
        let userName = $tr.data('userName');
        $('#ConfirmRemoveParticipantUserName').val(userName);
    })

});