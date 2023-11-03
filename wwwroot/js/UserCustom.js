

function UserCustom_Popup(FunctionName, URL) {
    var param = {
        RenderURL: URL
    };
    jQuery.ajax({
        method: "GET",
        url: "/UserCustom/ReturnPopup",
        data: param
    }).done(function (data) {
        // 通信成功時の処理

        $("#ModalHere").html(data);
        $("#ModalHere").find('.modal').modal('show');
        //$("#PopupCallURL").val(URL);

        //console.log("URL : " + url);
        console.log("data : " + data);
    }).fail(function (jqXHR, textStatus, errorThrown) {
        // 通信失敗時の処理
        alert('検索が失敗しました。');
        console.log("ajax通信に失敗しました");
        console.log("jqXHR          : " + jqXHR.status); // HTTPステータスが取得
        console.log("textStatus     : " + textStatus);    // タイムアウト、パースエラー
        console.log("errorThrown    : " + errorThrown.message); // 例外情報
        //console.log("URL            : " + url);
    }).always(function (data) {
        // 処理終了時

        Load_UserCustom(FunctionName);
        //if (FunctionName == 'SearchCustom') {
        //    Load_SearchCustom();
        //}
        //else if (FunctionName == 'IndexCustom') {
        //    Load_IndexCustom();
        //}
    });
}

function UserCustom_Reflect(FunctionName) {
    console.log("UserCustom_Reflect");

        $('[id^="' + FunctionName + '"]').each(function (i) {
            var id = $(this).attr('id');
            var val = localStorage.getItem(id);
            if (val == null) {
                val = UserCustom_Default[id];
            }
            if (val == 'OFF') {
                $(this).css("display", "none");
            }
            else if (val == 'ON') {
                $(this).css("display", "");
            }
        });
}

function UserCustom_Save(FunctionName, URL) {
    $('[id^="b_' + FunctionName + '"]').each(function (i) {
        var id = $(this).attr('id').slice(2);
        var val = $(this).text();
        localStorage.setItem(id, val);
    });
    if (URL != null) {
        window.location.href = URL;
    }
    else {
        UserCustom_Reflect(FunctionName);
        $('#ModalWindow').modal('hide');
    }
}

$(document).ready(function () {
    // ページ読み込み時に実行したい処理

    //UserCustomを使用したいフォームにid「UserCustom_FunctionName」のhiddenを持たせること
    $('[id^="UserCustom_FunctionName"]').each(function (i) {
        var FunctionName = $(this).val();
        console.log('FunctionName');
        console.log(FunctionName);
        UserCustom_Reflect(FunctionName);
    });
});
$(function () {
    $('#ModalHere').on('show.bs.modal', function (event) {
        // モーダルPopup時に実行したい処理

        //UserCustomを使用したいフォームにid「UserCustom_FunctionName」のhiddenを持たせること
        $('[id^="UserCustom_FunctionName"]').each(function (i) {
            var FunctionName = $(this).val();
            console.log('FunctionName');
            console.log(FunctionName);
            UserCustom_Reflect(FunctionName);
        });
    });
});

function Load_UserCustom(FunctionName) {

    $('[id^="b_' + FunctionName + '"]').each(function (i) {
        console.log('Load_UserCustom');
        var id = $(this).attr('id').slice(2);
        var LocalStorageValue = localStorage.getItem(id);
        if (LocalStorageValue == null) { LocalStorageValue = UserCustom_Default[id]; }//UserCustom_Defaultで指定した既定値を読取
        if (LocalStorageValue == null) { LocalStorageValue = "ON" }
        SwitchOnOff('b_' + id, LocalStorageValue);
        localStorage.setItem(id, LocalStorageValue);
    });
}


function Reset_UserCustom(FunctionName,URL) {
    var result = window.confirm('設定をリセットしますか？');
    if (result == false) {
        event.preventDefault();
    }

    Object.keys(localStorage).forEach(function (key) {
        if (key.startsWith(FunctionName)) {
            localStorage.removeItem(key);
        }
    });
    Load_UserCustom(FunctionName);
    if (URL != null) {
        window.location.href = URL;
    }
    else {
        UserCustom_Reflect(FunctionName);
    }
}


function SwitchOnOff(IDname,setCondition) {//検索条件表示非表示切替
    var target = $(event.target);
    if (IDname == null) {
        IDname = target.attr('id');
    }

    var id = '#' + IDname;
    var condition = $(id).text();
    if (condition == 'OFF') {
        $(id).text('ON');
        $(id).attr('class', 'custom-btn btn-1');
    }
    else {
        $(id).text('OFF');
        $(id).attr('class', 'custom-btn btn-2');
    }
    if (setCondition == 'ON') {
        $(id).text('ON');
        $(id).attr('class', 'custom-btn btn-1');
    }
    else if (setCondition == 'OFF') {
        $(id).text('OFF');
        $(id).attr('class', 'custom-btn btn-2');
    }
}