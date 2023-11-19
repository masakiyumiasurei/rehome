function PopupSearch(CallerName, URL) {
    console.log("CallerName : " + CallerName);
    jQuery.ajax({
        method: "GET",
        url: URL,
    }).done(function (data) {
        // 通信成功時の処理

        $("#ModalHere").html(data);
        $("#ModalHere").find('.modal').modal('show');
        $("#CallerName").val(CallerName);
        $("#PopupCallURL").val(URL);

        //console.log("URL : " + url);
        //console.log("data : " + data);
    }).fail(function (jqXHR, textStatus, errorThrown) {
        // 通信失敗時の処理
        alert('検索が失敗しました。');
        console.log("ajax通信に失敗しました");
        console.log("jqXHR          : " + jqXHR.status); // HTTPステータスが取得
        console.log("textStatus     : " + textStatus);    // タイムアウト、パースエラー
        console.log("errorThrown    : " + errorThrown.message); // 例外情報
        console.log("URL            : " + URL);
    }).always(function (data) {
        // 処理終了時
    });
}


function PopupSearch_Syouhin() {
    console.log("PopupSearch_Syouhin");
    $('#loading').show();
    $('#PopupResultView').html('');
    var URL = $("#PopupCallURL").val();
    var returnStr = "";
    var param = {
        PopupSyouhinID: $('#PopupSyouhinID').val(),
        PopupSyouhinName: $('#PopupSyouhinName').val(),
        PopupMakerName: $('#PopupMakerName').val(),
        PopupNumber: $('#PopupNumber').val()
    };
    console.log(param);

    console.log(URL);
    jQuery.ajax({
        method: "POST",
        url: URL,
        data: param,
        dataType: 'html',
    }).done(function (data) {
        // 通信成功時の処理
        $('#PopupResultView').html(data);

        //console.log("data : " + data);
    }).fail(function (jqXHR, textStatus, errorThrown) {
        // 通信失敗時の処理
        alert('検索が失敗しました。');
        console.log("ajax通信に失敗しました");
        console.log("jqXHR          : " + jqXHR.status); // HTTPステータスが取得
        console.log("textStatus     : " + textStatus);    // タイムアウト、パースエラー
        console.log("errorThrown    : " + errorThrown.message); // 例外情報
        console.log("URL            : " + url);
    }).always(function (data) {
        // 処理終了時
        $('#loading').hide();
    });
}

//ポップアップフォームの選択ボタンにセットしているid属性PopupSelectがクリックされたときの処理
//選択したレコードのidを取得してメインフォームの所定のname属性のボックスにidを登録する
$(function () {
    $(document).on("click", "#PopupSelect", function () {
        var ID = $(this).closest('tr').children("td")[1].innerText;

        var CallerName = $("#CallerName").val();
        console.log(CallerName);
        $('input[name="' + CallerName + '"]').val(ID).change();
        $('#ModalWindow').modal('hide');
    });
});