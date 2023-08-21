const connection = new signalR.HubConnectionBuilder()
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

connection.on("changeTurns", (coordinateX, coordinateY, moveType, isRoundFinished) => {
    if (moveType != null) {
        if (moveType == 0) {
            moveType = 'X';
        }
        else {
            moveType = 'O';
        }
        $("#" + coordinateX + '_' + coordinateY).text(moveType);
    }
    if (isRoundFinished) {
    }
    else {
        var slideout = document.getElementById('notif');
        slideout.classList.toggle('visible');
        $(".tic").css("pointer-events", "auto");
        $("#playerName").prop("disabled", false);
        $("#changeClubs").prop("disabled", false);
        $("#playerName").val("")
        $("#message").text("");
    }
});
connection.on("selectedPlayer", (playerName) => {
    $("#message").text("Opponent selected " + playerName);
});
connection.on("changeRoundClubs", (newRoundClubs) => {
    for (var i = 0; i < Object.keys(newRoundClubs).length; i++) {
        $("#" + parseInt(i + 1) + " img").attr("alt", Object.keys(newRoundClubs)[i])
        $("#" + parseInt(i + 1) + " img").attr("src", Object.values(newRoundClubs)[i])
    }
    for (var i = 0; i <= 2; i++) {
        for (var j = 0; j <= 2; j++) {
            $("#"+i+"_"+j).text('');
        }
    }
});
