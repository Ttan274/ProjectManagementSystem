let appSelector;
const chartInstances = {};

function fetchGitCommitStats(appId) {
    $.ajax({
        url: '/GithubAnalytics/GetCommitStats',
        method: 'GET',
        data: { appId: appId },
        success: function (data) {
            updateGitCommitAnalysisUI(data);
        },
        error: function (xhr, status, error) {
            $('#total-commits').text('Error');
            $('#avg-commit-size').text('Error');
            $('#feature-commits').text('Error');
            $('#refactor-commits').text('Error');
            $('#net-growth').text('Error');
            $('#net-growth-bar').css('width', '50%').removeClass('bg-success bg-danger');
            $('#top-committer').text('Error');
            $('#busy-period-info').text('Error');
            $('#high-risk-commits').text('Error');
            $('#single-author-files').text('Error');
            $('#deadline-rushes').text('Error');
            $('#risk-files-info').html('<strong>Risky files:</strong> Error');

            const responseJson = xhr.responseJSON;

            toastr.error(responseJson.errorMessage || 'Error occurred while fetching commit statuses', "Error");
        }
    });
}

function fetchAppInfoDropdown() {
    $.ajax({
        url: '/AppInfo/GetAppInfoSelects',
        method: 'GET',
        success: function (response) {
            if (!response.success) {
                toastr.error(response.errorMessage || "Error occurred while fetching application info", "Error");
                return;
            }
            let data = response.data;

            appSelector.empty();
            data.forEach(function (app, index) {
                appSelector.append(`<option value="${app.id}">${app.name}</option>`);
            });

            if (data.length > 0) {
                fetchGitCommitStats(data[0].id);
            }
        },
        error: function (xhr, status, error) {
            const responseJson = xhr.responseJSON;

            toastr.error(responseJson.errorMessage || 'Error occurred while fetching the application list', "Error");
        }
    });
}

function updateGitCommitAnalysisUI(data) {
    $('#commit-table').DataTable().clear().draw();
    $('#total-commits').text('--');
    $('#avg-commit-size').text('--');
    $('#feature-commits').text('--');
    $('#refactor-commits').text('--');
    $('#net-growth').text('--');
    $('#net-growth-bar').css('width', '50%').removeClass('bg-success bg-danger');
    $('#top-committer').text('--');
    $('#busy-period-info').text('');
    $('#high-risk-commits').text('--');
    $('#single-author-files').text('--');
    $('#deadline-rushes').text('--');
    $('#risk-files-info').html('<strong>Risky files:</strong> Loading...');

    try {
        $('#total-commits').text(data.basicMetrics.totalCommits || '--');
        $('#avg-commit-size').text((data.basicMetrics.avgCommitSize || 0).toFixed(0) + ' lines');
        $('#feature-commits').text(data.codeQuality.featureCommits || '--');
        $('#refactor-commits').text(data.codeQuality.refactoringCommits || '--');

        const netGrowth = data.projectHealth.netGrowth || 0;
        $('#net-growth').text(netGrowth > 0 ? `+${netGrowth}` : netGrowth);
        const growthPercent = Math.min(Math.abs(netGrowth) / 1000 * 100, 100);
        $('#net-growth-bar')
            .css('width', `${growthPercent}%`)
            .toggleClass('bg-success', netGrowth > 0)
            .toggleClass('bg-danger', netGrowth < 0);

        const committerLabels = Object.keys(data.teamPerformance.commitsByCommitter);
        const committerData = Object.values(data.teamPerformance.commitsByCommitter);

        const topCommitterIndex = committerData.indexOf(Math.max(...committerData));

        const topCommitter = committerLabels[topCommitterIndex];

        $('#top-committer').text(topCommitter);

        renderCommitDistributionChart('team-activity-chart', committerLabels, committerData, 'Team Distribution');

        const activityLabels = Object.keys(data.projectHealth.activityTrend);
        const activityData = Object.values(data.projectHealth.activityTrend);
        renderActivityTrendChart('activity-trend-chart', activityLabels, activityData, 'Monthly Commits', '#43aa8b');

        const hours = Array.from({ length: 24 }, (_, i) => i);
        const commitsByHour = hours.map(h => data.workflow.commitsByHour[h] || 0);
        renderCommitTimeChart('commits-by-hour-chart', hours, commitsByHour, 'Commits by Hour', '#4361ee');

        const days = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
        renderCommitTimeChart('commits-by-day-chart', days, data.workflow.commitsByDay, 'Commits by Day', '#f8961e');

        const busyDays = data.workflow.busyPeriods.length;
        $('#busy-period-info').text(`${busyDays} busy days detected (5+ commits per day)`);

        updateRiskIndicators(data);

        if (data.commitStatAnalysis?.commitStats) {
            populateTopDevelopersTable(data.commitStatAnalysis.commitStats);
        }
    } catch (error) {
        toastr.error("Error updating Git commit analysis UI", "Error");
    }
}

function updateRiskIndicators(data) {
    try {
        const riskCommits = data.riskIndicators.highRiskCommits;
        const fridayCommits = data.riskIndicators.deadlineRushes || [];

        const commitData = [];

        if (riskCommits.length > 0) {
            const highRiskCommitData = riskCommits.map(commit => {
                const riskReasons = [];

                if (commit.size && commit.size > 500) {
                    riskReasons.push('Large Commit');
                }

                return {
                    commitId: commit.sha,
                    author: commit.committer,
                    date: commit.date,
                    filesChanged: commit.filesChanged.length,
                    commitMessage: commit.message,
                    isHighRisk: true,
                    riskReason: riskReasons.length > 0 ? riskReasons.join(', ') : 'No specific reason'
                };
            });

            commitData.push(...highRiskCommitData);
        }

        if (fridayCommits.length > 0) {
            const fridayCommitData = fridayCommits.map(commit => {
                return {
                    commitId: commit.sha,
                    author: commit.committer,
                    date: commit.date,
                    filesChanged: commit.filesChanged.length,
                    commitMessage: commit.message,
                    isHighRisk: false,
                    riskReason: 'Deadline Rush (Friday Commit)'
                };
            });

            commitData.push(...fridayCommitData);
        }

        updateCommitTable(commitData);

        $('#high-risk-commits').text(riskCommits.length || '--');
        $('#single-author-files').text(Object.keys(data.riskIndicators.singleAuthorFiles).length || '--');
        $('#deadline-rushes').text(fridayCommits.length || '--');

        const riskFiles = Object.entries(data.riskIndicators.singleAuthorFiles)
            .slice(0, 5)
            .map(([file, author]) =>
                `<span class="badge risk-file-badge bg-light text-dark">${file.split('/').pop()} (${author})</span>`
            ).join('');

        const moreFiles = Object.keys(data.riskIndicators.singleAuthorFiles).length > 5 ?
            ` and ${Object.keys(data.riskIndicators.singleAuthorFiles).length - 5} more files` : '';

        $('#risk-files-info').html(`<strong>Single-author files:</strong> ${riskFiles}${moreFiles}`);

        if (riskCommits.length === 0) {
            $('#risk-files-alert').hide();
        } else {
            $('#risk-files-alert').show();
        }

    } catch (error) {
        toastr.error("Error updating risk indicators", "Error");
    }
}

function updateCommitTable(commitData) {
    $('#commit-table').DataTable({
        destroy: true,
        data: commitData,
        columns: [
            { data: 'author' },
            { data: 'date' },
            { data: 'filesChanged' },
            { data: 'riskReason' },
            { data: 'commitMessage' }
        ],
        order: [[2, 'desc']],
        paging: true,
        searching: true,
        lengthChange: false,
        info: true,
        autoWidth: false
    });
}

function renderCommitDistributionChart(canvasId, labels, data, label) {
    const ctx = document.getElementById(canvasId).getContext('2d');
    if (chartInstances[canvasId]) chartInstances[canvasId].destroy();

    chartInstances[canvasId] = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: labels,
            datasets: [{
                label: label,
                data: data,
                backgroundColor: [
                    '#4361ee', '#3f37c9', '#4895ef', '#4cc9f0',
                    '#f72585', '#f8961e', '#43aa8b', '#90be6d',
                    '#577590', '#8338ec'
                ],
                borderWidth: 2,
                borderColor: '#ffffff',
                hoverOffset: 10
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                tooltip: {
                    callbacks: {
                        label: (context) => `${context.label}: ${context.parsed} commit`
                    }
                },
                legend: {
                    position: 'right',
                    labels: {
                        boxWidth: 14,
                        font: {
                            size: 12,
                            weight: 'bold'
                        }
                    }
                }
            },
            animation: {
                animateScale: true
            }
        }
    });
}

function renderActivityTrendChart(canvasId, labels, data, label, color) {
    const ctx = document.getElementById(canvasId).getContext('2d');
    if (chartInstances[canvasId]) chartInstances[canvasId].destroy();

    chartInstances[canvasId] = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: label,
                data: data,
                borderColor: color,
                backgroundColor: color + '33',
                pointBackgroundColor: color,
                pointRadius: 4,
                pointHoverRadius: 6,
                borderWidth: 2,
                tension: 0.4,
                fill: true
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                tooltip: {
                    mode: 'index',
                    intersect: false
                },
                legend: {
                    display: true,
                    labels: {
                        font: {
                            size: 12,
                            weight: 'bold'
                        }
                    }
                }
            },
            interaction: {
                mode: 'nearest',
                axis: 'x',
                intersect: false
            },
            scales: {
                y: {
                    beginAtZero: true,
                    grid: {
                        color: '#eeeeee'
                    }
                },
                x: {
                    grid: {
                        display: false
                    }
                }
            }
        }
    });
}

function renderCommitTimeChart(canvasId, labels, data, label, color) {
    const ctx = document.getElementById(canvasId).getContext('2d');
    if (chartInstances[canvasId]) chartInstances[canvasId].destroy();

    chartInstances[canvasId] = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: label,
                data: data,
                backgroundColor: color + 'aa',
                borderColor: color,
                borderWidth: 1,
                borderRadius: 6,
                barPercentage: 0.6,
                categoryPercentage: 0.7
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: true,
                    labels: {
                        font: {
                            size: 12,
                            weight: 'bold'
                        }
                    }
                },
                tooltip: {
                    callbacks: {
                        label: context => `${context.label}: ${context.parsed.y} commit`
                    }
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        precision: 0
                    },
                    grid: {
                        color: '#e0e0e0'
                    }
                },
                x: {
                    grid: {
                        display: false
                    }
                }
            }
        }
    });
}

function populateTopDevelopersTable(commitStatAnalysis) {
    const tableBody = $('#top-devs-table-body');
    tableBody.empty();

    if (!commitStatAnalysis || commitStatAnalysis.length === 0) {
        tableBody.append(`<tr><td colspan="7" class="text-center text-muted">No data</td></tr>`);
        return;
    }

    commitStatAnalysis
        .sort((a, b) => b.codingScore - a.codingScore)
        .forEach(dev => {
            const row = `
                <tr>
                    <td>${dev.username ?? '-'}</td>
                    <td>${dev.commitCount}</td>
                    <td><span class="text-success">+${dev.totalAdditions}</span></td>
                    <td><span class="text-danger">-${dev.totalDeletions}</span></td>
                    <td>${dev.estimatedRefactoredLines}</td>
                    <td>${dev.netChanges}</td>
                    <td>${dev.codingScore.toFixed(2)}</td>
                </tr>
            `;
            tableBody.append(row);
        });
}

function populateTopDevelopersTable(commitStatAnalysis) {
    const tableBody = $('#top-devs-table-body');
    tableBody.empty();

    if (!commitStatAnalysis || commitStatAnalysis.length === 0) {
        tableBody.append(`<tr><td colspan="7" class="text-center text-muted">No data</td></tr>`);
        return;
    }

    commitStatAnalysis
        .sort((a, b) => b.codingScore - a.codingScore)
        .forEach(dev => {
            const row = `
                <tr>
                    <td>${dev.username ?? '-'}</td>
                    <td>${dev.commitCount}</td>
                    <td><span class="text-success">+${dev.totalAdditions}</span></td>
                    <td><span class="text-danger">-${dev.totalDeletions}</span></td>
                    <td>${dev.estimatedRefactoredLines}</td>
                    <td>${dev.netChanges}</td>
                    <td>${dev.codingScore.toFixed(2)}</td>
                </tr>
            `;
            tableBody.append(row);
        });
}

function fetchPullRequestStats(appId) {
    $.ajax({
        url: `/GithubAnalytics/GetPullRequestStats?appId=${appId}`,
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            if (!response || !response.success || !response.data) {
                toastr.error(response?.errorMessage || "Pull request metrics not found", "Error");
                return;
            }

            const data = response.data;

            $('#total-prs').text(data.totalPRs ?? '--');
            $('#avg-pr-close-time').text((data.avgCloseTime != null ? data.avgCloseTime + ' days' : '--'));

            if (data.slowestPR) {
                $('#slowest-closed-pr').text('#' + (data.slowestPR.number ?? '--'));
                const user = data.slowestPR.username ?? 'Unknown';
                const duration = data.slowestPR.duration != null ? data.slowestPR.duration + ' days' : '--';
                $('#slowest-pr-user').text(`${user} (${duration})`);
            } else {
                $('#slowest-closed-pr').text('--');
                $('#slowest-pr-user').text('No data');
            }

            if (data.weeklyPRs && typeof data.weeklyPRs === 'object') {
                renderPRTrendChart(data.weeklyPRs);
            } else {
                clearPRChart();
            }
        },
        error: function (xhr) {
            const responseJson = xhr.responseJSON;
            toastr.error(responseJson?.errorMessage || "Pull request metrics not found", "Error");

            $('#total-prs').text('Error');
            $('#avg-pr-close-time').text('--');
            $('#slowest-closed-pr').text('--');
            $('#slowest-pr-user').text('Data not available');
            clearPRChart();
        }
    });
}

let prTrendChart;
function renderPRTrendChart(weeklyPRs) {
    const labels = Object.keys(weeklyPRs);
    const data = Object.values(weeklyPRs);

    const ctx = document.getElementById('pr-trend-chart').getContext('2d');

    if (prTrendChart) {
        prTrendChart.destroy();
    }

    prTrendChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'PR Count',
                data: data,
                backgroundColor: '#6f42c1'
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: { display: false }
            },
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });
}

function clearPRChart() {
    const ctx = document.getElementById('pr-trend-chart').getContext('2d');
    if (prTrendChart) {
        prTrendChart.destroy();
    }

    prTrendChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: [],
            datasets: [{
                label: 'PR Count',
                data: [],
                backgroundColor: '#6f42c1'
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: { display: false }
            },
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });
}

window.addEventListener('resize', () => {
    if (prTrendChart) {
        prTrendChart.resize();
    }
});
