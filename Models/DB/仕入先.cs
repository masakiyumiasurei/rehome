using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;
using rehome.Enums;

namespace rehome.Models.DB
{
    [Table("T_仕入先")]
    public class 仕入先
    {
        [Key]
        public short 仕入先ID { get; set; }
        [Required(ErrorMessage = "{0}を入力してください。")]
        public string? 仕入先名 { get; set; }

        [RegularExpression(@"\d{3}-\d{4}", ErrorMessage = "正しい郵便番号ではありません。")]
        public string? 郵便番号 { get; set; }

        public string? 住所 { get; set; }

        public string? 肩書 { get; set; }

        public string? TEL { get; set; }

        public string? FAX { get; set; }
        public string? 銀行名 { get; set; }
        public string? 支店名 { get; set; }
        public string? 口座区分 { get; set; }
        public string? 口座番号 { get; set; }
        public string? 口座名義 { get; set; }
        public string? インボイス番号 { get; set; }
        public 業種? 業種 { get; set; }
        public string? 支払日 { get; set; }

        public static IList<SelectListItem> 支払日items()
        {
            var tmplist = new List<SelectListItem>();
            tmplist.Add(new SelectListItem() { Text = "25日", Value = "25日" });
            tmplist.Add(new SelectListItem() { Text = "末締", Value = "末締" });
            return tmplist;
        }
        public 仕入分類 ? 分類 { get; set; }
    }
}