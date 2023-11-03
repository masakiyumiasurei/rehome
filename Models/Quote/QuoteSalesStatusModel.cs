using rehome.Models.DB;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using X.PagedList;
namespace rehome.Models
{
    public class QuoteSalesStatusModel
    {
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? start_date { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? end_date { get; set; }

        public int? 期 { get; set; }
        public List<SalesStatus>? 期オフィス別累計50 { get; set; }

        public List<SalesStatus>? 期オフィス別累計49 { get; set; }

        public List<SalesStatus>? 期間オフィス別累計 { get; set; }

        public List<SalesStatus>? 期個人別累計 { get; set; }

        public List<SalesStatus>? 期間個人別累計 { get; set; }

        public static IList<SelectListItem> 月items()
        {
            var tmplist = new List<SelectListItem>();
            tmplist.Add(new SelectListItem() { Text = "1", Value = "1" });
            tmplist.Add(new SelectListItem() { Text = "2", Value = "2" });
            tmplist.Add(new SelectListItem() { Text = "3", Value = "3" });
            tmplist.Add(new SelectListItem() { Text = "4", Value = "4" });
            tmplist.Add(new SelectListItem() { Text = "5", Value = "5" });
            tmplist.Add(new SelectListItem() { Text = "6", Value = "6" });
            tmplist.Add(new SelectListItem() { Text = "7", Value = "7" });
            tmplist.Add(new SelectListItem() { Text = "8", Value = "8" });
            tmplist.Add(new SelectListItem() { Text = "9", Value = "9" });
            tmplist.Add(new SelectListItem() { Text = "10", Value = "10" });
            tmplist.Add(new SelectListItem() { Text = "11", Value = "11" });
            tmplist.Add(new SelectListItem() { Text = "12", Value = "12" });
            return tmplist;
        }

    }
}
