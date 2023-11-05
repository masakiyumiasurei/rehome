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
        
        int? 担当ID { get; set; }
        public List<int>? 相談者ID { get; set; }

        [StringLength(2000)]
        public string? 内容 { get; set; }

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
        public static IList<SelectListItem> 支援種別items()
        {
            var tmplist = new List<SelectListItem>();
            tmplist.Add(new SelectListItem() { Text = "来所相談", Value = "来所相談" });
            tmplist.Add(new SelectListItem() { Text = "特別支援", Value = "特別支援" });
            tmplist.Add(new SelectListItem() { Text = "同行支援", Value = "同行支援" });
            tmplist.Add(new SelectListItem() { Text = "個別支援", Value = "個別支援" });
            return tmplist;
        }

        public static IList<SelectListItem> 業務区分items()
        {
            var tmplist = new List<SelectListItem>();
            tmplist.Add(new SelectListItem() { Text = "相談対応業務", Value = "相談対応業務" });
            tmplist.Add(new SelectListItem() { Text = "利用勧奨業務", Value = "利用勧奨業務" });
            
            return tmplist;
        }
        public string? 備考 { get; set; }

        [StringLength(50)]
        public string? 相談手段 { get; set; }

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
