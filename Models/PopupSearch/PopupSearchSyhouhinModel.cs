using rehome.Enums;
using rehome.Models;
using rehome.Models.DB;

namespace rehome.Models
{
    public class PopupSearchSyouhinModel
    {
        public SyouhinSearchConditions SearchConditions { get; set; } = new SyouhinSearchConditions();

        public IList<商品>? SearchSyouhins { get; set; }

    }
}
