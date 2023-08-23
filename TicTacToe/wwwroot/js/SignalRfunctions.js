﻿const connection = new signalR.HubConnectionBuilder()
    .withUrl("/TicTacToeHub")
    .build();

connection.on("ChangeScreenEnterGame", (gameId) => {
    $.ajax({
        type: "GET",
        url: "/Home/ConnGame/",
        contentType: 'application/json; charset=utf-8',
        datatype: 'json',
        data: {
            gameId: gameId
        },
        beforeSend: function () { $('#loading-container').show(); },
        success: function (data) {
            $("#content").html(data);
            $('#p1timer').text("1:00");
            countdown("p1timer", "p2timer");
        },
        error: function (xhr, ajaxOptions, thrownError) {
        },
        complete: function () {
            $('#loading-container').hide();
        }
    });
});

connection.start()
    .then(() => {
        // Connection established
    })
    .catch(err => console.error(err));

connection.on("changeTurns", (coordinateX, coordinateY, moveType, isRoundFinished, activetimer) => {
    if (moveType != null) {
        var fontColor;
        if (moveType == 0) {
            moveType = 'X';
            fontColor = 'red';
        }
        else {
            moveType = 'O';
            fontColor = 'blue';
        }
        $("#" + coordinateX + '_' + coordinateY).text(moveType);
        $("#" + coordinateX + '_' + coordinateY).css('color', fontColor);
    }
    if (isRoundFinished) {
    }
    else {
        var nonactivetimer;
        if (activetimer == "p1timer") {
            nonactivetimer = "p2timer";
        }
        else {
            nonactivetimer = "p1timer";
        }
        var slideout = document.getElementById('notif');
        slideout.classList.toggle('visible');
        $(".tic").css("pointer-events", "auto");
        $("#playerName").prop("disabled", false);
        $("#changeClubs").prop("disabled", false);
        $("#playerName").val("")
        $("#message").text("");

        $("#" + nonactivetimer).css("display", "none");
        $("#" + activetimer).css("display", "block");
        $('#' + activetimer).text("1:00");
        
        countdown(activetimer, nonactivetimer);
    }
});
connection.on("selectedPlayer", (playerName) => {
    $("#message").text("Opponent selected " + playerName);
});
connection.on("changeRoundClubs", (newRoundClubs) => {
    for (var i = 0; i < Object.keys(newRoundClubs).length; i++) {
        $("#" + parseInt(i + 1) + " .clubLogo").attr("alt", Object.keys(newRoundClubs)[i])
        $("#" + parseInt(i + 1) + " .clubLogo").attr("src", Object.values(newRoundClubs)[i])
    }
    for (var i = 0; i <= 2; i++) {
        for (var j = 0; j <= 2; j++) {
            $("#"+i+"_"+j).text('');
        }
    }
});
