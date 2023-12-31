using X.PagedList;
using rehome.Enums;
using rehome.Models.DB;

namespace rehome.Models
{
    public class PayCreateModel
    {
        public ViewMode Mode { get; set; } = ViewMode.New;
        public string BackUrl { get; set; }
        public 仕入帳 Pay { get; set; }

        public bool auth { get; set; }
               

        public IList<DropDownListModel>? 担当DropDownList { get; set; }

        

    }
}
