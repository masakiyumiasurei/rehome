using rehome.Models.DB;
using X.PagedList;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace rehome.Models
{
    public class ChumonIndexModel
    {
        public 見積 Quote { get; set; }
        public IList<注文>? Chumons { get; set; }
        public IList<DropDownListModel>? 仕入先DropDownList { get; set; }

        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:#,##0.#}", ApplyFormatInEditMode = true)]
        public int? 金額計 { get; set; }

        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:#,##0.#}", ApplyFormatInEditMode = true)]

        public int? 原価計 { get; set; }

        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:#,##0.#}", ApplyFormatInEditMode = true)]

        public int? 見込原価計 { get; set; }

        public string BackUrl { get; set; }

    }
}
