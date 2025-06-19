function submitAppInfoForm() {
    const $formContainer = $("#appInfoForm");

    const formData = {
        Id: $formContainer.find('#Id').val(),
        Name: $formContainer.find('#Name').val(),
        Description: $formContainer.find('#Description').val(),
        AppCode: parseInt($formContainer.find('#AppCode').val()),
        GitHubPatToken: $formContainer.find('#GitHubPatToken').val(),
        GitHubOwner: $formContainer.find('#GitHubOwner').val(),
        GitHubRepo: $formContainer.find('#GitHubRepo').val(),
        DecommissionDate: $formContainer.find('#DecommissionDate').val(),
        ProjectId: $formContainer.find('#ProjectId').val()
    };

    const isCreate = !formData.Id || formData.Id === "00000000-0000-0000-0000-000000000000";
    const url = '/AppInfo/Action';

    const token = $('input[name="__RequestVerificationToken"]').val();

    $.ajax({
        url: url,
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(formData),
        headers: {
            'RequestVerificationToken': token
        },
        success: function (response) {
            if (response.success) {
                toastr.success(response.message || 'İşlem başarıyla tamamlandı.');

                if (isCreate) {
                    setTimeout(() => location.reload(), 1000);
                } else {
                    const modal = bootstrap.Modal.getInstance(document.getElementById('appInfoModal'));
                    modal.hide();
                }
            }
            else {
                toastr.error(response.errorMessage, 'Error');
            }
        },
        error: function (xhr) {
            if (xhr == null) {
                toastr.error("Operation failed.", "Error");
            }
            else {
                const responseJson = xhr.responseJSON;

                toastr.error(responseJson.errorMessage, "Error");
            }
        }
    });
}

function openAppInfoModal(queryParams) {
    $.ajax({
        url: '/AppInfo/Action',
        type: 'GET',
        data: queryParams,
        contentType: 'application/json',
        success: function (html) {
            $('#appInfoModalBody').html(html);
            var modal = new bootstrap.Modal(document.getElementById('appInfoModal'));
            modal.show();
        },
        error: function () {
            toastr.error("Form yüklenemedi.");
        }
    });
}

function displayAppInfo(appInfoId, projectId) {
    const requestParams = { Id: appInfoId, ProjectId: projectId };

    openAppInfoModal(requestParams);
}

function deleteAppInfo(id) {
    $('#confirmDeleteBtn').attr('onclick', `confirmDelete('${id}')`);
    $('#deleteAppModal').modal('show');
}

function displayProjectDependencies(appId) {
    $('#appInfoModalBody').html(`
        <div class="text-center py-4">
            <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">Loading...</span>
            </div>
            <p class="mt-2">Dependencies loading...</p>
        </div>
    `);

    $('#appInfoModalLabel').text('Project Dependencies');

    var modal = new bootstrap.Modal(document.getElementById('appInfoModal'));
    modal.show();

    $.ajax({
        url: "/GitDependency/GetAppDependencies?appId=" + appId,
        type: 'GET',
        success: function (data) {
            $('#appInfoModalBody').html(data);
        },
        error: function (xhr, status, error) {
            $('#appInfoModalBody').html(`
                <div class="alert alert-danger">
                    <i class="bi bi-exclamation-triangle-fill"></i> 
                    Error loading dependencies: ${xhr.responseText || 'Unknown error'}
                </div>
            `);
        }
    });
}

function confirmDelete(id) {
    $('#deleteAppModal').modal('hide');

    const token = $('input[name="__RequestVerificationToken"]').val();

    $.ajax({
        url: '/AppInfo/Delete',
        type: 'POST',
        data: JSON.stringify({ Id: id }),
        contentType: 'application/json',
        headers: {
            'RequestVerificationToken': token
        },
        success: function (response) {
            if (response.success) {
                toastr.success('Application deleted successfully!', 'Success');
                setTimeout(() => location.reload(), 1000);
            }
            else {
                toastr.error(response.errorMessage, 'Error');
            }
        },
        error: function (xhr) {
            if (xhr == null) {
                toastr.error("Operation failed.", "Error");
            }
            else {
                const responseJson = xhr.responseJSON;

                toastr.error(responseJson.errorMessage, "Error");
            }
        }
    });
}