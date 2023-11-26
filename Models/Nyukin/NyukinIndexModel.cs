using rehome.Models.DB;
using X.PagedList;
namespace rehome.Models
{
    public class NyukinIndexModel
    {
        public NyukinSearchConditions? NyukinSearchConditions { get; set; } = new NyukinSearchConditions();
        public IList<見積>? Quotes { get; set; }

        public IList<入金>? Nyukins { get; set; }        
        public IList<DropDownListModel>? 担当DropDownList { get; set; }

        //Nyukindetailに渡すため
        public int 見積ID { get; set; }

        public int 履歴番号 { get; set; }
    }
}
