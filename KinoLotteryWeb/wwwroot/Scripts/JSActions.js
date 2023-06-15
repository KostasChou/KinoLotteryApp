function showToast(message) {
    debugger;
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

async function logOutFunction(){
    debugger;

    const response = await fetch('/api/account/Logout', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        }
    });

    window.location = "/Login.html";
    
}
