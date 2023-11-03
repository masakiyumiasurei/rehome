using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;
using rehome.Enums;

namespace rehome.Models.DB
{
    public class 注文
    {
        [Key]
        public int 注文ID { get; set; }
        public int 見積ID { get; set; }        
        public int 履歴番号 { get; set; } 
        public int? 枝番 { get; set; }
        public int? 仕入先ID { get; set; }

        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:#,##0.#}", ApplyFormatInEditMode = true)]
        public int? 金額 { get; set; }

        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:#,##0.#}", ApplyFormatInEditMode = true)]
        public int? 原価 { get; set; }

        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:#,##0.#}", ApplyFormatInEditMode = true)]
        public int? 見込原価 { get; set; }

        public string? 納期 { get; set; }

        public string? 納入先 { get; set; }

        public string? 仕入先名 { get; set; }

        public string? 支払締め { get; set; }

        public string? 支払日 { get; set; }

        public string? 摘要 { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime 作成日 { get; set; } = DateTime.Now;

        public string? 弊社担当者 { get; set; }

        public string? 自由仕入先名 { get; set; }

        //public static IList<SelectListItem> 理化学医療区分items()
        //{
        //    var tmplist = new List<SelectListItem>();
        //    tmplist.Add(new SelectListItem() { Text = "理化学", Value = "理化学" });
        //    tmplist.Add(new SelectListItem() { Text = "医療", Value = "医療" });
        //    tmplist.Add(new SelectListItem() { Text = "両方", Value = "両方" });
        //    return tmplist;
        //}

    }
}
