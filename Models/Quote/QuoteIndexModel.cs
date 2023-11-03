using rehome.Models.DB;
using X.PagedList;
namespace rehome.Models
{
    public class QuoteIndexModel
    {
        public QuoteSearchConditions? QuoteSearchConditions { get; set; } = new QuoteSearchConditions();
        public IList<見積>? Quotes { get; set; }
        public IList<DropDownListModel>? 営業所DropDownList { get; set; }

        public IList<DropDownListModel>? 担当DropDownList { get; set; }
    }
}
