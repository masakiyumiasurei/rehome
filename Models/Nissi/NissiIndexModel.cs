
using rehome.Models.DB;
using rehome.Models.Nissi;
using X.PagedList;

namespace rehome.Models
{
    public class NissiIndexModel
    {
        public NissiSearchConditions NissiSearchConditions { get; set; } = new NissiSearchConditions();
        public IList<日誌表示>? Nissis { get; set; }
        public IList<DropDownListModel>? 担当DropDownList { get; set; }        

        public int PageNo { get; set; }
        public string mess { get; set; }

        public string 印刷区分 { get; set; }

    }
}
