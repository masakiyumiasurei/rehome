using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rehome.Models.DB
{
    [Table("T_商品")]
    
    //[Index("中分類ID", Name = "IX_T_商品_中分類ID")]
    //[Index("大分類ID", Name = "IX_T_商品_大分類ID")]
    //[Index("小分類ID", Name = "IX_T_商品_小分類ID")]
    //[Index("大分類ID", "中分類ID", "小分類ID", Name = "分類")]
    public class 商品
    {             
        [Key]
        public int 商品ID { get; set; }
                
        [StringLength(200)]
        [Required(ErrorMessage = "{0}を入力してください。")] // null 以外を強制できる
        public string? 商品名 { get; set; }
        
        [Column(TypeName = "money")]
        public int? 単価 { get; set; }

        [StringLength(4)]
        public string? 単位 { get; set; }
        
        public byte? 税区分 { get; set; }
        public bool? 非課税 { get; set; }


        [Column(TypeName = "money")]
        public int? 仕入額 { get; set; }

        [Range(0, 100, ErrorMessage = "仕入掛率は0から100の範囲で入力してください。")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "仕入掛率は小数第2位まで入力してください。")]
        public decimal? 仕入掛率 { get; set; }

        [StringLength(500)]
        public string? 備考 { get; set; }
        public string? 表示カタログ { get; set; }
        public int? カタログ { get; set; }

        [Column(TypeName = "money")]
        public int? 保証 { get; set; }

        [Column(TypeName = "money")]
        public int? 推奨売価 { get; set; }

        [Column(TypeName = "money")]
        public int? 仕切価格 { get; set; }

        public bool openFLG { get; set; } = false;

        public string? 外部システムコード { get; set; }

        public bool 削除フラグ { get; set; } = false;

        public string ToCsvString()
        {
            // CSV形式の文字列を構築します
            // プロパティの値にカンマが含まれる場合にはダブルクォーテーションで囲みます
            return $"{商品ID}," +
                   $"\"{商品名}\","+
                   $"{仕入額},{仕入掛率},\"{備考}\",{カタログ},{openFLG},\"{外部システムコード}\",{削除フラグ}";
        }

    }
}
