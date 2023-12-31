using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rehome.Models
{
    public class PaySearchConditions
    {
        public string? 仕入先名 { get; set; }
              

        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? 日付start { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? 日付end { get; set; }

        
    }
}
