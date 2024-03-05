using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;　//これを定義しないとDropDownListForにIListを渡せない
using rehome.Enums;

namespace rehome.Models.DB
{
    
    public class 見積
    {
        [Key]
        public int 見積ID { get; set; }
        [Key]
        public int 履歴番号 { get; set; } = 1;
        [Required(ErrorMessage = "{0}を入力してください。")]
        public string? 見積番号 { get; set; }
        public bool? 最新FLG { get; set; } = true;
        public int 担当ID { get; set; }

        public int 顧客ID { get; set; }

        public string? 担当者名 { get; set; }

        public int? 営業所ID { get; set; }

        public string? 営業所名 { get; set; }

        public string? 顧客名 { get; set; }

        public string? 敬称 { get; set; } = "様";

        public static IList<SelectListItem> 敬称items()
        {
            var tmplist = new List<SelectListItem>();
            tmplist.Add(new SelectListItem() { Text = "御中", Value = "御中" });
            tmplist.Add(new SelectListItem() { Text = "様", Value = "様" });
            return tmplist;
        }
        [Required(ErrorMessage = "{0}を入力してください。")]
        public string? 件名 { get; set; }

        public string? 納期 { get; set; }

        public string? 項目 { get; set; }

        public static IList<SelectListItem> 項目items()
        {
            var tmplist = new List<SelectListItem>();
            tmplist.Add(new SelectListItem() { Text = "一般", Value = "一般" });
            tmplist.Add(new SelectListItem() { Text = "JS", Value = "JS" });
            tmplist.Add(new SelectListItem() { Text = "JS(外注)", Value = "JS(外注)" });
            tmplist.Add(new SelectListItem() { Text = "網戸", Value = "網戸" });
            tmplist.Add(new SelectListItem() { Text = "インプラス", Value = "インプラス" });
            tmplist.Add(new SelectListItem() { Text = "住改", Value = "住改" });
            return tmplist;
        }

        public string? 種類 { get; set; }

        public static IList<SelectListItem> 種類items()
        {
            var tmplist = new List<SelectListItem>();
            tmplist.Add(new SelectListItem() { Text = "建築", Value = "建築" });
            tmplist.Add(new SelectListItem() { Text = "内装", Value = "内装" });
            tmplist.Add(new SelectListItem() { Text = "その他", Value = "その他" });
            
            return tmplist;
        }

        public string? 種類2 { get; set; }

        public static IList<SelectListItem> 種類2items()
        {
            var tmplist = new List<SelectListItem>();
            tmplist.Add(new SelectListItem() { Text = "空家", Value = "空家" });
            tmplist.Add(new SelectListItem() { Text = "分譲", Value = "分譲" });
            tmplist.Add(new SelectListItem() { Text = "経常", Value = "経常" });
            tmplist.Add(new SelectListItem() { Text = "小修理", Value = "小修理" });
            tmplist.Add(new SelectListItem() { Text = "長期", Value = "長期" });
            tmplist.Add(new SelectListItem() { Text = "その他", Value = "その他" });
            return tmplist;
        }


        public string? 見積区分 { get; set; }
        public static IList<SelectListItem> 見積区分items()
        {
            var tmplist = new List<SelectListItem>();
            tmplist.Add(new SelectListItem() { Text = "概算", Value = "概算" });
            tmplist.Add(new SelectListItem() { Text = "参考", Value = "参考" });
            tmplist.Add(new SelectListItem() { Text = "本見積", Value = "本見積" });
            return tmplist;
        }

        public string? 受渡場所 { get; set; }

        [StringLength(50, ErrorMessage = "支払条件は最大50文字までです。")]
        public string? 支払条件 { get; set; }
        public static IList<SelectListItem> 支払条件items()
        {
            var tmplist = new List<SelectListItem>();
            tmplist.Add(new SelectListItem() { Text = "従来通り", Value = "従来通り" });
            return tmplist;
        }

        public string? 有効期限 { get; set; } ="30日間";

        public static IList<SelectListItem> 有効期限items()
        {
            var tmplist = new List<SelectListItem>();
            tmplist.Add(new SelectListItem() { Text = "30日間", Value = "30日間" });
            tmplist.Add(new SelectListItem() { Text = "60日間", Value = "60日間" });
            tmplist.Add(new SelectListItem() { Text = "90日間", Value = "90日間" });
            return tmplist;
        }

        public string? 理化学医療区分 { get; set; }

        public static IList<SelectListItem> 理化学医療区分items()
        {
            var tmplist = new List<SelectListItem>();
            tmplist.Add(new SelectListItem() { Text = "理化学", Value = "理化学" });
            tmplist.Add(new SelectListItem() { Text = "医療", Value = "医療" });
            tmplist.Add(new SelectListItem() { Text = "両方", Value = "両方" });
            return tmplist;
        }
        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:#,##0.#}", ApplyFormatInEditMode = true)]
        public decimal? 見積金額 { get; set; }
        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:#,##0.#}", ApplyFormatInEditMode = true)]
        public decimal? 原価 { get; set; }
        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:#,##0.#}", ApplyFormatInEditMode = true)]
        public decimal? 見込原価 { get; set; }
        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:#,##0.#}", ApplyFormatInEditMode = true)]
        public decimal? 利益 { get; set; }
        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:#,##0.#}", ApplyFormatInEditMode = true)]
        public decimal? 見込利益 { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.0%}", ApplyFormatInEditMode = true)]
        public string? 粗利率 { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.0%}", ApplyFormatInEditMode = true)]
        public string? 見込粗利率 { get; set; }
        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:#,##0}", ApplyFormatInEditMode = true)]
        public int? 売上 { get; set; }


        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? 作成日 { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? 完了予定日 { get; set; }

        
        public string? 受注確度 { get; set; }
        public static IList<SelectListItem> 受注確度items()
        {
            var tmplist = new List<SelectListItem>();
            tmplist.Add(new SelectListItem() { Text = "100%", Value = "100%" });
            tmplist.Add(new SelectListItem() { Text = "99%", Value = "99%" });
            tmplist.Add(new SelectListItem() { Text = "70%", Value = "70%" });
            tmplist.Add(new SelectListItem() { Text = "50%", Value = "50%" });
            tmplist.Add(new SelectListItem() { Text = "49%以下", Value = "49%以下" });
            return tmplist;
        }

        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? time_stamp { get; set; }


        public string? 非課税名称 { get; set; }

        public decimal? 非課税額 { get; set; }

        public string? 値引名称 { get; set; }

        public decimal? 値引額 { get; set; }

        public string? 備考 { get; set; }

        public bool single { get; set; } = false;

        public bool 原価確認FLG { get; set; } = false;

        public string? 入金種別 { get; set; }
        public static IList<SelectListItem> 入金種別items()
        {
            var tmplist = new List<SelectListItem>();
            tmplist.Add(new SelectListItem() { Text = "現金", Value = "現金" });
            tmplist.Add(new SelectListItem() { Text = "カード", Value = "カード" });
            tmplist.Add(new SelectListItem() { Text = "小切手", Value = "小切手" });
            tmplist.Add(new SelectListItem() { Text = "手形", Value = "手形" });
            return tmplist;
        }

        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? 入金日 { get; set; }


        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? 入金締日 { get; set; }


        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? 取引年月日 { get; set; }

        public string? 見積ステータス { get; set; } = "見積";

        public static IList<SelectListItem> 見積ステータスitems()
        {
            var tmplist = new List<SelectListItem>();
            tmplist.Add(new SelectListItem() { Text = "見積", Value = "見積" });
            tmplist.Add(new SelectListItem() { Text = "請求", Value = "請求" });
            //tmplist.Add(new SelectListItem() { Text = "確定", Value = "確定" });
            tmplist.Add(new SelectListItem() { Text = "失注", Value = "失注" });            
            return tmplist;
        }

        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? 請求日 { get; set; }


        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? 最終入金日 { get; set; }


        public int? 入金合計 { get; set; }

        public string? 注文摘要 { get; set; }

        public string? 納付状況 { get; set; }

        public string? JS番号 { get; set; }

        public string? 部屋番号 { get; set; }

        public List<見積明細>? 見積明細リスト { get; set; }

        public List<見積分類表示順>? 見積分類表示順リスト { get; set; }

        public static IList<SelectListItem> 賃貸分譲items()
        {
            var tmplist = new List<SelectListItem>();
            tmplist.Add(new SelectListItem() { Text = "賃貸", Value = "賃貸" });
            tmplist.Add(new SelectListItem() { Text = "分譲", Value = "分譲" });
            return tmplist;
        }
    }
}
