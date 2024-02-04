using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public bool 宛名印刷FLG { get; set; } = true;

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

        public string? 法人名 { get; set; }

        public string? 詳細備考 { get; set; }

        public bool 依頼理由金額FLG { get; set; }
        public bool 依頼理由信頼感FLG { get; set; }
        public bool 依頼理由紹介業者FLG { get; set; }
        public bool 依頼理由HPFLG { get; set; }

        public string? 賃貸分譲区分 { get; set; }

        public static IList<SelectListItem> 賃貸分譲items()
        {
            var tmplist = new List<SelectListItem>();
            tmplist.Add(new SelectListItem() { Text = "賃貸", Value = "賃貸" });
            tmplist.Add(new SelectListItem() { Text = "分譲", Value = "分譲" });
            return tmplist;
        }

        public static IList<SelectListItem> 依頼者種別items()
        {
            var tmplist = new List<SelectListItem>();
            tmplist.Add(new SelectListItem() { Text = "個人", Value = "個人" });
            tmplist.Add(new SelectListItem() { Text = "法人", Value = "法人" });
            tmplist.Add(new SelectListItem() { Text = "家主", Value = "家主" });
            tmplist.Add(new SelectListItem() { Text = "不動産屋", Value = "不動産屋" });
            tmplist.Add(new SelectListItem() { Text = "業者", Value = "業者"});
            tmplist.Add(new SelectListItem() { Text = "その他", Value = "その他" });
            return tmplist;
        }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = false)]
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? 生年月日 { get; set; }
    }
}
