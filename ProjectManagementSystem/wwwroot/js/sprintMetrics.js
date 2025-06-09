function displaySprintSelects(targetSelectSelector, projectId) {
    $.ajax({
        url: '/SprintMetrics/GetSprintSelects',
        method: 'GET',
        dataType: 'json',
        data: { projectId: projectId },
        success: function (response) {
            if (!response.success) {
                toastr.error(response.errorMessage || "Sprints not found", "Error");
                return;
            }

            var sprintSelects = response.data;

            if (!sprintSelects || sprintSelects.length === 0) {
                toastr.warning("Sprints not found", "Warning");
                return;
            }

            var $select = $(targetSelectSelector);
            $select.empty();

            $select.append($('<option>', {
                value: '',
                text: 'Select sprint'
            }));

            $.each(sprintSelects, function (index, item) {
                const optionText = item.isCurrent
                    ? `(Current) ${item.name}`
                    : item.name;

                $select.append($('<option>', {
                    value: item.id,
                    text: optionText
                }));
            });

            const currentSprint = sprintSelects.find(s => s.isCurrent);
            if (currentSprint) {
                $select.val(currentSprint.id).trigger('change');
            } else {
                $select.val(sprintSelects[0].id).trigger('change');
            }
        },
        error: function (xhr) {
            const responseJson = xhr.responseJSON;
            const errorMessage = responseJson?.errorMessage || "Error occurred while fetching sprints";
            toastr.error(errorMessage, "Error");
        }
    });
}
