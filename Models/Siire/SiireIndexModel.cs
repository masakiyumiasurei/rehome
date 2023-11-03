
using rehome.Models.DB;
using X.PagedList;

namespace rehome.Models
{
    public class SiireIndexModel
    {
        public SiireSearchConditions SiireSearchConditions { get; set; } = new SiireSearchConditions();
        public IList<仕入先>? Siire { get; set; }      

    }
}
