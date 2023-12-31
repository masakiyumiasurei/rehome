using rehome.Enums;
using System.ComponentModel.DataAnnotations;
namespace rehome.Models
{
    public class SiireSearchConditions
    {
        public string? 仕入先名 { get; set; }

        public string? 郵便番号 { get; set; }

        public string? 住所 { get; set; }

        public string? 肩書 { get; set; }

        public string? TEL { get; set; }

        public string? FAX { get; set; }

        public 業種? 業種 { get; set; }

        public 仕入分類? 分類 { get; set; }

    }
}
