function confirmAllow(uniqueId, isAllowClicked) {
    var allowSpan = 'AllowSpan_' + uniqueId;
    var confirmAllowSpan = 'confirmAllowSpan_' + uniqueId;

    if (isAllowClicked) {
        $('#' + allowSpan).hide();
        $('#' + confirmAllowSpan).show();
    } else {
        $('#' + allowSpan).show();
        $('#' + confirmAllowSpan).hide();
    }
}
function confirmDeny(uniqueId, isDenyClicked) {
    var denySpan = 'DenySpan_' + uniqueId;
    var confirmDenySpan = 'confirmDenySpan_' + uniqueId;

    if (isDenyClicked) {
        $('#' + denySpan).hide();
        $('#' + confirmDenySpan).show();
    } else {
        $('#' + denySpan).show();
        $('#' + confirmDenySpan).hide();
    }
}