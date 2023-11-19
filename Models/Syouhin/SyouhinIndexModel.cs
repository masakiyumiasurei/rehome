
using rehome.Models.DB;
using X.PagedList;

namespace rehome.Models
{
    public class SyouhinIndexModel
    {
        public SyouhinSearchConditions SyouhinSearchConditions { get; set; } = new SyouhinSearchConditions();
        public IPagedList<商品>? Syouhins { get; set; }
        //public IList<商品> ListSyouhins { get; set; } = new List<商品>();
    }
}
