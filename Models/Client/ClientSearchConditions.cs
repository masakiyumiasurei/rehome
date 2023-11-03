using System.ComponentModel.DataAnnotations;
using rehome.Enums;

namespace rehome.Models
{
    public class ClientSearchConditions
    {

        public string? 顧客名 { get; set; }
        public string? 郵便番号 { get; set; }

        public string? 住所 { get; set; }

        public string? TEL { get; set; }

        public string? カナ { get; set; }

    }
}
