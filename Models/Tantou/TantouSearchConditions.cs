using System.ComponentModel.DataAnnotations;
namespace rehome.Models
{
    public class TantouSearchConditions
    {
        public string? 氏名 { get; set; }

        public int? 営業所ID { get; set; }

        public string? tel { get; set; }

        public bool 退職者検索 { get; set; }
    }
}
