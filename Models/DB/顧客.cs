using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace rehome.Models.DB
{
    public class 顧客
    {
        [Key]
        public int 顧客ID { get; set; }
        [StringLength(50)]

        [Required(ErrorMessage = "{0}を入力してください。")]
        public string? 顧客名 { get; set; }
        public string? 郵便番号 { get; set; }

        public string? 住所1 { get; set; }

        public string? 住所2 { get; set; }
        public string? 肩書 { get; set; }

        public string? TEL { get; set; }
        public string? FAX { get; set; }

        [EmailAddress(ErrorMessage = "有効なメールアドレスを入力してください。")]
        public string? メールアドレス { get; set; }

        public string? カナ { get; set; }

        public string? 紹介者 { get; set; }

        public string? 備考 { get; set; }
    }
}
