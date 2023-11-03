using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace rehome.Models.DB
{
    public class 分類
    {
        [Key]
        public int 分類ID { get; set; }

        [Required(ErrorMessage = "{0}を入力してください。")]
        public string? 分類名 { get; set; }

        [Required(ErrorMessage = "{0}を入力してください。")]
        public string? 理化学医療区分 { get; set; }

        public static IList<SelectListItem> 理化学医療区分items()
        {
            var tmplist = new List<SelectListItem>();
            tmplist.Add(new SelectListItem() { Text = "理化学", Value = "理化学" });
            tmplist.Add(new SelectListItem() { Text = "医療", Value = "医療" });
            tmplist.Add(new SelectListItem() { Text = "両方", Value = "両方" });
            return tmplist;
        }
        [Required(ErrorMessage = "表示順を入力してください。")]
        public int? ソート { get; set; }


    }
}
