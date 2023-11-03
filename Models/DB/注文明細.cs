using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace rehome.Models.DB
{
    public class 注文明細
    {
        [Key]
        public int 注文ID { get; set; }

        [Key]
        public int? 枝番 { get; set; }    

        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:#,##0.#}", ApplyFormatInEditMode = true)]
        public int? 単価 { get; set; }

        public int? 数量 { get; set; }

        public string? 件名 { get; set; }

        public string? 摘要 { get; set; }

        public int? 原価 { get; set; }

        public int? 見込原価 { get; set; }

        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:#,##0.#}", ApplyFormatInEditMode = true)]
        public int? 金額 { get; set; }

        public int? 原価計 { get; set; }

        public int? 見込原価計 { get; set; }

        public int? 金額計 { get; set; }
    }
}
