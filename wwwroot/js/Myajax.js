//コントローラーとメソッドを渡して、結果をもらう　未使用
function callServerAction(controller, action) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            url: '/' + controller + '/' + action,
            type: 'GET',
            success: function (data) {
                resolve(data); // 成功時、データを返す
            },
            error: function (error) {
                reject(error); // エラー時、エラーオブジェクトを返す
            }
        });
    });
}