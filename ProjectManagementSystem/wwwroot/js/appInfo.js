function submitAppInfoForm() {
    const formData = {
        Name: $('#Name').val(),
        Description: $('#Description').val(),
        AppCode: parseInt($('#AppCode').val()) || 0,
        GitHubPatToken: $('#GitHubPatToken').val(),
        GitHubOwner: $('#GitHubOwner').val(),
        GitHubRepo: $('#GitHubRepo').val(),
        DecommissionDate: $('#DecommissionDate').val() || null,
        ProjectId: $('#ProjectId').val() || null
    };

    //const isCreate = !formData.ProjectId || formData.ProjectId === "00000000-0000-0000-0000-000000000000";

    const url = '/AppInfo/Action';

    $.ajax({
        url: url,
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(formData),
        success: function (response) {
            toastr.success(response.message || 'İşlem başarıyla tamamlandı.');
            setTimeout(() => location.reload(), 1000);
        },
        error: function (xhr) {
            const errorMsg = xhr.responseJSON?.message || 'Bir hata oluştu.';
            toastr.error(errorMsg);
        }
    });
}

function openAppInfoModal(projectId) {
    $.ajax({
        url: '/AppInfo/Action',
        type: 'GET',
        data: { ProjectId: projectId },
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