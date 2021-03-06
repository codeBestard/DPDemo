﻿"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/followHub").build();

connection.on("ReceiveMessage", function (user, message) {
    //var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = message;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});

//document.getElementById("sendButton").addEventListener("click", function (event) {
  
//    var message = document.getElementById("messageInput").value;
//    connection.invoke("SendMessage", "test", message).catch(function (err) {
//        return console.error(err.toString());
//    });
//    event.preventDefault();
//});