using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;
using rehome.Enums;

namespace rehome.Models.DB
{
    public class 法人
    {
        [Key]
        [Required(ErrorMessage = "{0}を入力してください。")]
        public string? 社名 { get; set; }
        public string? 代表名 { get; set; }        
        public string? 郵便番号 { get; set; } 
        public string? 住所 { get; set; }
        public string? 仕入先ID { get; set; }

        public string? TEL { get; set; }

         public string? FAX { get; set; }

         public string? オフィス { get; set; } 

    }
}
