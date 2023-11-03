﻿using rehome.Enums;
using rehome.Models.DB;

namespace rehome.Models
{
    public class BunruiDetailModel
    {
        public ViewMode Mode { get; set; } = ViewMode.View;

        public string BackUrl { get; set; }
        public 分類 Bunrui { get; set; }

        public object GetHtmlAtt(string idName)
        {
            if (Mode == ViewMode.View)
            {
                return new { id = idName, @readonly = "" };
            }
            else
            {
                return new { id = idName };
            }
        }




    }
}
