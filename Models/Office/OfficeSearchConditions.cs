using System.ComponentModel.DataAnnotations;
namespace rehome.Models
{
    public class OfficeSearchConditions
    {
        public string? 営業所名 { get; set; }
        public string? 郵便番号 { get; set; }

        public string? 住所 { get; set; }

        public string? 肩書 { get; set; }

        public string? TEL { get; set; }
        public string? FAX { get; set; }

    }
}
