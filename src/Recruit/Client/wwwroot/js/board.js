export function ApplySortable(instance, methodName) {
    $(document).ready(function () {
        var data = [];
        var newStageId = null;

        $(".sortable").sortable({
            connectWith: ".connectedSortable",
            delay: 50,
            receive: function (event, ui) {
                $(ui.item).attr('data-position', 0);
                newStageId = parseInt(this.id);
            },
            update: function (event, ui) {
                data = [];
                $(this).children().each(function (index) {
                    if ($(this).attr("data-position") != (index + 1)) {
                        // VERY IMPORTANT: DO NOT enable this. Let Blazor to render the data-position attribute.
                        // $(this).attr("data-position", (index + 1));

                        var applicantId = parseInt($(this).attr("id"));
                        data.push({ applicantId, position: (index + 1) });
                    }
                });
            },
            stop: function (event, ui) {
                // VERY IMPORTANT: This will let blazor to do the ordering/re-rendering.
                // Otherwise, there will be conflict between jQuery UI and Blazor while trying to update DOM.
                $(this).sortable("cancel");

                // Check whether is the same board or new
                newStageId = newStageId == null ? parseInt(this.id) : newStageId;
                if (newStageId != null) {
                    instance.invokeMethodAsync(methodName, newStageId, data);
                    data = [];
                    newStageId = null;
                }
            }
        }).disableSelection();
    });
}