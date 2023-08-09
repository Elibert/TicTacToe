// getting all required elements
const searchInput = document.querySelector(".searchInput");
const input = searchInput.querySelector("input");
const resultBox = searchInput.querySelector(".resultBox");
const icon = searchInput.querySelector(".icon");
let linkTag = searchInput.querySelector("a");
let webLink;

// if user press any key and release
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
    $("#playerName").text("");
    $("#message").text($(param).text()+" is selected.");
    searchInput.classList.remove("active");
}

$(".tic").click(function () {
    if ($("#playerId").val() == "" || $("#playerId").val() == null) {
        alert("select a player!");
    }
    else {
        $.ajax({
            type: "GET",
            url: "/Home/MakeMove/",
            contentType: 'application/json; charset=utf-8',
            datatype: 'json',
            data: {
                GameId: $("#GameId").val(),
                CoordinateX: 0,
                CoordinateY: 0,
                PlayerId: $("#playerId").val(),
                Movetype: "Y"
            }, 
            success: function (data) {

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

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/TicTacToeHub")
    .build();

connection.on("ReceiveMessage", (message) => {
    // Handle received message
    console.log(message);
});

connection.start()
    .then(() => {
        // Connection established
    })
    .catch(err => console.error(err));

document.getElementById("sendMessageBtn").addEventListener("click", () => {
    const userId = "user123"; // Replace with the actual user ID
    const message = "Hello, unauthenticated user!";
    connection.invoke("SendMessageToUser", userId, message)
        .catch(err => console.error(err));
})