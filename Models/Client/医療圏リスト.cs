using Microsoft.AspNetCore.Mvc.Rendering;

namespace rehome.Models.Client
{
    public  class 医療圏リスト
    {
        public static IList<SelectListItem> 医療圏comb()
        {
            var tmplist = new List<SelectListItem>();
            tmplist.Add(new SelectListItem() { Text = "豊中市", Value = "豊能" });
            tmplist.Add(new SelectListItem() { Text = "池田市", Value = "豊能" });
            tmplist.Add(new SelectListItem() { Text = "箕面市", Value = "豊能" });
            tmplist.Add(new SelectListItem() { Text = "吹田市", Value = "豊能" });
            tmplist.Add(new SelectListItem() { Text = "能勢市", Value = "豊能" });
            tmplist.Add(new SelectListItem() { Text = "豊能市", Value = "豊能" });

            tmplist.Add(new SelectListItem() { Text = "高槻市", Value = "三島" });
            tmplist.Add(new SelectListItem() { Text = "島本町", Value = "三島" });
            tmplist.Add(new SelectListItem() { Text = "茨木市", Value = "三島" });
            tmplist.Add(new SelectListItem() { Text = "摂津市", Value = "三島" });

            tmplist.Add(new SelectListItem() { Text = "守口市", Value = "北河内" });
            tmplist.Add(new SelectListItem() { Text = "寝屋川市", Value = "北河内" });
            tmplist.Add(new SelectListItem() { Text = "門真市", Value = "北河内" });
            tmplist.Add(new SelectListItem() { Text = "枚方市", Value = "北河内" });
            tmplist.Add(new SelectListItem() { Text = "交野市", Value = "北河内" });
            tmplist.Add(new SelectListItem() { Text = "大東市", Value = "北河内" });
            tmplist.Add(new SelectListItem() { Text = "四条畷市", Value = "北河内" });

            tmplist.Add(new SelectListItem() { Text = "東大阪市", Value = "中河内" });
            tmplist.Add(new SelectListItem() { Text = "八尾市", Value = "中河内" });
            tmplist.Add(new SelectListItem() { Text = "柏原市", Value = "中河内" });

            tmplist.Add(new SelectListItem() { Text = "富田林市", Value = "南河内" });
            tmplist.Add(new SelectListItem() { Text = "河内長野市", Value = "南河内" });
            tmplist.Add(new SelectListItem() { Text = "藤井寺市", Value = "南河内" });
            tmplist.Add(new SelectListItem() { Text = "松原市", Value = "南河内" });
            tmplist.Add(new SelectListItem() { Text = "羽曳野市", Value = "南河内" });
            tmplist.Add(new SelectListItem() { Text = "大阪狭山市", Value = "南河内" });
            tmplist.Add(new SelectListItem() { Text = "太子町", Value = "南河内" });
            tmplist.Add(new SelectListItem() { Text = "河南市", Value = "南河内" });
            tmplist.Add(new SelectListItem() { Text = "千早赤阪村", Value = "南河内" });

            tmplist.Add(new SelectListItem() { Text = "堺市", Value = "堺" });

            tmplist.Add(new SelectListItem() { Text = "和泉市", Value = "泉州" });
            tmplist.Add(new SelectListItem() { Text = "高石市", Value = "泉州" });
            tmplist.Add(new SelectListItem() { Text = "忠岡町", Value = "泉州" });
            tmplist.Add(new SelectListItem() { Text = "泉大津市", Value = "泉州" });
            tmplist.Add(new SelectListItem() { Text = "岸和田市", Value = "泉州" });
            tmplist.Add(new SelectListItem() { Text = "貝塚市", Value = "泉州" });
            tmplist.Add(new SelectListItem() { Text = "泉佐野市", Value = "泉州" });
            tmplist.Add(new SelectListItem() { Text = "泉南市", Value = "泉州" });
            tmplist.Add(new SelectListItem() { Text = "阪南市", Value = "泉州" });
            tmplist.Add(new SelectListItem() { Text = "熊取町", Value = "泉州" });
            tmplist.Add(new SelectListItem() { Text = "田尻町", Value = "泉州" });
            tmplist.Add(new SelectListItem() { Text = "岬町", Value = "泉州" });

            tmplist.Add(new SelectListItem() { Text = "大阪市北区", Value = "大阪北" });
            tmplist.Add(new SelectListItem() { Text = "大阪市都島区", Value = "大阪北" });
            tmplist.Add(new SelectListItem() { Text = "大阪市淀川区", Value = "大阪北" });
            tmplist.Add(new SelectListItem() { Text = "大阪市東淀川区", Value = "大阪北" });
            tmplist.Add(new SelectListItem() { Text = "大阪市旭区", Value = "大阪北" });

            tmplist.Add(new SelectListItem() { Text = "大阪市福島区", Value = "大阪西" });
            tmplist.Add(new SelectListItem() { Text = "大阪市此花区", Value = "大阪西" });
            tmplist.Add(new SelectListItem() { Text = "大阪市大正区", Value = "大阪西" });
            tmplist.Add(new SelectListItem() { Text = "大阪市西区", Value = "大阪西" });
            tmplist.Add(new SelectListItem() { Text = "大阪市港区", Value = "大阪西" });
            tmplist.Add(new SelectListItem() { Text = "大阪市西淀川区", Value = "大阪西" });

            tmplist.Add(new SelectListItem() { Text = "大阪市中央区", Value = "大阪東" });
            tmplist.Add(new SelectListItem() { Text = "大阪市天王寺区", Value = "大阪東" });
            tmplist.Add(new SelectListItem() { Text = "大阪市浪速区", Value = "大阪東" });
            tmplist.Add(new SelectListItem() { Text = "大阪市東成区", Value = "大阪東" });
            tmplist.Add(new SelectListItem() { Text = "大阪市生野区", Value = "大阪東" });
            tmplist.Add(new SelectListItem() { Text = "大阪市城東区", Value = "大阪東" });
            tmplist.Add(new SelectListItem() { Text = "大阪市鶴見区", Value = "大阪東" });

            tmplist.Add(new SelectListItem() { Text = "大阪市阿倍野区", Value = "大阪南" });
            tmplist.Add(new SelectListItem() { Text = "大阪市住之江区", Value = "大阪南" });
            tmplist.Add(new SelectListItem() { Text = "大阪市住吉区", Value = "大阪南" });
            tmplist.Add(new SelectListItem() { Text = "大阪市東住吉区", Value = "大阪南" });
            tmplist.Add(new SelectListItem() { Text = "大阪市平野区", Value = "大阪南" });
            tmplist.Add(new SelectListItem() { Text = "大阪市西成区", Value = "大阪南" });
            return tmplist;
        }


        public static IList<SelectListItem> 医療圏items()
        {
            var tmplist = new List<SelectListItem>();
            tmplist.Add(new SelectListItem() { Text = "豊能", Value = "豊能" });
            tmplist.Add(new SelectListItem() { Text = "三島", Value = "三島" });
            tmplist.Add(new SelectListItem() { Text = "北河内", Value = "北河内" });
            tmplist.Add(new SelectListItem() { Text = "中河内", Value = "中河内" });
            tmplist.Add(new SelectListItem() { Text = "南河内", Value = "南河内" });
            tmplist.Add(new SelectListItem() { Text = "堺", Value = "堺" });
            tmplist.Add(new SelectListItem() { Text = "泉州", Value = "泉州" });
            tmplist.Add(new SelectListItem() { Text = "大阪北", Value = "大阪北" });      
            tmplist.Add(new SelectListItem() { Text = "大阪西", Value = "大阪西" });         
            tmplist.Add(new SelectListItem() { Text = "大阪東", Value = "大阪東" });
            tmplist.Add(new SelectListItem() { Text = "大阪南", Value = "大阪南" });
            return tmplist;
        }

    }
}
