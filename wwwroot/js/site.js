﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
/*function sendMessage(string accessTokenValue) {
  var message = document.getElementsByName("message")[0].value;
  
const url = "https://graph.microsoft.com/v1.0/me/chats/19:6e6ff219-ac42-4e80-b50f-c0d35b57da84_beb1c85e-5821-4235-b1aa-14bbcf995384@unq.gbl.spaces/messages";
const requestBody = {
  body: {
    content: message
  }
};
const accessToken =  accessTokenValue;

const xhttp = new XMLHttpRequest();

        xhttp.open("POST", url, true);
        xhttp.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
        xhttp.setRequestHeader("Authorization", "Bearer " + accessToken);
        xhttp.send(JSON.stringify(requestBody));

        xhttp.onload = function() {
          if (xhttp.status === 200) {
            console.log("Message sent successfully!");
          } else {
            console.log("Failed to send message:", xhttp.statusText);
          }
        };
}*/