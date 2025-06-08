$(document).ready(function () {
    $('#sidebarCollapse').on('click', function () {
        $('#sidebar').toggleClass('active');
    });

    $('.menu-items li a').on('click', function (e) {
        if ($(this).parent().hasClass('active')) {
            $(this).parent().removeClass('active');
            $(this).siblings('.submenu').slideUp();
        } else {
            $('.menu-items li').removeClass('active');
            $('.submenu').slideUp();
            $(this).parent().addClass('active');
            $(this).siblings('.submenu').slideDown();
        }

        if ($(this).siblings('.submenu').length) {
            e.preventDefault();
        }
    });

    $('#sidebar').on('transitionend', function () {
        if ($(this).hasClass('active')) {
            $('.menu-items li').removeClass('active');
            $('.submenu').slideUp();
        }
    });

    $(window).on('resize', function () {
        if ($(window).width() < 992) {
            $('#sidebar').removeClass('active');
        } else {
            $('#sidebar').addClass('active');
        }
    });

    if ($(window).width() < 992) {
        $('#sidebar').removeClass('active');
    }

    var projectInput = document.querySelector('input[name="ProjectId"]');
    if (projectInput) {
        var projectId = projectInput.value;
        var link = document.getElementById("goToBoardBtn");
        if (link) {
            link.href = "/Board/Index?projectId=" + encodeURIComponent(projectId);
        }
    }
});

$(document).ajaxStart(function () {
    $("#globalLoader").fadeIn(200);
});

$(document).ajaxStop(function () {
    $("#globalLoader").fadeOut(200);
});