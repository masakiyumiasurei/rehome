using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using rehome.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using rehome.Models.DB;

namespace rehome.Models.Nissi
{
    public class 日誌表示
    {
        public int? 日誌ID { get; set; }

        public string? 支援区分 { get; set; }

        public static IList<SelectListItem> 支援区分items()
        {
            var tmplist = new List<SelectListItem>();
            tmplist.Add(new SelectListItem() { Text = "相談", Value = "相談" });
            tmplist.Add(new SelectListItem() { Text = "個別支援", Value = "個別支援" });
            tmplist.Add(new SelectListItem() { Text = "特別支援", Value = "特別支援" });
            return tmplist;
        }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = false)]
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? 対応日 { get; set; }

        public List<担当>? 担当リスト { get; set; }


        public string? 相談内容区分1 { get; set; }

        public string? 相談内容区分2 { get; set; }

        public string? 内容 { get; set; }

        public string? 内容2 { get; set; }

        public string? 内容3 { get; set; }

        public string? 顧客名 { get; set; }

        public string? 相談手段 { get; set; }
        public string? 相談内容_運営状況 { get; set; }

        public string? 対応内容 { get; set; }

        public string? 支援種別 { get; set; }

        public int? 連番 { get; set; }

        public int? 顧客ID { get; set; }


        public string? 備考 { get; set; }


        public string? 相談内容_質問内容 { get; set; }

        public string? 業務区分 { get; set; } 

        public decimal? 入院基本料 { get; set; }

        public string? 事前調整事項 { get; set; }
        public string? 対応者立場 { get; set; }
        public string? 対応者関係 { get; set; }
        public string? 対応者姿勢 { get; set; }
        public string? 対応者関心 { get; set; }
        public string? 訪問成果 { get; set; }
        public string? 支援課題 { get; set; }
        public string? 支援提案 { get; set; }
        public string? 個別支援特記事項 { get; set; }
        public string? 取組状況区分 { get; set; }

        public string? 取組状況 { get; set; }
        public string? 労働時間課題 { get; set; }
        public string? 医療機関感想 { get; set; }
        public string? その他特記事項 { get; set; }


        public string? PDCA { get; set; }

        public string? 実施場所 { get; set; }

        public string? 当日議題 { get; set; }

        public string? 資料医療機関 { get; set; }

        public string? 資料勤改センター { get; set; }

        public string? 課題対応医療機関 { get; set; }
        public string? 課題対応勤改センター { get; set; }
        public string? 支援反応 { get; set; }
        public string? 支援成果 { get; set; }
        public string? step1 { get; set; }
        public string? step2 { get; set; }


        public string? step3 { get; set; }
        public string? step4 { get; set; }

        public string? step5 { get; set; }

        public string? step6 { get; set; }

        public string? step7 { get; set; }

        public string? 特別支援特記事項 { get; set; }


        public string? todo医療機関 { get; set; }

        public string? todo勤改センター { get; set; }



        public string? 相談事項 { get; set; }

        public string? その他 { get; set; }
        public string? 次回訪問 { get; set; }




    }
}
