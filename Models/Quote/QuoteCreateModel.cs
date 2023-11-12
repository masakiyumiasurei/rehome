using X.PagedList;
using rehome.Enums;
using rehome.Models.DB;

namespace rehome.Models
{
    public class QuoteCreateModel
    {
        public ViewMode Mode { get; set; } = ViewMode.New;
        public string BackUrl { get; set; }
        public 見積 Quote { get; set; }

        public bool auth { get; set; }

        public IList<DropDownListModel>? 分類DropDownList { get; set; }

        public int RowCount { get; set; }

        public IList<DropDownListModel>? 自由分類DropDownList { get; set; }

        public IList<DropDownListModel>? 担当DropDownList { get; set; }

        public IList<DropDownListModel>? 営業所DropDownList { get; set; }        

    }
}
