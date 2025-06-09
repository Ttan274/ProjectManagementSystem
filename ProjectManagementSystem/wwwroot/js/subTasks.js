const finalSubtasks = [];
var globalTaskId;
var globalDependentId;
$(document).ready(function () {
    $('.form-select').select2({
        width: '100%',
        dropdownParent: $('#taskModal')
    });

    $('#taskTabs button').on('click', function (e) {
        e.preventDefault();
        $(this).tab('show');
    });

    $('#aiSuggestedSubTaskBtn').on('click', getAiSuggestedSubTasks);
});


async function fetchAiSuggestedSubTasks(taskDescr) {
    if (!taskDescr) {
        toastr.warning("Lütfen görev açıklaması giriniz");
        return;
    }

    try {
        const response = await fetch("/SubTask/CreateByOllama", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ Description: taskDescr })
        });

        if (!response.ok) throw new Error("Network response was not ok");

        const data = await response.json();
        if (!Array.isArray(data)) throw new Error("Invalid data format");

        return data;
    } catch (error) {
        console.error("Error fetching AI suggestions:", error);
        throw error;
    }
}

async function getAiSuggestedSubTasks(taskId) {
    const desc = $("#taskDescInput").val().trim();

    if (!desc) {
        toastr.warning("Lütfen görev açıklaması giriniz");
        return;
    }

    const $aiBtn = $('#aiSuggestedSubTaskBtn');
    const $spinner = $('#spinnerSection');
    const $results = $('#subtaskResults');
    const $error = $('#aiError');

    $aiBtn.prop('disabled', true);
    $spinner.removeClass('d-none');
    $results.empty();
    $error.addClass('d-none');

    try {
        const suggestions = await fetchAiSuggestedSubTasks(desc);

        if (suggestions.length === 0) {
            $results.html('<div class="alert alert-info">Öneri bulunamadı</div>');
            return;
        }

        let html = '<h6 class="fw-semibold mb-3">AI Önerileri</h6>';
        suggestions.forEach((item, index) => {
            html += `
                <div class="card ai-suggestion-card mb-2" data-ai-suggested="false" data-title="${escapeHtml(item.title)}" data-description="${escapeHtml(item.description || '')}">
                    <div class="card-body p-0">
                        <div class="d-flex justify-content-between align-items-start">
                            <div>
                                <h6 class="card-title fw-semibold mb-1">${escapeHtml(item.title)}</h6>
                                <p class="card-text small text-muted mb-0">${escapeHtml(item.description || 'Açıklama yok')}</p>
                            </div>
                            <a class="btn btn-md btn-success" 
                                    onclick="addAiSubtaskFromCard(this, '${taskId}')">
                                <i class="bi bi-plus"></i>
                            </a>
                        </div>
                    </div>
                </div>
            `;
        });

        $results.html(html);
    } catch (error) {
        $error.removeClass('d-none');
    } finally {
        $spinner.addClass('d-none');
        $aiBtn.prop('disabled', false);
    }
}
function addAiSubtaskFromCard(button, taskId) {
    const $card = $(button).closest('.ai-suggestion-card');
    const title = $card.attr('data-title');
    const description = $card.attr('data-description');

    saveSubTask(title, description, taskId);
    addAiSubtask(title, description);

    $card.attr('data-ai-suggested', 'true');
    $card.remove();
}

function addAiSubtask(title, description) {
    if (!title) return;

    finalSubtasks.push({
        dependentId: globalDependentId,
        title: title.trim(),
        description: description ? description.trim() : '',
        aiSuggested: true
    });
    renderSubtaskList();
    toastr.success('Alt görev eklendi');
}

function addManualSubtask(taskId) {
    const title = $("#subtaskTitleInput").val().trim();
    const description = $("#subtaskDescInput").val().trim();

    if (!title) {
        toastr.warning("Lütfen başlık giriniz");
        return;
    }
    saveSubTask(title, description, taskId);
    finalSubtasks.push({ dependentId: globalDependentId, title, description, aiSuggested: false });
    $("#subtaskTitleInput, #subtaskDescInput").val('');
    renderSubtaskList();
    toastr.success('Alt görev eklendi');
}

function renderSubtaskList() {
    const $list = $('#finalSubtaskList');
    const $hiddenInputs = $('#hiddenSubtaskInputs');

    if (finalSubtasks.length === 0) {
        $list.html('<div class="alert alert-light">Henüz alt görev eklenmedi</div>');
        $hiddenInputs.empty();
        return;
    }

    let listHtml = '';
    let inputsHtml = '';

    finalSubtasks.forEach((task, index) => {
        listHtml += `
            <div class="subtask-item card mb-2" data-index="${index}" data-ai-suggested="${task.aiSuggested ? 'true' : 'false'}">
                <div class="card-body py-2 px-3 d-flex justify-content-between align-items-center gap-2">
                    <div class="flex-grow-1">
                        <div class="subtask-display ${task.isEditing ? 'd-none' : ''}">
                            <h6 class="mb-1">${escapeHtml(task.title)}</h6>
                            <p class="mb-0 text-muted small">${escapeHtml(task.description)}</p>
                        </div>
                        <div class="subtask-edit ${task.isEditing ? '' : 'd-none'}">
                            <input type="text" id="editTitleText" class="form-control border-0 shadow-none bg-transparent form-control-sm subtask-title-input" 
                                   value="${escapeHtml(task.title)}" />
                            <textarea id="editDescText" class="form-control border-0 shadow-none bg-transparent form-control-sm subtask-desc-input mt-1" rows="2">${escapeHtml(task.description)}</textarea>
                        </div>
                    </div>
                    <div class="btn-group flex-shrink-0" role="group">
                        <button class="btn btn-sm btn-primary edit-btn ${task.isEditing ? 'd-none' : ''}" title="Düzenle" type="button">
                            <i class="bi bi-pencil"></i>
                        </button>
                        <button class="btn btn-sm btn-success save-btn ${task.isEditing ? '' : 'd-none'}" title="Kaydet" type="button" onclick="updateSubTask('${task.dependentId}')">
                            <i class="bi bi-check-lg"></i>
                        </button>
                        <button class="btn btn-sm btn-danger delete-btn" title="Sil" type="button" onclick="deleteSubTask('${task.dependentId}')">
                            <i class="bi bi-trash"></i>
                        </button>
                    </div>
                </div>
            </div>
        `;

        inputsHtml += `
            <input type="hidden" name="SubTasks[${index}].Title" value="${escapeHtml(task.title)}" />
            <input type="hidden" name="SubTasks[${index}].Description" value="${escapeHtml(task.description)}" />
        `;
    });

    $list.html(listHtml);
    $hiddenInputs.html(inputsHtml);

    $('.edit-btn').off('click').on('click', function () {
        const $card = $(this).closest('.subtask-item');
        const index = Number($card.data('index'));
        finalSubtasks[index].isEditing = true;
        renderSubtaskList();
    });

    $('.save-btn').off('click').on('click', function () {
        const $card = $(this).closest('.subtask-item');
        const index = Number($card.data('index'));
        const $titleInput = $card.find('.subtask-title-input');
        const $descInput = $card.find('.subtask-desc-input');

        const newTitle = $titleInput.val().trim();
        const newDesc = $descInput.val().trim();

        if (!newTitle) {
            toastr.warning('Başlık boş olamaz');
            $titleInput.focus();
            return;
        }

        finalSubtasks[index].title = newTitle;
        finalSubtasks[index].description = newDesc;
        finalSubtasks[index].isEditing = false;

        updateHiddenInputs();
        toastr.success('Alt görev güncellendi');

        renderSubtaskList();
    });

    $('.delete-btn').off('click').on('click', function () {
        const $card = $(this).closest('.subtask-item');
        const index = Number($card.data('index'));
        const removed = finalSubtasks.splice(index, 1)[0];
        renderSubtaskList();

        if (removed.aiSuggested) {
            const $results = $('#subtaskResults');
            if ($results.find('h6').length === 0) {
                $results.html('<h6 class="fw-semibold mb-3">AI Önerileri</h6>');
            }
            $results.append(`
                <div class="card ai-suggestion-card mb-2" data-ai-suggested="false" data-title="${escapeHtml(removed.title)}" data-description="${escapeHtml(removed.description)}">
                    <div class="card-body p-0">
                        <div class="d-flex justify-content-between align-items-start">
                            <div>
                                <h6 class="card-title fw-semibold mb-1">${escapeHtml(removed.title)}</h6>
                                <p class="card-text small text-muted mb-0">${escapeHtml(removed.description || 'Açıklama yok')}</p>
                            </div>
                            <a class="btn btn-md btn-success" onclick="addAiSubtaskFromCard(this, '${globalTaskId}')"> 
                                <i class="bi bi-plus"></i>
                            </a>
                        </div>
                    </div>
                </div>
            `);
        }
        toastr.info('Alt görev kaldırıldı');
        const $results = $('#subtaskResults');
        const aiSuggestionCount = $results.find('.ai-suggestion-card').length;
        if (aiSuggestionCount === 0) {
            $results.html('<div class="alert alert-light">Tüm AI önerileri seçildi, başka öneri kalmadı. :)</div>');
        }
    });
}


function updateHiddenInputs() {
    let inputsHtml = '';
    finalSubtasks.forEach((task, index) => {
        inputsHtml += `
            <input type="hidden" name="SubTasks[${index}].Title" value="${escapeHtml(task.title)}" />
            <input type="hidden" name="SubTasks[${index}].Description" value="${escapeHtml(task.description)}" />
        `;
    });
    $('#hiddenSubtaskInputs').html(inputsHtml);
}
function removeSubtask(index) {
    if (index >= 0 && index < finalSubtasks.length) {
        const removed = finalSubtasks.splice(index, 1)[0];
        renderSubtaskList();

        if (removed.aiSuggested) {
            const $results = $('#subtaskResults');
            if ($results.find('h6').length === 0) {
                $results.html('<h6 class="fw-semibold mb-3">AI Önerileri</h6>');
            }
            $results.append(`
                <div class="card ai-suggestion-card mb-2" data-ai-suggested="false" data-title="${escapeHtml(removed.title)}" data-description="${escapeHtml(removed.description)}">
                    <div class="card-body p-0">
                        <div class="d-flex justify-content-between align-items-start">
                            <div>
                                <h6 class="card-title fw-semibold mb-1">${escapeHtml(removed.title)}</h6>
                                <p class="card-text small text-muted mb-0">${escapeHtml(removed.description || 'Açıklama yok')}</p>
                            </div>
                            <a class="btn btn-md btn-success" onclick="addAiSubtaskFromCard(this, '${globalTaskId}')">
                                <i class="bi bi-plus"></i>
                            </a>
                        </div>
                    </div>
                </div>
            `);
        }
        toastr.info('Alt görev kaldırıldı');
        const $results = $('#subtaskResults');
        const aiSuggestionCount = $results.find('.ai-suggestion-card').length;
        if (aiSuggestionCount === 0) {
            $results.html('<div class="alert alert-light">Tüm AI önerileri seçildi, başka öneri kalmadı. :)</div>');
        }
    }
}

function getTaskFormAsJson() {
    const taskData = {
        ProjectId: $('#Project_Id').val(),
        TaskName: $('#TaskToCreate_TaskName').val(),
        TaskDesc: $('#taskDescInput').val(),
        TaskEffort: $('#TaskToCreate_TaskEffort').val(),
        Priority: $('#TaskToCreate_Priority').val(),
        SprintId: $('#TaskToCreate_SprintId').val(),
        UserId: $('#TaskToCreate_UserId').val(),
        SubTasks: []
    };

    finalSubtasks.forEach((subtask, index) => {
        taskData.SubTasks.push({
            Title: subtask.title,
            Description: subtask.description,
            IsAISuggested: subtask.aiSuggested || false
        });
    });

    return taskData;
}

function saveSubTask(title, desc, taskId) { /*userId, sprintId*/
    $.ajax({
        url: '/SubTask/Create',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            title: title,
            description: desc,
            //userId: userId,
            //sprintId: sprintId,
            parentTaskId: taskId
        }),
        success: function (result) {
            globalDependentId = result.depId;
        },
        error: function () {
            return;
        }
    });
}

function deleteSubTask(taskId) {
    $.ajax({
        url: '/SubTask/Delete',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            Id: taskId
        }),
        success: function (result) {
            return;
        },
        error: function () {
            return;
        }
    });
}

function updateSubTask(taskId) {
    const title = $("#editTitleText").val().trim();
    const desc = $("#editDescText").val().trim();

    $.ajax({
        url: '/SubTask/Update',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            title: title,
            description: desc,
            taskId: taskId
        }),
        success: function (result) {
            return;
        },
        error: function () {
            return;
        }
    });
}