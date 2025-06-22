let performanceValues = {};
let codingValues = {};


function openProjectConfigModal(showConfigAlert = false) {
    if (!projectId) {
        toastr.warning("Project not found.", "Warning");
        return;
    }

    if (showConfigAlert) {
        $("#projectTeamconfigAlert").removeClass("d-none");
    } else {
        $("#projectTeamconfigAlert").addClass("d-none");
    }

    $.ajax({
        url: `/ProjectTeamConfig/Action?projectId=${projectId}`,
        type: 'GET',
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        beforeSend: function () {
            $('#projectTeamConfigModal .modal-body').html(`
                <div class="text-center my-4">
                    <div class="spinner-border text-danger" role="status">
                        <span class="visually-hidden">Loading...</span>
                    </div>
                    <p class="mt-3 text-muted">Loading configuration...</p>
                </div>
            `);
            $('#projectTeamConfigModal').modal('show');
        },
        success: function (html) {
            $('#projectTeamConfigModal .modal-body').html(html);
        },
        error: function (xhr) {
            const message = xhr.responseText || "An unexpected error occurred.";
            $('#projectTeamConfigModal .modal-body').html(`
                <div class="alert alert-danger text-start" role="alert">
                    <i class="bi bi-exclamation-circle me-2"></i>
                    ${message}
                </div>
            `);
        }
    });
}

const performanceSliders = {
    taskCompletion: { input: "taskCompletionWeight", label: "taskCompletionValue" },
    onTimeDelivery: { input: "onTimeDeliveryWeight", label: "onTimeDeliveryValue" },
    targetProximity: { input: "targetProximityWeight", label: "targetProximityValue" },
    codingScore: { input: "codingScoreWeight", label: "codingScoreValue" },
};

const codingSliders = {
    commits: { input: "commitWeight", label: "commitWeightValue" },
    netChange: { input: "netChangeWeight", label: "netChangeWeightValue" },
    refactor: { input: "refactorWeight", label: "refactorWeightValue" },
};

function autoAdjustSliders(changedKey, sliders, values, updateUI, total = 100) {
    const keys = Object.keys(values);
    const changedIndex = keys.indexOf(changedKey);
    const otherKeys = keys.filter((_, idx) => idx !== changedIndex);
    const changedValue = parseFloat(document.getElementById(sliders[changedKey].input).value);
    values[changedKey] = changedValue;

    let remaining = total - changedValue;
    const sumOthers = otherKeys.reduce((sum, key) => sum + values[key], 0);

    if (remaining < 0) return;

    if (sumOthers === 0) {
        const equalShare = remaining / otherKeys.length;
        otherKeys.forEach((key) => {
            values[key] = equalShare;
            document.getElementById(sliders[key].input).value = equalShare.toFixed(1);
            document.getElementById(sliders[key].label).textContent = `${equalShare.toFixed(1)}%`;
        });
    } else {
        otherKeys.forEach((key) => {
            const newVal = (values[key] / sumOthers) * remaining;
            values[key] = newVal;
            document.getElementById(sliders[key].input).value = newVal.toFixed(1);
            document.getElementById(sliders[key].label).textContent = `${newVal.toFixed(1)}%`;
        });
    }

    document.getElementById(sliders[changedKey].label).textContent = `${changedValue.toFixed(1)}%`;

    const totalSum = Object.values(values).reduce((acc, val) => acc + val, 0);
    const difference = total - totalSum;

    if (Math.abs(difference) > 0.1) {
        const lastKey = otherKeys[otherKeys.length - 1];
        values[lastKey] += difference;
        document.getElementById(sliders[lastKey].input).value = values[lastKey].toFixed(1);
        document.getElementById(sliders[lastKey].label).textContent = `${values[lastKey].toFixed(1)}%`;
    }
}
function setupSliders(sliders, values) {
    Object.keys(sliders).forEach((key) => {
        const inputElement = document.getElementById(sliders[key].input);
        inputElement.addEventListener("input", () => {
            autoAdjustSliders(key, sliders, values, () => {
                const total = Object.keys(sliders).reduce((sum, k) => {
                    const el = document.getElementById(sliders[k].input);
                    return sum + parseFloat(el.value || 0);
                }, 0);

                if (total !== 100) {
                    normalizeSliders(sliders);
                }
            });
        });
    });
}

function normalizeSliders(sliders) {
    const values = Object.keys(sliders).map((key) => {
        const el = document.getElementById(sliders[key].input);
        return {
            key: key,
            element: el,
            value: parseFloat(el.value || 0)
        };
    });

    const total = values.reduce((sum, item) => sum + item.value, 0);

    if (total === 0) return;

    values.forEach((item) => {
        const newValue = Math.round((item.value / total) * 100);
        item.element.value = newValue;
    });
}



function handleAiSupportButtonClick() {
    const teamIntroduction = $("#teamIntroduction").val().trim();

    if (!teamIntroduction) {
        toastr.warning("Please describe your team before requesting AI suggestions.");
        return;
    }

    $("#aiResponseMessage").show();
    $("#aiResponse").hide();

    $.ajax({
        url: "/ProjectTeamConfig/GetAiSuggestion",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({ TeamIntroduction: teamIntroduction }),
        success: function (result) {
            if (!result || !result.success || !result.data) {
                toastr.error(result.errorMessage || "Failed to get AI suggestions.");
                return;
            }

            const data = result.data;

            window.latestAiSuggestions = {
                taskCompletion: {
                    value: data["Task Completion"].value,
                    reason: data["Task Completion"].reason
                },
                onTimeDelivery: {
                    value: data["On-Time Delivery"].value,
                    reason: data["On-Time Delivery"].reason
                },
                targetProximity: {
                    value: data["Target Proximity"].value,
                    reason: data["Target Proximity"].reason
                },
                codingScore: {
                    value: data["Code Quality"].value,
                    reason: data["Code Quality"].reason
                }
            };

            let suggestionHtml = `<strong>AI Suggestions:</strong><ul class="mb-2">`;
            for (const [key, val] of Object.entries(window.latestAiSuggestions)) {
                const label = key
                    .replace(/([A-Z])/g, ' $1')
                    .replace(/^./, s => s.toUpperCase());
                suggestionHtml += `<li><strong>${escapeHtml(label)}:</strong> ${escapeHtml(val.value)}% – ${escapeHtml(val.reason)}</li>`;
            }
            suggestionHtml += `</ul>`;

            $("#aiSuggestionText").html(suggestionHtml);
            $("#aiResponseMessage").hide();
            $("#aiResponse").show();
            $("#applyAiSuggestion").show();

            handleApplyAiSuggestion();

            toastr.success("AI suggestions retrieved successfully.");
        },
        error: function (xhr) {
            $("#aiResponseMessage").hide();
            toastr.error("An error occurred while getting AI suggestions.");
        }
    });
}

function handleApplyAiSuggestion() {
    if (!window.latestAiSuggestions) {
        toastr.warning("No AI suggestion available to apply.");
        return;
    }

    const suggestions = window.latestAiSuggestions;

    Object.keys(suggestions).forEach(key => {
        const val = suggestions[key].value;
        const reason = suggestions[key].reason;

        document.getElementById(performanceSliders[key].input).value = val;
        document.getElementById(performanceSliders[key].label).textContent = `${val}%`;

        const reasonElement = document.getElementById(`${key}Reason`);
        if (reasonElement) {
            reasonElement.textContent = reason;
        }
    });

    document.getElementById("applyAiSuggestion").style.display = "none";

    toastr.success("AI suggestions applied successfully.");
}
function getProjectTeamConfigForm() {
    const data = {
        id: $("#projectConfigId").val(),
        projectId: $("#teamConfigProjectId").val(),
        teamIntroduction: $("#teamIntroduction").val().trim(),
        taskCompletionWeight: parseFloat($("#taskCompletionWeight").val()) / 100,
        onTimeDeliveryWeight: parseFloat($("#onTimeDeliveryWeight").val()) / 100,
        targetProximityWeight: parseFloat($("#targetProximityWeight").val()) / 100,
        codingScoreWeight: parseFloat($("#codingScoreWeight").val()) / 100,
        commitWeight: parseFloat($("#commitWeight").val()) / 100,
        netChangeWeight: parseFloat($("#netChangeWeight").val()) / 100,
        refactorWeight: parseFloat($("#refactorWeight").val()) / 100
    };

    return JSON.stringify(data);
}

function callProjectConfigChangePreCallback() {
    $("#performanceTabs .nav-link").removeClass("active");
    $("#performanceTabsContent .tab-pane").removeClass("show active");

    $("#empty-tab").addClass("active");
    $("#empty").addClass("show active");
}


function callProjectConfigChangePostCallback() {
    const lastTabId = localStorage.getItem("lastSelectedTabId") || "overview-tab";
    const targetTab = $(`#${lastTabId}`);
    const targetPaneId = targetTab.attr("data-bs-target");

    targetTab.addClass("active");
    $(targetPaneId).addClass("show active");
}


function createProjectConfig() {
    const configData = getProjectTeamConfigForm();

    $.ajax({
        url: "/ProjectTeamConfig/Create",
        type: "POST",
        contentType: "application/json",
        headers: {
            "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
        },
        data: configData,
        success: function (response) {
            if (!response.success) {
                toastr.error(response.errorMessage || "Something went wrong", "Error");
                return;
            }

            toastr.success("Project configuration created successfully.", "Success");

            setTimeout(() => {
                if (hasProjectConfigured === 0 && projectId) {
                    window.location.href = `/ProjectPerformance/Index?projectId=${projectId}`;
                } else {
                    location.reload();
                }
            }, 1000);
        },
        error: function (xhr) {
            const errorMsg = xhr.responseJSON?.errorMessage || "Failed to create configuration.";
            toastr.error(errorMsg, "Error");
        }
    });
}

function updateProjectConfig() {
    const configData = getProjectTeamConfigForm();

    $.ajax({
        url: "/ProjectTeamConfig/Update",
        type: "POST",
        contentType: "application/json",
        headers: {
            "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
        },
        data: configData,
        success: function (response) {

            toastr.success("Project configuration updated successfully.", "Success");

            setTimeout(() => {
                location.reload();
            }, 1000);
        },
        error: function (xhr) {
            const errorMsg = xhr.responseJSON?.errorMessage || "Failed to update configuration.";
            toastr.error(errorMsg, "Error");
        }
    });
}
