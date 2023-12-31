using rehome.Enums;
using rehome.Models;
using rehome.Models.DB;

namespace rehome.Models
{
    public class PopupSearchSiireModel
    {
        public SiireSearchConditions SearchConditions { get; set; } = new SiireSearchConditions();

        public IList<仕入先>? SearchSiires { get; set; }

    }
}
