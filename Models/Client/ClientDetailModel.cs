using rehome.Enums;
using rehome.Models.DB;
using rehome.Models.Nissi;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace rehome.Models
{
    public class ClientDetailModel
    {
        public string? BackUrl { get; set; }
        public ViewMode Mode { get; set; } = ViewMode.View;
        public 顧客 Client { get; set; }
        
        public ClientSearchConditions AjaxSearchConditions { get; set; } = new ClientSearchConditions();

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

        public IList<日誌>? Nissi { get; set; }

        public List<見積>? Quotes { get; set; }

        public IList<DropDownListModel>? 担当DropDownList { get; set; }

        
        public List<IFormFile>? PostedFile { get; set; }

       // public IList<Files>? Files { get; set; }

        public IList<DropDownListModel>? 相談者DropDownList { get; set; }

        public static IList<SelectListItem> yesnoitems()
        {
            var tmplist = new List<SelectListItem>();
            tmplist.Add(new SelectListItem() { Text = "はい", Value = "はい" });
            tmplist.Add(new SelectListItem() { Text = "いいえ", Value = "いいえ" });
            return tmplist;
        }

        public string? 支援区分フィルター { get; set; }

        public string? 担当名フィルター { get; set; }
        public string? 相談者名フィルター { get; set; }




    }
}
