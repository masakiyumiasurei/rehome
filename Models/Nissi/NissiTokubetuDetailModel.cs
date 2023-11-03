using rehome.Enums;
using rehome.Models.DB;

namespace rehome.Models
{
    public class NissiTokubetuDetailModel
    {
        public ViewMode Mode { get; set; } = ViewMode.View;

        public string BackUrl { get; set; }

        public string? 顧客名 { get; set; }

        public 日誌 Nissi { get; set; }
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
        
        public IList<DropDownListModel>? 担当DropDownList { get; set; }

        public IList<DropDownListModel>? 相談者DropDownList { get; set; }

        public IList<DropDownListModel>? 顧客DropDownList { get; set; }

    }
}
