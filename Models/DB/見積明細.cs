using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rehome.Models.DB
{

    public class 見積明細
    {
        [Key]
        public int 見積ID { get; set; }
        [Key]
        public int 履歴番号 { get; set; }
        [Key]
        public int 分類ID { get; set; }
        [Key]
        public int 連番 { get; set; }

        public int 商品ID { get; set; }
        public string? 商品名 { get; set; }

        public int? 内訳数1 { get; set; }

        public string? 内訳単位1 { get; set; }
        public int? 内訳数2 { get; set; }

        public string? 内訳単位2 { get; set; }

        public int? 数量 { get; set; }

        public string? 単位 { get; set; }
        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:#,##0.#}", ApplyFormatInEditMode = true)]
        public decimal? 単価 { get; set; }

        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:#,##0.#}", ApplyFormatInEditMode = true)]
        public decimal? 金額 { get; set; }

        public string? 分類名 { get; set; }

        public bool 削除FLG { get; set; }

        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:#,##0.#}", ApplyFormatInEditMode = true)]
        public decimal? 見込原価 { get; set; }

        public string? 備考 { get; set; }
    }
}
