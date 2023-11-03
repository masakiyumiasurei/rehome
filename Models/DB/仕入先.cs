using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace rehome.Models.DB
{
    [Table("T_仕入先")]
    public class 仕入先
    {
        [Key]
        public short 仕入先ID { get; set; }
        [Required(ErrorMessage = "{0}を入力してください。")]
        public string? 仕入先名 { get; set; }

        [RegularExpression(@"\d{3}-\d{4}", ErrorMessage = "正しい郵便番号ではありません。")]
        public string? 郵便番号 { get; set; }

        public string? 住所 { get; set; }

        public string? 肩書 { get; set; }

        public string? TEL { get; set; }

        public string? FAX { get; set; }
    }
}