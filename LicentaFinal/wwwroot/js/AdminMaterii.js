function confirmDeleteMaterie(uniqueId, isConfirmClicked) {
    var removeSpan = 'removeSpan_' + uniqueId;
    var confirmRemove = 'confirmRemoveSpan_' + uniqueId;

    if (isConfirmClicked) {
        $('#' + removeSpan).hide();
        $('#' + confirmRemove).show();
    } else {
        $('#' + removeSpan).show();
        $('#' + confirmRemove).hide();
    }
}