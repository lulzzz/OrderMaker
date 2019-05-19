
(() => {

    const dialog = new mdc.dialog.MDCDialog(document.getElementById('dialog-users-delete'));
    document.getElementById('users-open-dialog').addEventListener('click', () => {
        dialog.open();
    });

    const selectPart = new mdc.select.MDCSelect(document.getElementById("users-edit-role"));

})();