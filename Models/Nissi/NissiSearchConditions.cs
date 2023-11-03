using System.ComponentModel.DataAnnotations;
namespace rehome.Models
{
    public class NissiSearchConditions
    {
        public int? 日誌ID { get; set; }
        public int? 顧客ID { get; set; }
        public int? 相談者ID { get; set; }
        public int? 担当ID { get; set; }

        [StringLength(50)]
        public string? 相談手段 { get; set; }

        [DataType(DataType.Date)]
        public DateTime? 対応日 { get; set; }

        [DataType(DataType.Date)]
        public DateTime? 登録日 { get; set; }
        
        public string? 相談内容_運営状況 { get; set; }
        public string? 相談内容_質問内容 { get; set; }
        public string? 対応内容 { get; set; }
        public string? 支援種別 { get; set; }

        public string? 支援区分 { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = false)]
        [DataType(DataType.Date)]        
        public DateTime? 対応日_start { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = false)]
        [DataType(DataType.Date)]
        public DateTime? 対応日_end { get; set; }

        public string? 顧客名 { get; set; }
        public string? 担当名 { get; set; }

        public string? 相談内容区分 { get; set; }

        public int? page { get; set; }

         
    }
}
