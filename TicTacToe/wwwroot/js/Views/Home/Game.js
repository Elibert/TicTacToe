$(document).ready(function () {
    if ($("#CurrentRound_isPlayerTurn").val().toLowerCase() == 'true') {
        disableFunctions(false);
    }
    else {
        disableFunctions(true);
    }

    if (window.sessionStorage.getItem('minutes') != null) {
        if ($("#CurrentRound_IsP1Turn").val().toLowerCase() == 'true') {
            $("#p2timer").css("display", "none");
            $("#p1timer").css("display", "block");
            var seconds = parseInt(window.sessionStorage.getItem('seconds')) - 1;
            $("#p1timer").text(window.sessionStorage.getItem('minutes') + ":" + seconds);
            countdown("p1timer", "p2timer", $("#CurrentRound_isPlayerTurn").val().toLowerCase() == 'true');
        }
        else {
            $("#p1timer").css("display", "none");
            $("#p2timer").css("display", "block");
            var seconds = parseInt(window.sessionStorage.getItem('seconds')) - 1;
            $("#p2timer").text(window.sessionStorage.getItem('minutes') + ":" + seconds);
            countdown("p2timer", "p1timer", $("#CurrentRound_isPlayerTurn").val().toLowerCase() == 'true');
        }
    }

    $(".gameMoveType").each(function () {
        if ($(this).text().trim() == 'X')
            $(this).css('color', 'red');
        else if ($(this).text().trim() == 'O')
            $(this).css('color', 'blue');
    });
});

const searchInput = document.querySelector(".searchInput");
const input = searchInput.querySelector("input");
const resultBox = searchInput.querySelector(".resultBox");

input.onkeyup = (e) => {
    let userData = e.target.value; //user enetered data
    let emptyArray = [];
    if (userData) {
        $.ajax({
            type: "GET",
            url: "/Home/GetPlayersAutoComplete/",
            contentType: 'application/json; charset=utf-8',
            datatype: 'json',
            data: {
                playerName: userData
            },
            success: function (data) {              
                if (data.length != 0) {
                    emptyArray = data;
                    emptyArray = emptyArray.map((data) => {
                        var birthD = data.birthdate.split('T')[0];
                        return data = '<li value=' + data.playerId + '>' + data.playerName + ' (' + birthD +')' + '</li>';
                    });
                    searchInput.classList.add("active"); //show autocomplete box
                    showSuggestions(emptyArray);
                    let allList = resultBox.querySelectorAll("li");
                    for (let i = 0; i < allList.length; i++) {
                        //adding onclick attribute in all li tag
                        allList[i].setAttribute("onclick", "select(this)");
                    }
                }
                else {
                    searchInput.classList.remove("active"); //hide autocomplete box
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
            }
        });


    } else {
        searchInput.classList.remove("active"); //hide autocomplete box
    }
}
function select(param) {
    $("#playerId").val($(param).val());
    $("#playerName").val("");
    var playerNm = $(param).text().split('(')[0];
    $("#message").text(playerNm + " is selected.");

    searchInput.classList.remove("active");

    $.ajax({
        type: "GET",
        url: "/Home/SelectPlayer/",
        contentType: 'application/json; charset=utf-8',
        datatype: 'json',
        data: {
            userId: $("#OpponentUserId").val(),
            playerName: playerNm,
        },
        success: function (data) {
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });
}

$(".tic").click(function () {
    if ($("#playerId").val() == "" || $("#playerId").val() == null) {
        alert("Select a player!");
    }
    else {
        var slot = $(this).attr('id');
        $.ajax({
            type: "GET",
            url: "/Home/MakeMove/",
            contentType: 'application/json; charset=utf-8',
            datatype: 'json',
            data: {
                GameId: $("#GameId").val(),
                CoordinateX: slot.split('_')[0],
                CoordinateY: slot.split('_')[1],
                PlayerId: $("#playerId").val(),
                Movetype: $("#MoveType").val()
            }, 
            success: function (data) {
                if (data.finishedRound) {
                    if ($("#MoveType").val() == 'X') {
                        fontColor = 'red';
                    }
                    else {
                        fontColor = 'blue';
                    }
                    $("#" + slot).text($("#MoveType").val());
                    $("#" + slot).css('color', fontColor);
                    alert("Loja mbaroi!");
                }
                else {
                    disableFunctions(true);
                    $("#playerName").val("")
                    $("#playerId").val("");
                    $("#message").text("");
                    var nonactivetimer, activetimer;
                    if (!data.isP1turn) {
                        activetimer = "p1timer"
                        nonactivetimer = "p2timer";
                    }
                    else {
                        activetimer = "p2timer"
                        nonactivetimer = "p1timer";
                    }
                    $("#" + nonactivetimer).css("display", "none");
                    $("#" + activetimer).css("display", "block");
                    $("#" + activetimer).text("1:00");
                    countdown(activetimer, nonactivetimer,false);
                    if (data.correctMove) {
                        var fontColor;
                        if ($("#MoveType").val() == 'X') {
                            fontColor = 'red';
                        }
                        else {
                            fontColor = 'blue';
                        }
                        $("#" + slot).text($("#MoveType").val());
                        $("#" + slot).css('color', fontColor);
                    }
                    else {
                        alert("wrong move");
                    }
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
            }
        });
    }
});

function showSuggestions(list) {
    let listData;
    if (!list.length) {
        userValue = inputBox.value;
        listData = '<li>' + userValue + '</li>';
    } else {
        listData = list.join('');
    }
    resultBox.innerHTML = listData;
}
$("#changeClubs").click(function () {
    $.ajax({
        type: "GET",
        url: "/Home/changeRoundClubs/",
        contentType: 'application/json; charset=utf-8',
        datatype: 'json',
        beforeSend: function () {
            $('.small_loading').show();
            $('.clubLogo').hide();
        },
        data: {
            gameId: $("#GameId").val(),
        },
        success: function (data) {

        },
        complete: function(){
            $('.small_loading').hide();
            $('.clubLogo').show();
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });
});

var interval;

function countdown(activetimer,nonactivetimer,endturn) {
    clearInterval(interval);
    interval = setInterval(function () {
        var timer = $('#' + activetimer).html();
        timer = timer.split(':');
        var minutes = timer[0];
        var seconds = timer[1];
        seconds -= 1;
        if (minutes < 0) return;
        else if (seconds < 0 && minutes != 0) {
            minutes -= 1;
            seconds = 59;
        }
        else if (seconds < 10 && length.seconds != 2) seconds = '0' + seconds;

        $('#' + activetimer).html(minutes + ':' + seconds);
          window.sessionStorage.setItem('minutes', minutes);
          window.sessionStorage.setItem('seconds', seconds);

        if (minutes == 0 && seconds == 0) {
            window.sessionStorage.removeItem('minutes');
            window.sessionStorage.removeItem('seconds');
            if (endturn) {
                $.ajax({
                    type: "GET",
                    url: "/Home/MakeMove/",
                    contentType: 'application/json; charset=utf-8',
                    datatype: 'json',
                    data: {
                        GameId: $("#GameId").val(),
                        Movetype: $("#MoveType").val()
                    },
                    success: function (data) {
                        if (data.finishedRound) {
                            alert("Loja mbaroi");
                        }
                        else {
                            disableFunctions(true);
                            searchInput.classList.remove("active"); 
                            $("#playerName").val("")
                            $("#playerId").val("");
                            $("#message").text("");
                            $("#" + activetimer).css("display", "none");
                            $("#" + nonactivetimer).css("display", "block");
                            $("#" + nonactivetimer).text("1:00");
                            countdown(nonactivetimer, activetimer, false);
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                    }
                });
            }

            clearInterval(interval);
        } 
    }, 1000);
}