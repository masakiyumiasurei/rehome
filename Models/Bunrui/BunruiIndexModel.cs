
using rehome.Models.DB;
using X.PagedList;

namespace rehome.Models
{
    public class BunruiIndexModel
    {
        public BunruiSearchConditions BunruiSearchConditions { get; set; } = new BunruiSearchConditions();
        public IList<分類>? Bunrui { get; set; }      

    }
}
