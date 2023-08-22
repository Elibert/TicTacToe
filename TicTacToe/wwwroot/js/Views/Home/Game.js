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
                        // passing return data inside li tag
                        return data = '<li value='+data.playerId+'>' + data.playerName + '</li>';
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
    $("#playerName").val("")
    $("#message").text($(param).text() + " is selected.");

    searchInput.classList.remove("active");

    $.ajax({
        type: "GET",
        url: "/Home/SelectPlayer/",
        contentType: 'application/json; charset=utf-8',
        datatype: 'json',
        data: {
            userId: $("#OpponentUserId").val(),
            playerName: $(param).text(),
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
                    alert("Loja mbaroi");
                }
                else {
                    $(".tic").css("pointer-events", "none");
                    $("#playerName").prop("disabled", true);
                    $("#changeClubs").prop("disabled", true);
                    $("#playerName").val("")
                    $("#playerId").val("");
                    $("#message").text("");
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