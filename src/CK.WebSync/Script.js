var ws;
(function () {
    ws = new WebSocket("ws://localhost:8080/ws");

    ws.onopen = function () {
        console.log("CK:", "opened");

        //ws.send("CK Says Hi");
    };

    ws.onmessage = function (e) {
        console.log("CK:", "onmessage", e);
    };

    ws.onclose = function () {
        console.log("CK:", "onclose");
    };
}());