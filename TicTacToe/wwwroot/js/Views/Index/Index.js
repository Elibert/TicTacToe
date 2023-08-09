const connection = new signalR.HubConnectionBuilder()
    .withUrl("/TicTacToeHub")
    .build();

connection.on("ChangeScreenEnterGame", (gameCode) => {
    
    $.ajax({
        type: "GET",
        url: "/Home/ConnGame/",
        contentType: 'application/json; charset=utf-8',
        datatype: 'json',
        data: {
            gameCode: gameCode
        },
        success: function (data) {
            $("#content").html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });
});

connection.start()
    .then(() => {
        // Connection established
    })
    .catch(err => console.error(err));


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
            success: function (data) {
                connection.invoke("Subscribe", user)
                    .catch(err => console.error(err));
                $("#content").html(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
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
            success: function (data) {
                connection.invoke("Subscribe", user)
                    .catch(err => console.error(err));
                $("#content").html(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
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
            success: function (data) {
                connection.invoke("StartGame", gameCode)
                    .catch(err => console.error(err));
                $("#content").html(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
            }
        });
    }
    else {
        alert("Please fill name!");
    }
}