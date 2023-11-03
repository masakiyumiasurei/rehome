using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace rehome.Models.DB
{
    public class 設定
    {
        [Key]
        public int id { get; set; }

        [Required(ErrorMessage = "{0}を入力してください。")]
        public int? 期 { get; set; }



    }
}
