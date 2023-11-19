using rehome.Enums;
using rehome.Models.DB;

namespace rehome.Models
{
    public class SyouhinDetailModel
    {
        public SyouhinSearchConditions AjaxSearchConditions { get; set; } = new SyouhinSearchConditions();
        public ViewMode Mode { get; set; } = ViewMode.View;
        public string BackUrl { get; set; }

        public bool admin { get; set; }
        public bool auth { get; set; }
        public 商品 Syouhin { get; set; }
        public IList<商品>? ListSyouhins { get; set; }

        public IList<DropDownListModel>? 分類DropDownList { get; set; }
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
