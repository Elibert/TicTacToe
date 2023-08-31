﻿
$(document).ready(function () {
    window.sessionStorage.removeItem('minutes');
    window.sessionStorage.removeItem('seconds');
});

$("#create").click(function () {
    var user = $("#player1Name").val();
    if (user != "" && user != null) {
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
    else {
        alert("Please fill name!");
    }
})

$("#join").click(function () {
    var user = $("#player1Name").val();
    if (user != "" && user != null) {
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
    else {
        alert("Please fill name!");
    }
})

function joinRoom(playerId) {
    var gameCode = $("#gameCode").val();
    if (gameCode != "" && gameCode != null) {
        $.ajax({
            type: "GET",
            url: "/Home/ConnectGame/",
            contentType: 'application/json; charset=utf-8',
            datatype: 'json',
            data: {
                playerId: playerId,
                gameCode:gameCode
            },
            beforeSend: function () { $('#waiting_toJoin_bar').show(); },
            success: function (data) {
                $("#content").html(data);
                disableFunctions(true);
                $('#p1timer').text("1:00");
                countdown("p1timer","p2timer",false);
                window.history.pushState(null, '', '/Home/Game?gamecode='+gameCode);
            },
            error: function (xhr, ajaxOptions, thrownError) {
            },
            complete: function () {
                $('#waiting_toJoin_bar').hide();  
            }
        });
    }
    else {
        alert("Please fill name!");
    }
}


function copy() {
    navigator.clipboard.writeText($("#gameCode").text());

    $('#copied-success').css('opacity', '1');
    setTimeout(function () { $('#copied-success').css('opacity', '0') }, 500);
}