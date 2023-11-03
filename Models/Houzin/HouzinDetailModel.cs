using rehome.Enums;
using rehome.Models.DB;

namespace rehome.Models
{
    public class HouzinDetailModel
    {
        public ViewMode Mode { get; set; } = ViewMode.View;

        public string BackUrl { get; set; }
        public 法人 Houzin { get; set; }

       public 設定 Setting { get; set; }




    }
}
