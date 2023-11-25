using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rehome.Models
{
    public class NyukinSearchConditions
    {
 
      public int? 担当ID { get; set; }

        public string? 案件名 { get; set; }

        public string? 顧客名 { get; set; }

 
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? 請求日start { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? 請求日end { get; set; }

    }
}
