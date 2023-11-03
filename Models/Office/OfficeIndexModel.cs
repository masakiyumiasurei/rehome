
using rehome.Models.DB;
using X.PagedList;

namespace rehome.Models
{
    public class OfficeIndexModel
    {
        public OfficeSearchConditions OfficeSearchConditions { get; set; } = new OfficeSearchConditions();
        public IList<営業所>? Office { get; set; }      

    }
}
