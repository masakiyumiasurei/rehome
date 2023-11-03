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
        console.log("URL            : " + url);
    }).always(function (data) {
        // 処理終了時
    });
}
function PopupSearch_Client() {
    $('#loading').show();
    $('#PopupResultView').html('');
    var URL = $("#PopupCallURL").val();
    var returnStr = "";

    var param = {
        顧客ID: $("*[name='ClientSearchConditions.顧客ID']").val(),
        顧客名: $("*[name='ClientSearchConditions.顧客名']").val(),
        顧客名カナ: $("*[name='ClientSearchConditions.顧客名カナ']").val(),
        住所: $("*[name='ClientSearchConditions.住所']").val(),
        メイン担当: $("*[name='ClientSearchConditions.メイン担当']").val(),
        メイン担当所属: $("*[name='ClientSearchConditions.メイン担当所属']").val(),
        エリア: $("*[name='ClientSearchConditions.エリアID']").val(),
        商魂コード: $("*[name='ClientSearchConditions.商魂コード']").val(),
        代理店ID: $("*[name='ClientSearchConditions.代理店ID']").val(),
        代理店名: $("*[name='ClientSearchConditions.代理店名']").val(),
        TEL: $("*[name='ClientSearchConditions.TEL']").val(),
        業種: $("*[name='ClientSearchConditions.業種']").val(),
        決算月: $("*[name='ClientSearchConditions.決算月']").val(),
        担当法人: $("*[name='ClientSearchConditions.担当法人']").val(),
        グループ: $("*[name='ClientSearchConditions.グループID']").val(),
        取引開始日: $("*[name='ClientSearchConditions.取引開始日']").val(),
        登録日: $("*[name='ClientSearchConditions.登録日']").val(),
        更新日: $("*[name='ClientSearchConditions.更新日']").val(),
        取引停止日: $("*[name='ClientSearchConditions.取引停止日']").val(),
        廃業: $("*[name='ClientSearchConditions.廃業']").val()
    };

    var modelDataJSON = '@Html.Raw(Json.Encode(Model))';
    var url = '@Url.Action("PopupSearchClient", "Client")';
    console.log(param);
    console.log(modelDataJSON);
    jQuery.ajax({
        method: "POST",
        url: URL,
        data: param,
        dataType: 'html',
    }).done(function (data) {
        // 通信成功時の処理
        $('#PopupResultView').html(data);

        console.log("URL : " + url);
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
        UserCustom_Reflect('IndexCustom');
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


$(function () {
    console.log('called PopupSelect');
    $(document).on("click", "#PopupSelect", function () {
        var ID = $(this).closest('tr').children("td")[1].innerText;
        console.log(ID);

        var CallerName = $("#CallerName").val();
        console.log(CallerName);
        $('input[name="' + CallerName + '"]').val(ID).change();
        $('#ModalWindow').modal('hide');
    });
});