using rehome.Models.DB;
using X.PagedList;
namespace rehome.Models
{
    public class PayIndexModel
    {
        public PaySearchConditions? PaySearchConditions { get; set; } = new PaySearchConditions();
        public IList<仕入帳>? Pays { get; set; }
        public IList<DropDownListModel>? 営業所DropDownList { get; set; }

        public IList<DropDownListModel>? 担当DropDownList { get; set; }
    }
}
