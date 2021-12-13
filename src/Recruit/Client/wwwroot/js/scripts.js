var _instance = null;
var _methodName = null;

var HandleOutsideDivClick = function (event) {
    var div = document.querySelector(".applicant-details.show");
    if (div) {
        if (event.target.closest(".applicant-details.show") ||
            event.target.closest(".close-button") ||
            event.target.closest("a")) {
            return;
        }

        _instance.invokeMethodAsync(_methodName);
    }
};

export function AddOutsideDivClickListener(instance, methodName) {
    _instance = instance;
    _methodName = methodName;
    document.addEventListener("click", HandleOutsideDivClick);
}

export function RemoveOutsideDivClickListener() {
    document.removeEventListener("click", HandleOutsideDivClick);
}