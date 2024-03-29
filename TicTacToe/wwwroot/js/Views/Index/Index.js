﻿
$(document).ready(function () {
    window.sessionStorage.removeItem('minutes');
    window.sessionStorage.removeItem('seconds');
});

$("#create").click(function () {
    var user = $("#player1Name").val();
    if (user != "" && user != null) {
        if (user.length > 8) {
            showMessage('Username is longer than 8 characters!');
        }
        else {
            $.ajax({
                type: "GET",
                url: "/Home/CreateGame/",
                contentType: 'application/json; charset=utf-8',
                datatype: 'json',
                data: {
                    playerName: user,
                },
                //beforeSend: function () { $('#loading-container').show(); },
                success: function (data) {              
                    connection.invoke("Subscribe", user)
                        .catch(function (e) {
                        });
                    $("#content").html(data);
                },
                error: function (xhr, ajaxOptions, thrownError) {
                },
                //complete: function () {
                //    $('#loading-container').hide();
                //}
            });
        }
    }
    else {
        showMessage('Please fill name!');
    }
})

$("#join").click(function () {
    var user = $("#player1Name").val();
    if (user != "" && user != null) {
        if (user.length > 8) {
            showMessage('Username is longer than 8 characters!');
        }
        else {
            $.ajax({
                type: "GET",
                url: "/Home/JoinRoom/",
                contentType: 'application/json; charset=utf-8',
                datatype: 'json',
                data: {
                    playerName: user,
                },
                //beforeSend: function () { $('#loading-container').show(); },
                success: function (data) {
                    connection.invoke("Subscribe", user)
                        .catch(err => console.error(err));
                    $("#content").html(data);
                },
                error: function (xhr, ajaxOptions, thrownError) {
                },
                //complete: function () {
                //    $('#loading-container').hide();
                //}
            });
        }
    }
    else {
        showMessage('Please fill name!');
    }
})
function copy() {
    navigator.clipboard.writeText($("#gameCode").text());

    $('#copied-success').css('opacity', '1');
    setTimeout(function () { $('#copied-success').css('opacity', '0') }, 500);
}