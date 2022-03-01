export function AddOutsideDivClickListener(instance, methodName) {
    $("body").click(function (event) {
        var div = document.querySelectorAll(".applicant-details.show");
        if (div) {
            if (event.target.id != "applicant-details" && $(event.target).parents("#applicant-details").length == 0) {
                instance.invokeMethodAsync(methodName);
            }
        }
    });
}

export function RemoveOutsideDivClickListener() {
    $("body").off("click");
}