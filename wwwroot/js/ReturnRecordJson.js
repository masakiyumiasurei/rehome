


//<button type="button" class="btn-sm btn-primary" id="button" onclick="ReturnRecordJson('select * from RT_顧客 where 顧客ID=5')">ReturnRecordJson</button>
//sqlを引数にして呼び出す　sqlでセレクトされたレコード内容をjson形式で返す
function ReturnRecordJson(Table, WhereSql) {

    var param = {
        Table: Table,
        WhereSql: WhereSql
    };
    var url = '/ReturnRecordJson/ReturnRecordJson';
    return jQuery.ajax({
        method: "POST",
        url: url,
        data: param,
        dataType: 'json',
        async: false
    }).done(function (data) {
        // 通信成功時の処理
        var data_stringify = JSON.stringify(data);
        console.log("data_stringify : " + data_stringify);
        console.log("URL : " + url);
        console.log("data : " + data);
    }).fail(function (jqXHR, textStatus, errorThrown) {
        // 通信失敗時の処理
        console.log("ajax通信に失敗しました");
        console.log("jqXHR          : " + jqXHR.status); // HTTPステータスが取得
        console.log("textStatus     : " + textStatus);    // タイムアウト、パースエラー
        console.log("errorThrown    : " + errorThrown.message); // 例外情報
        console.log("URL            : " + url);
        return null;
    }).always(function (data) {
        // 処理終了時
    })
        .responseJSON[0];//Ajax実行返り値の中のresponseJSONにJSONデータが格納されている　1レコードのみ返しているので[0]を指定する
}

//function ReturnRecordJsonSqlAll(sql) {
//    var param = {
//        sql: sql
//    };
//    var url = '/ReturnRecordJson/ReturnRecordJsonSqlAll';

//    // Promiseを返す
//    return jQuery.ajax({
//        method: "POST",
//        url: url,
//        data: param,
//        dataType: 'json',
        
//    }).done(function (data) {
//        // 通信成功時の処理
//        var data_stringify = JSON.stringify(data);
//        console.log("data_stringify : " + data_stringify);
//        console.log("URL : " + url);
//        console.log("data : " + data);
//    }).fail(function (jqXHR, textStatus, errorThrown) {
//        // 通信失敗時の処理
//        console.log("ajax通信に失敗しました");
//        console.log("jqXHR          : " + jqXHR.status); // HTTPステータスが取得
//        console.log("textStatus     : " + textStatus);    // タイムアウト、パースエラー
//        console.log("errorThrown    : " + errorThrown.message); // 例外情報
//        console.log("URL            : " + url);
//        return null;
//    }).always(function (data) {
//        // 処理終了時
//    })
//        .responseJSON[0];
//}


