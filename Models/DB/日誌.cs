using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using rehome.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace rehome.Models.DB
{
        public class 日誌
    {
        [Key]
        public int? 日誌ID { get; set; }
        public int? 顧客ID { get; set; }

        public int?  担当ID { get; set; }
        public List<int>? 相談者ID { get; set; }

        [StringLength(2000)]
        public string? 内容 { get; set; }

        public string? カレンダー表示 { get; set; }

        public string? 日誌区分 { get; set; }

        public string? 担当名 { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = false)]
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? 対応日 { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = false)]
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? 登録日 { get; set; }

        [StringLength(50)]
        public string? 相談内容区分1 { get; set; }
        [StringLength(50)]
        public string? 相談内容区分2 { get; set; }

        [StringLength(2000)]
        public string? 相談内容_運営状況 { get; set; }

        [StringLength(2000)]
        public string? 相談内容_質問内容 { get; set; }

        [StringLength(2000)]
        public string? 対応内容 { get; set; }

        [Required(ErrorMessage = "{0}を入力してください。")]
        public string? 支援種別 { get; set; }

        [StringLength(50)]
        public string? 業務区分 { get; set; } = "相談対応業務";

        public string? 顧客名 { get; set; }
       

        public static IList<SelectListItem> 業務区分items()
        {
            var tmplist = new List<SelectListItem>();
            tmplist.Add(new SelectListItem() { Text = "現調", Value = "現調" });
            tmplist.Add(new SelectListItem() { Text = "工事", Value = "工事" });
            tmplist.Add(new SelectListItem() { Text = "引取", Value = "引取" });
            tmplist.Add(new SelectListItem() { Text = "納品", Value = "納品" });
            tmplist.Add(new SelectListItem() { Text = "要連絡", Value = "要連絡" });
            tmplist.Add(new SelectListItem() { Text = "その他", Value = "その他" });

            return tmplist;
        }
        public string? 備考 { get; set; }

        [StringLength(50)]
        public string? 相談手段 { get; set; }

        public TimeSpan? 時刻 { get; set; }

        //public static IList<SelectListItem> 相談手段items()
        //{
        //    var tmplist = new List<SelectListItem>();
        //    tmplist.Add(new SelectListItem() { Text = "電話", Value = "電話" });
        //    tmplist.Add(new SelectListItem() { Text = "来所", Value = "来所" });
        //    tmplist.Add(new SelectListItem() { Text = "事業所訪問", Value = "事業所訪問" });
        //    tmplist.Add(new SelectListItem() { Text = "センター架電", Value = "センター架電" });
        //    tmplist.Add(new SelectListItem() { Text = "その他", Value = "その他" });
        //    return tmplist;
        //}

    }
}
