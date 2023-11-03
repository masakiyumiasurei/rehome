using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace rehome.Models.DB
{
    public class 営業所
    {
        [Key]
        public int 営業所ID { get; set; }
        [StringLength(50)]

        [Required(ErrorMessage = "{0}を入力してください。")]
        public string? 営業所名 { get; set; }
        public string? 郵便番号 { get; set; }

        public string? 住所 { get; set; }

        public string? 肩書 { get; set; }

        public string? TEL { get; set; }
        public string? FAX { get; set; }


    }
}
