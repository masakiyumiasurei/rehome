using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;
using rehome.Enums;

namespace rehome.Models.DB
{
    
    public class 入金
    {        
        
        [Key]
        public int 入金ID { get; set; }

        public int 見積ID { get; set; }

        public int 履歴番号 { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? 入金日 { get; set; } 

        [Required(ErrorMessage = "{0}を入力してください。")]
        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:#,##0.#}", ApplyFormatInEditMode = true)]
        public decimal? 入金額 { get; set; }


        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:#,##0.#}", ApplyFormatInEditMode = true)]
        public decimal? 振込手数料 { get; set; }

        public string? 振込名義 { get; set; }

        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:#,##0.#}", ApplyFormatInEditMode = true)]
        public decimal? 前受金 { get; set; }

        public 入金種別?　入金種別 { get; set; }


        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? 登録日 { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? time_stamp { get; set; }
        
        public string? 備考 { get; set; }

  
        //public static IList<SelectListItem> 入金種別items()
        //{
        //    var tmplist = new List<SelectListItem>();
        //    tmplist.Add(new SelectListItem() { Text = "現金", Value = "現金" });
        //    tmplist.Add(new SelectListItem() { Text = "カード", Value = "カード" });
        //    tmplist.Add(new SelectListItem() { Text = "小切手", Value = "小切手" });
        //    tmplist.Add(new SelectListItem() { Text = "手形", Value = "手形" });
        //    return tmplist;
        //}
 
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
