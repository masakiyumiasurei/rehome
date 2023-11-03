using X.PagedList;
using rehome.Enums;
using rehome.Models.DB;

namespace rehome.Models
{
    public class ChumonCreateModel
    {
        public ViewMode Mode { get; set; } = ViewMode.New;
        public string BackUrl { get; set; }
        public 注文 Chumon { get; set; }
        public 見積 Quote { get; set; }
        public IList<DropDownListModel>? 仕入先DropDownList { get; set; }
        public int RowCount { get; set; }
        public IList<注文明細>? ChumonMeisai { get; set; }

    }
}
