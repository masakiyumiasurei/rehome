using rehome.Enums;
using rehome.Models.DB;

namespace rehome.Models
{
    public class SiireDetailModel
    {
        public ViewMode Mode { get; set; } = ViewMode.View;

        public string BackUrl { get; set; }
        public 仕入先 Siire { get; set; }

        public object GetHtmlAtt(string idName)
        {
            if (Mode == ViewMode.View)
            {
                return new { id = idName, @readonly = "" };
            }
            else
            {
                return new { id = idName };
            }
        }




    }
}
