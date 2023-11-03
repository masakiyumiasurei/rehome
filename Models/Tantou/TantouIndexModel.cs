
using rehome.Models.DB;
using X.PagedList;

namespace rehome.Models
{
    public class TantouIndexModel
    {
        public TantouSearchConditions TantouSearchConditions { get; set; } = new TantouSearchConditions();
        public IList<担当>? Tantous { get; set; }
        public IList<DropDownListModel>? 担当DropDownList { get; set; }

        public IList<DropDownListModel>? 営業所DropDownList { get; set; }

    }
}
