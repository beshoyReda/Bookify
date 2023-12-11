var updatedRow; 
function showSuccessMessage(message = 'Saved Successfully!') {
    Swal.fire({
        icon: "success",
        title: "Success",
        text: message,
        customClass: {
            confirmButton: "btn btn-primary"
        }
    });
}
function showErrorMessage(message = 'Something went wrong!') {
    Swal.fire({
        icon: "error",
        title: "Oops...",
        text: message,
        customClass: {
            confirmButton: "btn btn-primary"
        }
    });
}
function onModalSuccess(item) {
    showSuccessMessage();
    $('#Modal').modal('hide');
    //el hta deh 3shan append ll html view f el a5r b3d el on modal success //dh fel create
    //$('tbody').append(item);
    //dh llupdate w create 
    if (updatedRow === undefined) {
        $('tbody').append(item);
    }
    else {
        $(updatedRow).replaceWith(item);
        //lazm arg3 undifined tany 3shan myrg3 ynfz el replace 
        updatedRow = undefined;
    }
    //3shan el dropdown bta3t el theme tsht8l 
    KTMenu.init();
    KTMenu.initHandlers();
}

$(document).ready(function () {
    var message = $('#Message').text();
    if (message !== '') {
        showSuccessMessage(message);

    }

    //Handle BootStrap Modal
    //b m3mlomt el body ebd2 dwr 3la el hagat ely et3mlha render aw lsa hyt3mlha 3n tare2 el el class dh(.js-render-modal)
    $('body').delegate('.js-render-modal','click', function () {
        var btn = $(this);
        var modal = $('#Modal');
        
        modal.find('#ModalLabel').text(btn.data('title'));

        if (btn.data('update') !== undefined) {
            updatedRow = btn.parents('tr');
        }

        $.get({
            url: btn.data('url'),
            success: function (form) {
                modal.find('.modal-body').html(form);
                $.validator.unobtrusive.parse(modal);
            },
            error: function () {
                showErrorMessage();
            }
        });

        modal.modal('show');
    }); 
});