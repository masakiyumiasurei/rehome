using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;
using rehome.Enums;

namespace rehome.Models.DB
{
    
    public class Export仕入帳
    {        
                
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

        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:#,##0.#}", ApplyFormatInEditMode = true)]
        public decimal? 合計 { get; set; }


        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:#,##0.#}", ApplyFormatInEditMode = true)]
        public decimal? 振出額 { get; set; }
        
        public string? 備考 { get; set; }

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
        
        public 仕入分類? 分類 { get; set; }

        public string ToCsvString()
        {
            // CSV形式の文字列を構築します
            // プロパティの値にカンマが含まれる場合にはダブルクォーテーションで囲む
            return $"{支払日},\"{仕入先名}\",{インボイス番号},{分類},{金額},{消費税},{交通費},{値引等}," +
                   $"{合計},{振出額},{相手負担},{当社負担},{銀行名},{支店名},{口座区分},{口座番号},{口座名義}";
        }



    }
}
