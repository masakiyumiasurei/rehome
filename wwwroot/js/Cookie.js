
// Cookieを設定する関数  第3引数はcookieの有効期限

function setCookie(name, value, days) {
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + value + expires + "; path=/";
}

// Cookieから値を取得する関数
function getCookie(name) {
    var match = document.cookie.match(new RegExp(name + '=([^;]+)'));
    //alert(name + ":" + match)
    return match ? match[1] : "";
}