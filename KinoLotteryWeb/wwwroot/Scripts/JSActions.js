function showToast(message) {
    var toast = $('<div class="toast"><span class="message">' + message + '</span><span class="close">&times;</span></div>');
    toast.hide().appendTo('#toast-container').fadeIn();

    // Close button
    toast.find('.close').click(function () {
        toast.fadeOut(function () {
            $(this).remove();
        });
    });

    // Auto-hide after 3 seconds
    setTimeout(function () {
        toast.fadeOut(function () {
            $(this).remove();
        });
    }, 4000);
}
