using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using rehome.Enums;
using rehome.Models;
using rehome.Models.DB;
using X.PagedList;
using rehome.Calendar;

namespace rehome.Services
{
    public interface ICalendarService
    {
        IList<Event> GetCalendar(DateTime start, DateTime end);

    }

    public class CalendarService : ServiceBase, ICalendarService
    {
        private readonly ILogger<CalendarService> _logger;

        public CalendarService(ILogger<CalendarService> logger, IConfiguration configuration)
            : base(configuration)
        {
            _logger = logger;
       
        }
 

//**************************************** 一覧・検索 ********************************************************************

        public IList<Event> GetCalendar(DateTime start, DateTime end)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("select  顧客名 as Title, " +
                                            "対応日 as Start , " +
                                            "対応日 as [End], " +
                                            " '/Client/Detail?顧客ID=' + convert(nvarchar,顧客ID)  as url " +
                                            "from " +
                                             "(SELECT 対応日,RT_日誌.顧客ID,顧客名 FROM RT_日誌  " +
                                             "left join RT_顧客 B on RT_日誌.顧客ID = B.顧客ID　/**where**/ " +
                                             "group by 顧客名, 対応日, RT_日誌.顧客ID) " +
                                             " as T ");
                                             
                builder.Where("対応日 between @start and @end", new { start = start, end= end });

                var result = connection.Query<Event>(template.RawSql, template.Parameters).ToList();                
                
                return result;
            }
        }

 


     //   public List<日誌表示> GetNissis_Month(int? 年度,int? 月度)
     //   {
     //       using (var connection = new SqlConnection(_connectionString))
     //       {
     //           connection.Open();
     //           var builder = new SqlBuilder();
     //           var template = builder.AddTemplate("(SELECT '1' as 連番,日誌ID,T_日誌.顧客ID,'相談' as 支援区分,対応日,相談内容区分1,相談内容区分2,相談内容_質問内容 as 内容,担当ID FROM T_日誌 left join RT_顧客担当 on T_日誌.顧客ID = RT_顧客担当.顧客ID and RT_顧客担当.担当種別 = 'メイン'  /**where**/ and 相談手段 = '来所' ) " +
     //                                        "UNION (SELECT '2' as 連番,日誌ID,T_個別支援日誌.顧客ID,'個別支援' as 支援区分,対応日,相談内容区分1,相談内容区分2,訪問目的 as 内容,担当ID FROM T_個別支援日誌 left join RT_顧客担当 on T_個別支援日誌.顧客ID = RT_顧客担当.顧客ID and RT_顧客担当.担当種別 = 'メイン'  /**where**/ ) " +
     //                                        "UNION (SELECT '3' as 連番,日誌ID,T_特別支援日誌.顧客ID,'特別支援' as 支援区分,対応日,相談内容区分1,相談内容区分2,主な発言 as 内容,担当ID FROM T_特別支援日誌 left join RT_顧客担当 on T_特別支援日誌.顧客ID = RT_顧客担当.顧客ID and RT_顧客担当.担当種別 = 'メイン'  /**where**/ ) " +
     //                                        "order by 連番,対応日,担当ID");
     //           builder.Where("DATEPART(year,対応日) = @年度 and DATEPART(month,対応日) = @月度", new { 年度 = 年度,月度 = 月度 }) ;

     //           var result = connection.Query<日誌表示>(template.RawSql, template.Parameters).ToList();

     //           if (result != null)
     //           {
     //               foreach (var item in result)
     //               {
     //                   var builder2 = new SqlBuilder();
     //                   var template2 = builder2.AddTemplate("SELECT T_担当.* FROM T_日誌担当 INNER JOIN T_担当 on T_日誌担当.担当ID = T_担当.担当ID /**where**/ order by T_日誌担当.担当ID");
     //                   builder2.Where("T_日誌担当.日誌ID = @日誌ID and T_日誌担当.支援区分 = @支援区分", new { 日誌ID = item.日誌ID, 支援区分 = item.支援区分 });
     //                   item.担当リスト = connection.Query<担当>(template2.RawSql, template2.Parameters).ToList();


     //                   var builder3 = new SqlBuilder();
     //                   var template3 = builder3.AddTemplate("SELECT T_相談者.* FROM T_日誌相談者 INNER JOIN T_相談者 on T_日誌相談者.相談者ID = T_相談者.相談者ID /**where**/ order by T_日誌相談者.相談者ID");
     //                   builder3.Where("T_日誌相談者.日誌ID = @日誌ID and T_日誌相談者.支援区分 = @支援区分", new { 日誌ID = item.日誌ID, 支援区分 = item.支援区分 });
     //                   item.相談者リスト = connection.Query<相談者>(template3.RawSql, template3.Parameters).ToList();


     //               }
     //           }


     //           return result;
     //       }
     //   }

     //   public NissiIndexModel SearchNissis(NissiSearchConditions conditions)
     //   {
     //       using (var connection = new SqlConnection(_connectionString))
     //       {
     //           string WhereStr = "";
     //           bool joukenflg = false;
     //           connection.Open();
     //           var nissiIndexModel = new NissiIndexModel();
     //           var builder = new SqlBuilder();
     //           var Outer_builder = new SqlBuilder();
     //           //var template = builder.AddTemplate(
     //           var unionQuery = builder.AddTemplate(
     //"SELECT distinct * FROM " +
     //"(SELECT T_日誌.日誌ID, '相談' as 支援区分, 対応日, 相談内容区分1, 相談内容区分2, " +
     //"相談内容_質問内容 as 内容, B.顧客名, T_日誌.備考," +
     //"業務区分,  相談手段,相談内容_運営状況, 対応内容, " +
     //"'' as 入院基本料,'' as 事前調整事項,'' as 対応者立場,'' as 対応者関係,'' as 対応者姿勢,'' as 対応者関心,'' as 訪問成果,'' as 支援課題,'' as 支援提案,'' as 個別支援特記事項, " +
     //"'' as 取組状況区分,'' as 取組状況,'' as 労働時間課題,'' as 医療機関感想,'' as その他特記事項, " +
     //"'' as 実施場所,'' as 当日議題,'' as 資料医療機関,'' as 資料勤改センター,'' as 課題対応医療機関,'' as 課題対応勤改センター,'' as 支援反応,'' as 支援成果," +
     //"'' as PDCA,'' as step1,'' as step2,'' as step3,'' as step4,'' as step5,'' as step6,'' as step7,'' as 特別支援特記事項,'' as todo医療機関,'' as todo勤改センター,'' as 相談事項,'' as その他,'' as 次回訪問 " +
     //"FROM T_日誌  left join RT_顧客 B on T_日誌.顧客ID = b.顧客ID " +
     //"left join T_日誌担当 on T_日誌担当.日誌ID = T_日誌.日誌ID and T_日誌担当.支援区分 = '相談' " +
     //" /**where**/ " +

     //"UNION SELECT T_個別支援日誌.日誌ID, '個別支援' as 支援区分, 対応日, 相談内容区分1, 相談内容区分2, " +
     //"訪問目的 as 内容, B.顧客名,T_個別支援日誌.備考," +
     //"'' as 業務区分,'' as 相談手段, '' as 相談内容_運営状況, '' as 対応内容, " +
     //"入院基本料,事前調整事項,対応者立場,対応者関係,対応者姿勢,対応者関心,訪問成果,支援課題,支援提案,特記事項 as 個別支援特記事項, " +
     //"取組状況区分,取組状況,労働時間課題,医療機関感想,その他特記事項, " +
     //"'' as 実施場所,'' as 当日議題,'' as 資料医療機関,'' as 資料勤改センター,'' as 課題対応医療機関,'' as 課題対応勤改センター,'' as 支援反応,'' as 支援成果," +
     //"'' as PDCA,'' as step1,'' as step2,'' as step3,'' as step4,'' as step5,'' as step6,'' as step7,'' as 特別支援特記事項,'' as todo医療機関,'' as todo勤改センター,'' as 相談事項,'' as その他,'' as 次回訪問 " +
     //"FROM T_個別支援日誌  left join RT_顧客 B on T_個別支援日誌.顧客ID = b.顧客ID " +
     //"left join T_日誌担当 on T_日誌担当.日誌ID = T_個別支援日誌.日誌ID and T_日誌担当.支援区分 = '個別支援' " +
     //" /**where**/ " +

     //"UNION SELECT T_特別支援日誌.日誌ID, '特別支援' as 支援区分, 対応日, 相談内容区分1, 相談内容区分2," +
     //"主な発言 as 内容, B.顧客名, T_特別支援日誌.備考," +
     //"'' as 業務区分,'' as 相談手段, '' as 相談内容_運営状況, '' as 対応内容, " +
     //"'' as 入院基本料,'' as 事前調整事項,'' as 対応者立場,'' as 対応者関係,'' as 対応者姿勢,'' as 対応者関心,'' as 訪問成果,'' as 支援課題,'' as 支援提案,'' as 個別支援特記事項, " +
     //"'' as 取組状況区分,'' as 取組状況,'' as 労働時間課題,'' as 医療機関感想,'' as その他特記事項, " +
     //"実施場所,当日議題,資料医療機関,資料勤改センター,課題対応医療機関,課題対応勤改センター,支援反応,支援成果," +
     //"PDCA,step1,step2,step3,step4,step5,step6,step7,特記事項 as 特別支援特記事項,todo医療機関,todo勤改センター,相談事項,その他,次回訪問 " +
     //"FROM T_特別支援日誌  left join RT_顧客 B on T_特別支援日誌.顧客ID = b.顧客ID " +
     //"left join T_日誌担当 on T_日誌担当.日誌ID = T_特別支援日誌.日誌ID and T_日誌担当.支援区分 = '特別支援' " +
     //" /**where**/ " +
     //") as T ");//order by 対応日 desc");
                
     //           if (conditions != null)
     //           {
     //               //if (conditions.日誌ID != null)
     //               //{
     //               //    builder.Where("日誌ID= @日誌ID", new { 日誌ID = conditions.日誌ID });
     //               //    WhereStr += " and T_日誌.日誌ID=" + conditions.日誌ID;
     //               //}
     //               if (conditions.担当ID != null)
     //               {
     //                   builder.Where("担当ID= @担当ID", new { 担当ID = conditions.担当ID });
     //                   //WhereStr += " and 担当ID=" + conditions.担当ID;
     //                   joukenflg = true;
     //               }
 
     //               if (conditions.対応日_start != null)
     //               {
     //                   builder.Where("対応日>= @対応日_start", new { 対応日_start = conditions.対応日_start });
     //                   //WhereStr += " and 対応日>='" + conditions.対応日_start + "'";
     //                   joukenflg = true;
     //               }
     //               if (conditions.対応日_end != null)
     //               {
     //                   builder.Where("対応日<= @対応日_end", new { 対応日_end = conditions.対応日_end });
     //                   //WhereStr += " and 対応日<='" + conditions.対応日_end + "'";
     //                   joukenflg = true;
     //               }
     //               if (!string.IsNullOrEmpty(conditions.対応内容))
     //               {
     //                   builder.Where("対応内容 like @対応内容", new { 対応内容 = $"%{conditions.対応内容}%" });
     //                   //WhereStr += " and (対応内容 like '%" + conditions.対応内容 + "%')";
     //                   joukenflg = true;
     //               }            
     //               if (!string.IsNullOrEmpty(conditions.顧客名))
     //               {
     //                   builder.Where("顧客名 like @顧客名", new { 顧客名 = $"%{conditions.顧客名}%" });
     //                   //WhereStr += " and (顧客名 like '%" + conditions.顧客名 + "%')";
     //                   joukenflg = true;
     //               }
     //               if (!string.IsNullOrEmpty(conditions.相談内容区分))
     //               {
     //                   builder.Where("(相談内容区分1 like @相談内容区分 or 相談内容区分2 like @相談内容区分)", new { 相談内容区分 = $"%{conditions.相談内容区分}%" });
     //                   joukenflg = true;
     //               }
     //           }

     //           // ストアドプロシージャの実行
     //           //var storedProcedure = "PrcAllNissi";
     //           //var param = new DynamicParameters();
     //           //param.Add("@WhereStr", WhereStr);
     //           //var result = connection.Query<日誌>(storedProcedure, param, commandType: CommandType.StoredProcedure);

     //           string sql = unionQuery.RawSql;

     //           if (!string.IsNullOrEmpty(conditions.支援区分))
     //           {
     //               //builder.Where("支援区分= @支援区分", new { 支援区分 = conditions.支援区分 });
     //               WhereStr += " and 支援区分='" + conditions.支援区分 + "'";
     //           }
     //           if (!string.IsNullOrEmpty(conditions.相談手段))
     //           {
     //               //builder.Where("相談手段= @相談手段", new { 相談手段 = conditions.相談手段 });
     //               WhereStr += " and 相談手段='" + conditions.相談手段 + "'";
     //               joukenflg = true;
     //           }
     //           if (!string.IsNullOrEmpty(conditions.相談内容_運営状況))
     //           {
     //               //builder.Where("相談内容_運営状況 like @相談内容_運営状況", new { 相談内容_運営状況 = $"%{conditions.相談内容_運営状況}%" });
     //               WhereStr += " and (相談内容_運営状況 like '%" + conditions.相談内容_運営状況 + "%')";
     //               joukenflg = true;
     //           }
     //           if (!string.IsNullOrEmpty(conditions.相談内容_質問内容))
     //           {
     //               //builder.Where("相談内容_質問内容 like @相談内容_質問内容", new { 相談内容_質問内容 = $"%{conditions.相談内容_質問内容}%" });
     //               WhereStr += " and (相談内容_質問内容 like '%" + conditions.相談内容_質問内容 + "%')";
     //               joukenflg = true;
     //           }
     //           var template = builder.AddTemplate(unionQuery.RawSql + " where 1=1 " + WhereStr + " order by 対応日 desc");


     //           if (joukenflg == false && WhereStr=="")
     //           {
     //               nissiIndexModel.mess = "検索条件を入れてください";
     //               return nissiIndexModel;
     //           }

     //           nissiIndexModel.Nissis = connection.Query<日誌表示>(template.RawSql, template.Parameters).ToList();


     //           if (nissiIndexModel.Nissis.Count() >0 )
     //           {
     //               foreach (var item in nissiIndexModel.Nissis)
     //               {
     //                   var builder2 = new SqlBuilder();
     //                   var template2 = builder2.AddTemplate("SELECT T_担当.* FROM T_日誌担当 INNER JOIN T_担当 on T_日誌担当.担当ID = T_担当.担当ID /**where**/ order by T_日誌担当.担当ID");
     //                   builder2.Where("T_日誌担当.日誌ID = @日誌ID and T_日誌担当.支援区分 = @支援区分", new { 日誌ID = item.日誌ID, 支援区分 = item.支援区分 });
     //                   item.担当リスト = connection.Query<担当>(template2.RawSql, template2.Parameters).ToList();


     //                   var builder3 = new SqlBuilder();
     //                   var template3 = builder3.AddTemplate("SELECT T_相談者.* FROM T_日誌相談者 INNER JOIN T_相談者 on T_日誌相談者.相談者ID = T_相談者.相談者ID /**where**/ order by T_日誌相談者.相談者ID");
     //                   builder3.Where("T_日誌相談者.日誌ID = @日誌ID and T_日誌相談者.支援区分 = @支援区分", new { 日誌ID = item.日誌ID, 支援区分 = item.支援区分 });
     //                   item.相談者リスト = connection.Query<相談者>(template3.RawSql, template3.Parameters).ToList();
     //               }
     //           }
     //           else
     //           {
     //               nissiIndexModel.mess = "検索の対象が見つかりませんでした";
     //           }
     //           return nissiIndexModel;

     //       }
     //   }


     //   public 相談者 GetSodan(int SodanID)
     //   {
     //       using (var connection = new SqlConnection(_connectionString))
     //       {
     //           connection.Open();
     //           var builder = new SqlBuilder();
     //           var template = builder.AddTemplate("SELECT T_相談者.* FROM T_相談者 " +
     //               "/**where**/");

     //           builder.Where("相談者ID = @相談者ID", new { 相談者ID = SodanID });

     //           var result = connection.Query<相談者>(template.RawSql, template.Parameters).FirstOrDefault();

     //           return result;
     //       }
     //   }

    }
}
