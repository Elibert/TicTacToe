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
            beforeSend: function () { $('#loading-container').show(); },
            success: function (data) {
                connection.invoke("Subscribe", user)
                    .catch(err => console.error(err));
                $("#content").html(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
            },
            complete: function () {
                $('#loading-container').hide();
            }
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
            beforeSend: function () { $('#loading-container').show(); },
            success: function (data) {
                connection.invoke("Subscribe", user)
                    .catch(err => console.error(err));
                $("#content").html(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
            },
            complete: function () {
                $('#loading-container').hide();
            }
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
                $(".tic").css("pointer-events", "none");
                $("#playerName").prop("disabled", true);
                $("#changeClubs").prop("disabled", true);
                $('#p1timer').text("1:00");
                countdown("p1timer","p2timer");
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