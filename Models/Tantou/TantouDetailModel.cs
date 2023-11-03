using rehome.Enums;
using rehome.Models.DB;

namespace rehome.Models
{
    public class TantouDetailModel
    {
        public ViewMode Mode { get; set; } = ViewMode.View;

        public string BackUrl { get; set; }
        public 担当 Tantou { get; set; }

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

        public IList<担当営業所>? 担当営業所リスト { get; set; }
        
        public int? 担当営業所数 { get; set; }

        public int? 担当営業所ID新規 { get; set; }

        public IList<DropDownListModel>? 営業所DropDownList { get; set; }

    }
}
