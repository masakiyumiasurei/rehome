using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using rehome.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace rehome.Models.DB
{

    public class 担当
    {
        [Key]
        public int 担当ID { get; set; }
        [Required(ErrorMessage = "担当名を入力してください。")]
        public string? 氏名 { get; set; }

        public int? 営業所ID { get; set; }

        public string? 営業所名 { get; set; }
        public bool auth { get; set; }
        [Required(ErrorMessage = "ログインIDを入力してください。")]
        public string? loginID { get; set; }

        [Required(ErrorMessage = "パスワードを入力してください。")]
        public string? pass { get; set; }

        public string? tel { get; set; }

        [StringLength(3)]
        [Required(ErrorMessage = "{0}を入力してください。")]
        public string? イニシャル { get; set; }

        public string? new_pass { get; set; }

        public bool admin { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = false)]
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? del_date { get; set; }

        public bool assistant { get; set; }
    }
}
