var lastClickedBtn;

$(document).ready(function () {

    window.setTimeout(function () {
        $('.alert-success')
            .fadeTo(500, 0)
            .slideUp(500, function () {
                $(this).remove();
            });
    }, 3000);

    // Check for success message
    var successMessage = '@(TempData["SuccessMessage"] ?? "")';

    if (successMessage) {
        // Set message content
        $('#successMessage').text(successMessage);

        // Show modal
        $('#successModal').modal('show');

        setTimeout(function () {
            $('#successModal').modal('hide');
        }, 3000);
    }

    $('.confirm').on('click', function () {
        lastClickedBtn = $(this);                // Remember which button was clicked
        $('#confirmationMessage').text($(this).data('message'));
        $('#confirmationModal').modal('show');
    });

    $('#confirmButton').on('click', function () {
        const $form = $('#mainForm'),
            actionUrl = lastClickedBtn.attr('formaction'),
            methodType = lastClickedBtn.attr('formmethod')
                || $form.attr('method');

        $form.attr('action', actionUrl)
            .attr('method', methodType)
            .submit();
    });

});



