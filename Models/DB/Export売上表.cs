using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;
using rehome.Enums;

namespace rehome.Models.DB{
    
    public class Export売上表
    {
        public string? 計上月 { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? 請求日 { get; set; }

        public string? 担当 { get; set; }

        public string? 種類 { get; set; }

        public string? 現場名 { get; set; }

        public string? 顧客名 { get; set; }
  
        public string? 件名 { get; set; }

        public string? 納期 { get; set; }

        public string? 項目 { get; set; }
    
        public string? 種類2 { get; set; }

        public string? 見積区分 { get; set; }

        public string? 支払条件 { get; set; }
   
        public string? 有効期限 { get; set; }


        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:#,##0.#}", ApplyFormatInEditMode = true)]
        public decimal? 見積金額 { get; set; }
        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:#,##0.#}", ApplyFormatInEditMode = true)]
        public decimal? 原価 { get; set; }
        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:#,##0.#}", ApplyFormatInEditMode = true)]
        public decimal? 見込原価 { get; set; }
        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:#,##0.#}", ApplyFormatInEditMode = true)]
        public decimal? 利益 { get; set; }
        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:#,##0.#}", ApplyFormatInEditMode = true)]
        public decimal? 見込利益 { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.0%}", ApplyFormatInEditMode = true)]
        public string? 粗利率 { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.0%}", ApplyFormatInEditMode = true)]
        public string? 見込粗利率 { get; set; }
        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:#,##0}", ApplyFormatInEditMode = true)]
        public int? 売上 { get; set; }


        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? 作成日 { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? 完了予定日 { get; set; }

        public string? 受注確度 { get; set; }


        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? time_stamp { get; set; } 

        public string? 値引名称 { get; set; }

        public decimal? 値引額 { get; set; }

        public string? 備考 { get; set; }



        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? 入金日 { get; set; }


        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? 入金締日 { get; set; }


        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? 取引年月日 { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? 最終入金日 { get; set; }


        public int? 入金額 { get; set; }

        public int? 振込手数料 { get; set; }

        public string? 部屋番号 { get; set; }

        public string? 振込名義 { get; set; }

        public int? 前受金 { get; set; }

        public string? 入金種別 { get; set; }
        public string? JS番号 { get; set; }

        public string ToCsvString()
        {
            //CSV形式の文字列を構築します
            //プロパティの値にカンマが含まれる場合にはダブルクォーテーションで囲む
            return $"{計上月},{請求日},{担当},{種類},\"{現場名}\",{部屋番号},{種類2},\"{JS番号}\"," +
                   $"\"{件名}\",{入金日},{入金額},{振込手数料},{振込名義},{前受金},\"{備考}\",{項目},{入金種別}";
            
        }



    }
}
