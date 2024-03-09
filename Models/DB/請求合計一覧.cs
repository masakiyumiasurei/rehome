using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;
using rehome.Enums;

namespace rehome.Models.DB
{
    
    public class 請求合計一覧
    {        
        public string? 項目 { get; set; }

        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:#,##0.#}", ApplyFormatInEditMode = true)]
        public decimal? 合計額 { get; set; }
        
    }
}
