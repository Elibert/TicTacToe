const inputElements = [...document.querySelectorAll('.gameCodeInput')]
document.querySelector('#digit1').focus();
inputElements.forEach((ele, index) => {
    ele.addEventListener('keydown', (e) => {
        if (e.keyCode === 8 && e.target.value === '')
            inputElements[Math.max(0, index - 1)].focus()
    })
    ele.addEventListener('input', (e) => {
        const [first, ...rest] = e.target.value
        e.target.value = first ?? '' 
        const lastInputBox = index === inputElements.length - 1
        const didInsertContent = first !== undefined
        if (didInsertContent && !lastInputBox) {
            // continue to input the rest of the string
            inputElements[index + 1].focus()
            inputElements[index + 1].value = rest.join('')
            inputElements[index + 1].dispatchEvent(new Event('input'))
        }
        if (lastInputBox) {
            const code = inputElements.map(({ value }) => value).join('');
            if (code.length == 7) {
                var playerId = $("#playerId").val();
                $.ajax({
                    type: "GET",
                    url: "/Home/ConnectGame/",
                    contentType: 'application/json; charset=utf-8',
                    datatype: 'json',
                    data: {
                        playerId: playerId,
                        gameCode: code
                    },
                    beforeSend: function () { $('#waiting_toJoin_bar').show(); },
                    success: function (data) {
                        if (!data.correctCode && data.correctCode != undefined) {
                            showMessage("Game Code is invalid");
                            return false;
                        }
                        $("#content").html(data);
                        disableFunctions(true);
                        $('#p1timer').text("1:00");
                        countdown("p1timer", "p2timer", false);
                        window.history.pushState(null, '', '/Home/Game?gamecode=' + code);
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                    },
                    complete: function () {
                        $('#waiting_toJoin_bar').hide();
                    }
                });            }
        }
    })
})