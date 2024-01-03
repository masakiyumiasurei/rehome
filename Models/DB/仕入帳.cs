using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;
using rehome.Enums;

namespace rehome.Models.DB
{
    
    public class 仕入帳
    {        
        
        [Key]
        public int 仕入ID { get; set; }

        public int 仕入先ID { get; set; }

        public string? 仕入先名 { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? 日付 { get; set; } 

        [Required(ErrorMessage = "{0}を入力してください。")]
        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:#,##0.#}", ApplyFormatInEditMode = true)]
        public decimal? 金額 { get; set; }


        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:#,##0.#}", ApplyFormatInEditMode = true)]
        public decimal? 消費税 { get; set; }
        

        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:#,##0.#}", ApplyFormatInEditMode = true)]
        public decimal? 交通費 { get; set; }

        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:#,##0.#}", ApplyFormatInEditMode = true)]
        public decimal? 値引等 { get; set; }


        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:#,##0.#}", ApplyFormatInEditMode = true)]
        public decimal? 相手負担 { get; set; }

        
        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:#,##0.#}", ApplyFormatInEditMode = true)]
        public decimal? 当社負担 { get; set; }

        public string? 備考 { get; set; }

        public int 担当ID { get; set; }
        public static IList<SelectListItem> 見積ステータスitems()
        {
            var tmplist = new List<SelectListItem>();
            tmplist.Add(new SelectListItem() { Text = "請求", Value = "請求" });
            tmplist.Add(new SelectListItem() { Text = "確定", Value = "確定" });
            tmplist.Add(new SelectListItem() { Text = "見積", Value = "見積" });            
            tmplist.Add(new SelectListItem() { Text = "失注", Value = "失注" });            
            return tmplist;
        }
        
    }
}
