using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;
using rehome.Enums;

namespace rehome.Models.DB
{
    
    public class 見積分類表示順
    {
        [Key]
        public int 見積ID { get; set; }
        [Key]
        public int 履歴番号 { get; set; }

        [Key]
        public int 分類ID { get; set; }

        public int 表示順 { get; set; }
        
        public string? 分類名 { get; set; }
    }
}
