using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using Microsoft.EntityFrameworkCore;
using rehome.Enums;

namespace rehome.Models.DB
{
    public class 顧客
    {
        [Key]
        public int 顧客ID { get; set; }
        [StringLength(50)]

        [Required(ErrorMessage = "{0}を入力してください。")]
        public string? 顧客名 { get; set; }
        public string? 郵便番号 { get; set; }

        public string? 住所1 { get; set; }

        public string? 住所2 { get; set; }

        public string? 建物種別 { get; set; }
        public string? 電話番号1 { get; set; }
        public string? 電話番号2 { get; set; }

        public string? 肩書 { get; set; }
             
        public string? FAX { get; set; }

        [EmailAddress(ErrorMessage = "有効なメールアドレスを入力してください。")]
        public string? メールアドレス { get; set; }

        public string? カナ { get; set; }

        public string? 紹介者 { get; set; }

        public string? 備考 { get; set; }

        public bool 宛名印刷FLG { get; set; }

        public string? 依頼者種別 { get; set; }

        public bool ブラックリストFLG { get; set; }

        public string? 知った理由 { get; set; }

        public string? 依頼理由 { get; set; }

        public yesno 工事検討 { get; set; }
        //public　string? 工事検討 { get; set; }
        public bool 玄関FLG { get; set; }

        public bool リビングFLG { get; set; }

        public bool キッチンFLG { get; set; }
        public bool トイレFLG { get; set; }
        public bool 洗面所FLG { get; set; }
        public bool 風呂FLG { get; set; }
        public bool 居室FLG { get; set; }
        public bool 建具FLG { get; set; }
            
        public bool 窓FLG { get; set; }

        public bool 内装FLG { get; set; }
        public bool 外壁FLG { get; set; }
        public bool 屋根FLG { get; set; }
        public string? その他 { get; set; }

        public static IList<SelectListItem> yesnoitems()
        {
            var tmplist = new List<SelectListItem>();
            tmplist.Add(new SelectListItem() { Text = "はい", Value = "はい" });
            tmplist.Add(new SelectListItem() { Text = "いいえ", Value = "いいえ" }); 
            return tmplist;
        }

    }
}
