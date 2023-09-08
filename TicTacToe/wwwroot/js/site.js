// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function showMessage(msg) {
    var slideout = document.getElementById('alert');
    slideout.innerHTML = msg;
    slideout.classList.remove('visible');
    void slideout.offsetWidth;
    slideout.classList.add('visible');
}