using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace rehome.Models.DB
{
    public class 担当営業所
    {
   
        public int 営業所ID { get; set; }

        public string? 営業所名 { get; set; }


        public int 担当ID { get; set; }


   

    }
}
