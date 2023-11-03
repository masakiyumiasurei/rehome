
$(function () {//読み込み時に実行される関数

    $(document).on("click", "#Remove", function () {
        $(this).closest('tr').remove();
        RenameNumber();
    });

    //trタグをクリックしたら発火
    $(document).on('click', '.td', function () {
        //console.log($(this));
        //var closest_tr = $(this).parent();
        //console.log(closest_tr);
        //console.log(closest_tr.index());

        //localStorage.setItem('tmp_tr', closest_tr.offset().top);
        $('td').css('background-color', '');
        $('td').css('transition', '0.3s');
        $(this).css('background-color', '#DEE3FB');
    });

});
    //var closest_tr = localStorage.getItem('tmp_tr');
    //$('.table tr').eq(closest_tr).focus();
    //$("html,body").animate({
    //    scrollTop: closest_tr
    //});
    //console.log(closest_tr);

function RenameNumber() {
    console.log('called Renumber');
    $('table tr').each(function () {
        var idx = $(this).prop('rowIndex') - 1;
        var name;
        var rowStr;
        for (let i = 0; i < $(this).children().length; ++i) {
            for (let j = 0; j < $(this).children().eq(i).children().length; ++j) {
                for (let k = 0; k < $(this).children().eq(i).children().eq(j).children().length; ++k) {
                    for (let l = 0; l < $(this).children().eq(i).children().eq(j).children().eq(k).children().length; ++l) {
                        name = null;
                        name = $(this).children().eq(i).children().eq(j).children().eq(k).children().eq(l).attr('name');
                        if (name != null) {
                            console.log('name l:' + name);
                            rowStr = name.substr(name.indexOf('['));
                            rowStr = rowStr.substr(0, rowStr.indexOf(']') + 1);
                            var newname = name.replace(rowStr, '[' + String(idx) + ']');
                            $(this).children().eq(i).children().eq(j).children().eq(k).children().eq(l).attr('name', newname);
                            $(this).children().eq(i).children().eq(j).children().eq(k).children().eq(l).attr('id', newname);
                        }
                    }
                    name = null;
                    name = $(this).children().eq(i).children().eq(j).children().eq(k).attr('name');
                    if (name != null) {
                        console.log('name k:' + name);
                        rowStr = name.substr(name.indexOf('['));
                        rowStr = rowStr.substr(0, rowStr.indexOf(']') + 1);
                        var newname = name.replace(rowStr, '[' + String(idx) + ']');
                        $(this).children().eq(i).children().eq(j).children().eq(k).attr('name', newname);
                        $(this).children().eq(i).children().eq(j).children().eq(k).attr('id', newname);
                    }
                }
                name = null;
                name = $(this).children().eq(i).children().eq(j).attr('name');
                if (name != null) {
                    console.log('name j:' + name);
                    rowStr = name.substr(name.indexOf('['));
                    rowStr = rowStr.substr(0, rowStr.indexOf(']') + 1);
                    var newname = name.replace(rowStr, '[' + String(idx) + ']');
                    $(this).children().eq(i).children().eq(j).attr('name', newname);
                    $(this).children().eq(i).children().eq(j).attr('id', newname);
                }
            }
        }
    });
}

function AddSelectedTR(TableID, JsonResult) {//tableにTRを追加し、JsonResultの中身に書き換える
                                    //JsonResultとtdのName属性が同じ場合に使える
    //Ajax返り値からデータを取得
    AddTR(TableID);
    var TR = $('#' + TableID + ' tbody tr').last();
    // tdの数
    var tdLen = TR.children().length;
    for (let i = 0; i < tdLen; ++i) {
        for (let j = 0; j < TR.children().eq(i).children().length; ++j) {
            var name = TR.children().eq(i).children().eq(j).attr('name');
            if (name != null) {
                var ColumnName = name.substr(name.indexOf('].') + 2);
                TR.children().eq(i).children().eq(j).val(nz(JsonResult[ColumnName]));
                console.log(JsonResult[ColumnName]);
            }
        }
    }
    RenameNumber();
}

function AddTR(TableID) {
    var TR = $('#' + TableID + ' tbody tr').first().clone();
    console.log(TR);
    TR.appendTo('#' + TableID+' tbody');
}

