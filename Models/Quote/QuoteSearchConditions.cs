using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rehome.Models
{
    public class QuoteSearchConditions
    {
      public int? 営業所ID { get; set; }
      public int? 担当ID { get; set; }

        public string? 案件名 { get; set; }

        public string? 顧客名 { get; set; }

        public string? 受注確度 { get; set; }

        public string? 項目 { get; set; }

        public string? 見積番号 { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? 完了予定日start { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? 完了予定日end { get; set; }

        public int? LoginID { get; set; }

        public bool auth { get; set; }

        public bool assistant { get; set; }
    }
}
