

$(function () {//読み込み時に実行される関数
    ChosenSet();
    NavibarSet();
    CheckLabel_Set();
    $('#ModalHere').on('show.bs.modal', function (event) {
        // モーダルPopup時に実行したい処理
        ChosenSet();
    });

    //trタグをクリックしたら発火
    $(document).on('click', 'td', function () {
        //console.log($(this));
        //var closest_tr = $(this).parent();
        //console.log(closest_tr);
        //console.log(closest_tr.index());

        //localStorage.setItem('tmp_tr', closest_tr.offset().top);
        $('td').css('background-color', '');
        $('td').css('transition','0.3s');
        $(this).css('background-color', '#DEE3FB');
    });
    //var closest_tr = localStorage.getItem('tmp_tr');
    //$('.table tr').eq(closest_tr).focus();
    //$("html,body").animate({
    //    scrollTop: closest_tr
    //});
    //console.log(closest_tr);
});


//Enter押下時のsubmitをキャンセルする
document.addEventListener('keydown', function (event) {
    if (event.key === 'Enter' && !event.shiftKey & location.pathname != '/Login/Login') {
        event.preventDefault();
    }
});

function CheckLabel_Set() {
    console.log('CheckLabel_Set');
    $('input:hidden').each(function (index, element) {
        var nextItem = $(element).next();
        if ((nextItem.attr('class') == "checkbox_label") && ($(element).val() == 'True')) {
            CheckLabel_Switch($(element).attr('name'), nextItem);
        }
    })
}

function CheckLabel_Switch(label_name, targetItem) {
    if (targetItem == null) { targetItem = $(event.target) }
    var labelcss = targetItem.attr('class');
    if (labelcss == 'checkbox_label') {
        targetItem.attr('class', 'checkbox_label_on');
        $("*[name='" + label_name + "']").val('True');
    }
    else if (labelcss == 'checkbox_label_on') {
        targetItem.attr('class', 'checkbox_label');
        $("*[name='" + label_name + "']").val('False');
    }
}

//ドロップダウンリストに自由入力するため、text属性のinputに値を配列で設定する
$('.free_dropdown').on('click focus', function () {
    //「input」要素の「data-options」をカンマで分割し、配列にする。
    var options = $(this).data("options").split(',');
    $(this).autocomplete({
        source: options,
        minLength: 0,  // 「0」を設定したら、全ての項目を表示する。
        delay: 1,
        autoFocus: false,
        scroll: true,
        position: { my: "right top", at: "right bottom", collision: "flip" } //不具合対応

    });
    $(this).autocomplete("search", "");//この行を入れないと、初回にプルダウンボックス（セレクトボックス）が効かないという不具合がある
});

function MoveToTop(){//ﾍﾟｰｼﾞの一番上まで移動
    $('body, html').scrollTop(0);
}

function nz(val1, val2) {//AccessのNz関数と同じ処理
    if (!val2) val2 = "";
    return (val1 == null) ? val2 : val1;
}

function DeleteConfirm() {
    var result = window.confirm('削除しますか？');
    if (result == false) {
        event.preventDefault();
    }
}
function Submit() {
    $('form').submit();
}
// 日付をYYYY-MM-DDの書式で返すメソッド
function formatDate(dt) {
    var y = dt.getFullYear();
    var m = ('00' + (dt.getMonth() + 1)).slice(-2);
    var d = ('00' + dt.getDate()).slice(-2);
    return (y + '-' + m + '-' + d);
}

function SwitchSearch() {//検索条件表示非表示切替
    var condition = $('#Search-Area').attr('name');
    console.log(condition);
    if (condition == 'show') {
        $('#Search-Area').hide("normal");
        $('#Search-Area').attr('name', 'hide');
        $('#SwitchButton').attr('class', 'custom-btn btn-1');
    }
    else {
        $('#Search-Area').show("normal");
        $('#Search-Area').attr('name', 'show');
        $('#SwitchButton').attr('class', 'custom-btn btn-2');
    }
}
$(function () {
    console.log('called SwitchSearch');
    if (location.pathname.indexOf('Search') !== -1) {
        SwitchSearch();
    }
});


function OmitTextSwitch(){//css 「omit_text」 指定した文字の表示非表示切り替え
    console.log('called OmitTextSwitch');
    $('.omit_text').css('background-color', '');
    $(event.target).toggleClass('omit_text');
    $(event.target).toggleClass('omit_text_full');
}
//function OmitPopup() {
//    console.log('OmitPopup');
//    var Element = $(event.target).next(".omit_text_full");
//    Element.show();
//    $(event.target).hide();
//    //Element.css('cssText', 'display: block !important');
//}
//function OmitHide() {
//    console.log('OmitHide');
//    var Element = $(event.target).next(".omit_text_full");
//    Element.hide();
//    $(event.target).show();
//    //Element.css('cssText', 'display: none !important');
//}

//function textTrim() {
//    // テキストをトリミングする要素
//    var selector = document.getElementsByClassName('omit_text');

//    // 制限する文字数
//    var wordCount = 80;
//    // 改行する文字数
//    let brCount = 20;

//    // 文末に追加したい文字
//    var clamp = '…';

//    for (var i = 0; i < selector.length; i++) {
//        // 文字数を超えたら
//        if (selector[i].innerText.length > wordCount) {
//            var str = selector[i].innerText; // 文字数を取得
//            str = str.substr(0, (wordCount - 1)); // 1文字削る
//            selector[i].innerText = str + clamp; // 文末にテキストを足す
//        }
//    }

//}

function OpenGoogleMap(TextboxName1, TextboxName2) {
    var Address = $("*[name='" + TextboxName1 + "']").val();
    if (TextboxName2 != null) { Address = Address + $("*[name='" + TextboxName2 + "']").val(); }
    var GoogleMapURL = "https://www.google.co.jp/maps?hl=ja&tab=wl&q=";
    window.open(GoogleMapURL + Address, '_blank');
}


function Clear_Multi(val1,val2,val3,val4,val5) {//引数に指定したname属性のvalを空にする 5つまで
    $("*[name='" + val1 + "']").val('');
    $("*[name='" + val2 + "']").val('');
    $("*[name='" + val3 + "']").val('');
    $("*[name='" + val4 + "']").val('');
    $("*[name='" + val5+ "']").val('');
}

function ChosenSet() {//Jquery chosenライブラリを利用して、DropDownをフィルタ可能コンボボックスにする
    console.log('called chosen');
    $(".form-control-chosen").chosen({
        search_contains: true,
        no_results_text: "ありません"
    });
}

function NavibarSet() {//ナビゲーションバー選択位置表示
    var page = new Array();
    page[1] = 'Client';
    page[2] = 'Quote';
    page[3] = 'Dounyu';
    page[4] = 'Renraku';
    page[5] = 'Nissi';
    page[6] = '';

    var page2 = new Array();
    page2[1] = 'New';

    var url = location.href.split('/');
    var now = url[3];
    var now2 = url[4];

    for (var i = 1; i <= page.length; i++) {
        if (page[i] == now) {
            if (page[i] == page[5]) {
                if (page2[1] == now2) {
                    $('#nav-Nissi-New').addClass('nav-active');
                } else {
                    $('#nav-Nissi-Index').addClass('nav-active');
                }
                $('#navbarDropDownMenuLinkNissi').addClass('nav-active');
            }else if (page[i] == page[6]) {
                $('#nav-Client').addClass('nav-active');
            } else {
                var actv = page[i];
                $('#nav-' + actv).addClass('nav-active');
            }
        }
    }
}


//const list = document.querySelectorAll(".list");
//console.log(list);

//function activeLink() {
//    list.forEach((item) =>
//        // console.log(item);
//        item.classList.remove("active")
//    );
//    this.classList.add("active");
//}

//list.forEach((item) => {
//    item.addEventListener("click", activeLink);
//});


jQuery(function ($) {
    // デフォルトの設定を変更
    $.extend($.fn.dataTable.defaults, {
        language: {
            url: "//cdn.datatables.net/plug-ins/1.13.4/i18n/ja.json"
        }
    });
    $("#myTable").DataTable({
        //searching: false,
        order: [],
        scrollX: true,
        scrollY: 600
    });
});