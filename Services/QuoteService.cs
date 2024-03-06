using Dapper;
using Microsoft.Data.SqlClient;
using rehome.Enums;
using rehome.Models;
using rehome.Models.DB;
using X.PagedList;
using System.Data;
using System.Collections.Generic;

namespace rehome.Services
{
    public interface IQuoteService
    {
        public IList<見積> SearchQuotes(QuoteSearchConditions conditions);

        List<見積> GetQuote(int 顧客ID);
        見積 GetQuote(int 見積ID, int 履歴番号);

        見積 RegistQuote(QuoteCreateModel model);

        void DeleteQuote(int 見積ID, int 履歴番号);


        QuoteSalesStatusModel SearchSalesStatus(int period, DateTime start_date, DateTime end_date);
        int GetHistoryNumber(int 見積ID);

        int GetPeriod(DateTime dt);

        public void RegistQuoteTekiyo(int 見積ID, int 履歴番号, string 売上適用);

        string GetQuoteNumber(int 営業所ID, int 担当ID, DateTime 作成日);

        IList<見積明細> GetQuoteBunrui(int 見積ID, int 履歴番号);

        IList<見積明細> GetQuoteBunruiMeisai(int 見積ID, int 履歴番号, int 分類ID);
            }

    public class QuoteService : ServiceBase, IQuoteService
    {
        private readonly ILogger<QuoteService> _logger;

        public QuoteService(ILogger<QuoteService> logger, IConfiguration configuration)
            : base(configuration)
        {
            _logger = logger;
        }


        public IList<見積> SearchQuotes(QuoteSearchConditions conditions)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string WhereStr = "";
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT T_見積.*,T_担当.氏名," +
                    "COALESCE(T_見積.見込原価,0) as 見込原価,COALESCE(T.金額計,0) as 原価,COALESCE(見積金額,0)-COALESCE(値引額,0)+COALESCE(非課税額,0) as 売上, " +
                    "COALESCE(見積金額,0)-COALESCE(値引額,0)+COALESCE(非課税額,0)-COALESCE(金額計,0) AS 利益, " +
                    "FORMAT(IIF(COALESCE(見積金額,0)-COALESCE(値引額,0)+COALESCE(非課税額,0) = 0,0 " +
                    ",cast(COALESCE(見積金額,0)-COALESCE(値引額,0)+COALESCE(非課税額,0)-COALESCE(金額計,0) as decimal)/cast(COALESCE(見積金額,0)-COALESCE(値引額,0)+COALESCE(非課税額,0) as decimal)),'0.0%') as 粗利率, " +
                    "COALESCE(見積金額,0)-COALESCE(値引額,0)+COALESCE(非課税額,0)-COALESCE(見込原価,0) AS 見込利益, " +
                    "FORMAT(IIF(COALESCE(見積金額,0)-COALESCE(値引額,0)+COALESCE(非課税額,0) = 0,0 " +
                    ",cast(COALESCE(見積金額,0)-COALESCE(値引額,0)+COALESCE(非課税額,0)-COALESCE(見込原価,0) as decimal)/cast(COALESCE(見積金額,0)-COALESCE(値引額,0)+COALESCE(非課税額,0) as decimal)),'0.0%') as 見込粗利率 " +
                    "FROM T_見積 LEFT JOIN (SELECT 見積ID,履歴番号,SUM(金額) AS 金額計 FROM T_注文 GROUP BY 見積ID,履歴番号) AS T ON T_見積.見積ID = T.見積ID and T_見積.履歴番号 = T.履歴番号 " +
                    "left join T_担当 on T_見積.担当ID=T_担当.担当ID " +
                    " /**where**/ /**orderby**/");
                
                //開発中ログインID付与　注意！！
                //conditions.LoginID = 1;
               
                builder.OrderBy("T_見積.見積ID DESC");

                //if (conditions.auth == true)
                //{
                //    builder.Where("(営業所ID in (SELECT 営業所ID FROM T_担当営業所 WHERE 担当ID = @担当ID) " +
                //    "OR T_見積.担当ID= @担当ID) "
                //        , new { 担当ID = conditions.LoginID });
                //}
                //else
                //{
                //    //営業アシスタントで顧客番号の検索時のみ登録営業所すべての見積が見れる    
                //    if (conditions.assistant == true && !string.IsNullOrEmpty(conditions.見積番号))
                //    {
                //        builder.Where("(営業所ID in (SELECT 営業所ID FROM T_担当営業所 WHERE 担当ID = @担当ID) " +
                //    "OR T_見積.担当ID= @担当ID) "
                //        , new { 担当ID = conditions.LoginID });
                //    }
                //    else
                //    {
                //        //一般ユーザーと営業アシスタントで、顧客番号検索がない場合は、自分の担当見積のみ検索 
                //        builder.Where("T_見積.担当ID= @担当ID", new { 担当ID = conditions.LoginID });
                //    }
                //}

                //builder.Where("T_見積.見積番号 like @見積番号", new { 見積番号 = $"%{conditions.見積番号}%" });
                //完全一致に修正
                


                if (conditions != null)
                {

                    //if (conditions.営業所ID != null)
                    //{
                    //    builder.Where("T_見積.営業所ID= @営業所ID", new { 営業所ID = conditions.営業所ID });

                    //}
                    //if (conditions.担当ID != null)
                    //{
                                 
                    //}
                  
                    if (!string.IsNullOrEmpty(conditions.案件名))
                    {
                        builder.Where("T_見積.件名 like @案件名", new { 案件名 = $"%{ conditions.案件名}%" });

                    }

                    if (!string.IsNullOrEmpty(conditions.顧客名))
                    {
                        builder.Where("T_見積.顧客名 like @顧客名", new { 顧客名 = $"%{conditions.顧客名}%" });


                    }
                                        

                    if (!string.IsNullOrEmpty(conditions.項目))
                    {
                        builder.Where("T_見積.項目 = @項目", new { 項目 = conditions.項目 });

                    }

                    

                    if (conditions.dateStart != null)
                    {
                        builder.Where("T_見積.作成日>= @dateStart", new { dateStart = conditions.dateStart });

                    }
                    if (conditions.dateEnd != null)
                    {
                        builder.Where("T_見積.作成日<= @dateEnd", new { dateEnd = conditions.dateEnd });
    
                    }

                    builder.Where("T_見積.原価確認FLG = 0");

                    builder.Where("T_見積.見積ステータス <> '失注'");

                    // T_入金からの合計値がT_見積の見積金額未満である、またはT_入金にレコードがない条件を追加する。
                    //条件で分けたいと言われたら、ここを追加するか選択する
                    builder.Where(@"NOT EXISTS (
                                    SELECT 1
                                    FROM T_入金
                                    WHERE T_入金.見積ID = T_見積.見積ID
                                    HAVING SUM(ISNULL(T_入金.入金額, 0)) + SUM(ISNULL(T_入金.前受金, 0)) >= T_見積.見積金額
                                )");

                }

                var result = connection.Query<見積>(template.RawSql, template.Parameters);

                return result.ToList();
            }
        }


        public QuoteSalesStatusModel SearchSalesStatus(int period, DateTime start_date, DateTime end_date)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                //connection.Open();

                var model = new QuoteSalesStatusModel();

                //// ストアドプロシージャ名
                //var storedProcedure = "PrcSalesStatus";
                //// パラメータ
                //var param = new DynamicParameters();
                //param.Add("@period", period);
                //param.Add("@start_date", start_date);
                //param.Add("@end_date", end_date);

                //// ストアドプロシージャの実行

                //param.Add("@req", 1);
                //model.期オフィス別累計50 = connection.Query<SalesStatus>(storedProcedure, param, commandType: CommandType.StoredProcedure).ToList();
                //param.Add("@req", 2);
                //model.期オフィス別累計49 = connection.Query<SalesStatus>(storedProcedure, param, commandType: CommandType.StoredProcedure).ToList();
                //param.Add("@req", 3);
                //model.期間オフィス別累計 = connection.Query<SalesStatus>(storedProcedure, param, commandType: CommandType.StoredProcedure).ToList();
                //param.Add("@req", 4);
                //model.期個人別累計 = connection.Query<SalesStatus>(storedProcedure, param, commandType: CommandType.StoredProcedure).ToList();
                //param.Add("@req", 5);
                //model.期間個人別累計 = connection.Query<SalesStatus>(storedProcedure, param, commandType: CommandType.StoredProcedure).ToList();

                return model;
            }
        }
        public List<見積> GetQuote(int 顧客ID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT T_見積.*, T_担当.氏名 as 担当者名, RT_顧客.顧客名 FROM T_見積 " +
                    "left join T_担当 on T_見積.担当ID = T_担当.担当ID " +
                    "left join RT_顧客 on T_見積.顧客ID = RT_顧客.顧客ID " +
                    "/**where**/");

                builder.Where("T_見積.顧客ID = @顧客ID ", new { 顧客ID = @顧客ID});

                var result = connection.Query<見積>(template.RawSql, template.Parameters).ToList();

                return result;
            }           
        }

        public 見積 GetQuote(int 見積ID, int 履歴番号)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT T_見積.*, T_担当.氏名 as 担当者名, RT_顧客.顧客名 FROM T_見積 " +
                    "left join T_担当 on T_見積.担当ID = T_担当.担当ID " +
                    "left join RT_顧客 on T_見積.顧客ID = RT_顧客.顧客ID " +
                    "/**where**/");
           
                builder.Where("見積ID = @見積ID and 履歴番号=@履歴番号", new { 見積ID = 見積ID, 履歴番号 = @履歴番号 });

                var result = connection.Query<見積>(template.RawSql, template.Parameters).FirstOrDefault();

                if (result != null)
                {
                    // 見積明細を取得する
                    builder = new SqlBuilder();
                    template = builder.AddTemplate("SELECT * FROM T_見積明細 /**where**/ ORDER BY 連番");

                    builder.Where("見積ID = @見積ID and 履歴番号=@履歴番号", new { 見積ID = 見積ID, 履歴番号 = @履歴番号 });
                    var quotations = connection.Query<見積明細>(template.RawSql, template.Parameters);
                    result.見積明細リスト = quotations.ToList();//これだとOK  

                    builder = new SqlBuilder();
                    template = builder.AddTemplate("SELECT T_見積分類表示順.見積ID,T_見積分類表示順.履歴番号,T_見積分類表示順.分類ID, " +
                                                   "COALESCE(T_見積分類表示順.表示順,99999) as 表示順,IIf(COALESCE(B.分類名,'')<>'',B.分類名,T_分類.分類名) as 分類名 " +
                                                   "FROM T_見積分類表示順 " +
                                                   "left join T_分類 on T_見積分類表示順.分類ID = T_分類.分類ID " +
                                                   "left join (SELECT DISTINCT 分類ID,分類名 FROM T_見積明細 WHERE 見積ID = @見積ID and 履歴番号 = @履歴番号) as B on B.分類ID = T_見積分類表示順.分類ID " +
                                                   "/**where**/ORDER BY 表示順");

                    builder.Where("見積ID = @見積ID and 履歴番号=@履歴番号", new { 見積ID = 見積ID, 履歴番号 = @履歴番号 });
                    var bunruis = connection.Query<見積分類表示順>(template.RawSql, template.Parameters);
                    result.見積分類表示順リスト = bunruis.ToList();//これだとOK  

                }

                return result;
            }
        }

        public int GetHistoryNumber(int 見積ID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT MAX(履歴番号) FROM T_見積  /**where**/");

                builder.Where("見積ID = @見積ID", new { 見積ID = 見積ID });

                var result = connection.Query<int>(template.RawSql, template.Parameters).FirstOrDefault();

                return result;
            }
        }

        public void RegistQuoteTekiyo(int 見積ID, int 履歴番号, string 注文摘要)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var tx = connection.BeginTransaction())
                {
                    try
                    {
                        var sql = "";
                        sql = "update T_見積 set 注文摘要=@注文摘要 " +
                            " WHERE 見積ID = @見積ID and 履歴番号 =@履歴番号";

                        var result = connection.Execute(sql,
                        new
                        {
                            見積ID = 見積ID,
                            履歴番号 = 履歴番号,
                            注文摘要 = 注文摘要
                        }
                            , tx);
                        tx.Commit();
                        return;
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }


        public int GetPeriod(DateTime dt)
        {
            int Period = 16;

            // 2022年11月の日付を作成
            DateTime november2022 = new DateTime(2022, 11, 1);

            // 現在の日付を取得
            DateTime currentDate = dt;

            // 12カ月経過するごとに1を追加
            int yearDifference = currentDate.Year - november2022.Year;
            if (currentDate.Month < november2022.Month || (currentDate.Month == november2022.Month && currentDate.Day < november2022.Day))
            {
                yearDifference--;
            }

            Period += yearDifference;

            return Period;
        }


        public 見積 RegistQuote(QuoteCreateModel model)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var queryInsert = "INSERT INTO T_見積 (見積ID,履歴番号,見積番号,最新FLG,担当ID,顧客ID," +
                             "顧客名,敬称,件名,納期,項目,見積区分,受渡場所,支払条件,有効期限," +
                             "見積金額,見込原価,見込利益,作成日,請求日,受注確度," +
                             "time_stamp,非課税名称,非課税額,値引名称,値引額,備考,single,入金種別," +
                             "入金日,入金締日,見積ステータス,取引年月日,種類,種類2,JS番号,部屋番号,原価確認FLG) " +
                        "VALUES ( @見積ID,@履歴番号,@見積番号,@最新FLG,@担当ID,@顧客ID," +
                        "@顧客名,@敬称,@件名,@納期,@項目,@見積区分,@受渡場所,@支払条件,@有効期限," +
                        "@見積金額,@見込原価,@見込利益,@作成日,@請求日,@受注確度," +
                        "@time_stamp,@非課税名称,@非課税額,@値引名称,@値引額,@備考,@single,@入金種別," +
                        "@入金日,@入金締日,@見積ステータス, @取引年月日, @種類, @種類2, @JS番号, @部屋番号,@原価確認FLG)";

                connection.Open();
                using (var tx = connection.BeginTransaction())
                {
                    try
                    {
                        var sql = "";
                        int 処理後見積ID = -1;
                        int 処理後履歴番号 = -1;

                        if (model.Mode == ViewMode.New)
                        {
                            //新規
                            sql = "SELECT (isnull(MAX(見積ID),0) + 1) as 見積ID FROM T_見積 ";

                            var result = connection.QuerySingle(sql, null, tx);

                            model.Quote.見積ID = result.見積ID;
                            model.Quote.履歴番号 = 1;
                            model.Quote.time_stamp = DateTime.Now;
                            処理後見積ID = model.Quote.見積ID;
                            処理後履歴番号 = model.Quote.履歴番号;                                                        

                            var insert = connection.Execute(queryInsert, model.Quote, tx);

                        }
                        else if (model.Mode == ViewMode.Edit)
                        {
                            //更新
                            var queryUpdate = "UPDATE T_見積 SET 見積番号=@見積番号,最新FLG=@最新FLG,担当ID=@担当ID," +
                                "顧客ID=@顧客ID,顧客名=@顧客名,敬称=@敬称,件名=@件名,納期=@納期,項目=@項目," +
                                "見積区分=@見積区分,受渡場所=@受渡場所, 支払条件=@支払条件,有効期限=@有効期限," +
                                "見積金額=@見積金額,見込原価=@見込原価,見込利益=@見込利益," +
                                "作成日=@作成日,請求日=@請求日,受注確度=@受注確度,time_stamp=@time_stamp, " +
                                "非課税名称=@非課税名称,非課税額=@非課税額,値引名称=@値引名称,値引額=@値引額," +
                                "備考=@備考,single=@single,入金種別=@入金種別,入金日=@入金日,入金締日=@入金締日," +
                                "見積ステータス=@見積ステータス,取引年月日=@取引年月日, " +
                                "種類=@種類, 種類2=@種類2 , JS番号=@JS番号, 部屋番号=@部屋番号,原価確認FLG=@原価確認FLG " +
                                " WHERE 見積ID = @見積ID and 履歴番号 =@履歴番号";

                            var result = connection.Execute(queryUpdate, model.Quote, tx);
                            model.Quote.time_stamp = DateTime.Now;
                            処理後見積ID = model.Quote.見積ID;
                            処理後履歴番号 = model.Quote.履歴番号;

                            var queryDetailsDelete = "DELETE FROM T_見積明細 WHERE 見積ID = @見積ID and 履歴番号 = @履歴番号";

                            var deleteDatails = connection.Execute(queryDetailsDelete, model.Quote, tx);

                            var queryDetailsDelete2 = "DELETE FROM T_見積分類表示順 WHERE 見積ID = @見積ID and 履歴番号 = @履歴番号";

                            var deleteDatail2s = connection.Execute(queryDetailsDelete2, model.Quote, tx);



                        }
                        else if (model.Mode == ViewMode.ReEstimate)
                        {
                            //再見積

                            model.Quote.最新FLG = false;
                            var queryUpdate = "UPDATE T_見積 SET 最新FLG = @最新FLG WHERE 見積ID = @見積ID";

                            var update = connection.Execute(queryUpdate, model.Quote, tx);

                            sql = "SELECT (MAX(履歴番号) + 1) as 履歴番号 FROM T_見積 WHERE 見積ID = @見積ID";

                            var result = connection.QuerySingle(sql, model.Quote, tx);

                            model.Quote.最新FLG = true;
                            model.Quote.履歴番号 = result.履歴番号;
                            model.Quote.作成日 = DateTime.Now;
                            model.Quote.time_stamp = DateTime.Now;
                            処理後見積ID = model.Quote.見積ID;
                            処理後履歴番号 = model.Quote.履歴番号;

                            var insert = connection.Execute(queryInsert, model.Quote, tx);

                        }
                        else if (model.Mode == ViewMode.Copy)
                        {
                            //複写
                            sql = "SELECT (MAX(見積ID) + 1) as 見積ID FROM T_見積 ";

                            var result = connection.QuerySingle(sql, null, tx);

                            model.Quote.見積ID = result.見積ID;
                            model.Quote.履歴番号 = 1;
                            model.Quote.作成日 = DateTime.Now;
                            model.Quote.time_stamp = DateTime.Now;
                            処理後見積ID = model.Quote.見積ID;
                            処理後履歴番号 = model.Quote.履歴番号;
                                                        
                            var insert = connection.Execute(queryInsert, model.Quote, tx);

                        }

                        if (model.Quote.見積明細リスト != null)
                        {

                            //var i = 1;

                            List<見積明細> MeisaiList = model.Quote.見積明細リスト.OrderBy(a => a.分類ID).ThenBy(a => a.連番).ToList();

                            for (var idx=0;idx< MeisaiList.Count();idx++)
                            {
                                if (MeisaiList[idx].削除FLG == false && (MeisaiList[idx].商品名 != null || (MeisaiList[idx].数量 != null && MeisaiList[idx].数量 != 0) || (MeisaiList[idx].金額 != null && MeisaiList[idx].金額 != 0) || (MeisaiList[idx].単価 != null && MeisaiList[idx].単価 != 0) || MeisaiList[idx].単位 != null))
                                {
                                    var queryDetailInsert = "INSERT INTO T_見積明細 (見積ID,履歴番号,分類ID,分類名," +
                                        "連番,商品名,数量,単位,単価,金額,見込原価,備考,非計上FLG,寸法) " +
                                        "                                    VALUES (@見積ID,@履歴番号,@分類ID,@分類名," +
                                        "@連番,@商品名,@数量,@単位,@単価,@金額,@見込原価,@備考,@非計上FLG,@寸法)";

                                    MeisaiList[idx].見積ID = model.Quote.見積ID;
                                    MeisaiList[idx].履歴番号 = model.Quote.履歴番号;

                                    var insertDetail = connection.Execute(queryDetailInsert, MeisaiList[idx], tx);
                                    //i++;
                                }

                            }
                        }


                        if (model.Quote.見積分類表示順リスト != null)
                        {
                            List<見積分類表示順> BunruiList = model.Quote.見積分類表示順リスト.OrderBy(a => a.表示順).ToList();

                            for (var idx = 0; idx < BunruiList.Count(); idx++)
                            {
                                var queryBunruiInsert = "INSERT INTO T_見積分類表示順 (見積ID,履歴番号,分類ID,表示順) " +
                                                                              "VALUES (@見積ID,@履歴番号,@分類ID,@表示順)";

                                BunruiList[idx].見積ID = model.Quote.見積ID;
                                BunruiList[idx].履歴番号 = model.Quote.履歴番号;

                                var insertBunrui = connection.Execute(queryBunruiInsert, BunruiList[idx], tx);
                            }
                        }
                        
                        tx.Commit();
                        return GetQuote(処理後見積ID, 処理後履歴番号);                        
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        public void DeleteQuote(int 見積ID,int 履歴番号)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var tx = connection.BeginTransaction())
                {
                    try
                    {
                        //削除
                        var queryDelete = "DELETE FROM T_見積 WHERE 見積ID = " + 見積ID + " and 履歴番号 = " + 履歴番号;

                        var delete = connection.Execute(queryDelete, null, tx);


                        var queryDetailsDelete = "DELETE FROM T_見積明細 WHERE 見積ID = " + 見積ID + " and 履歴番号 = " + 履歴番号;

                        var deleteDatails = connection.Execute(queryDetailsDelete, null, tx);

                        tx.Commit();

                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        public string GetQuoteNumber(int 営業所ID,int 担当ID,DateTime 作成日)
        {
            using (var connection = new SqlConnection(_connectionString))
            {

                var QuoteNumber = "";

                connection.Open();

                var 期 = GetPeriod(DateTime.Now);

                var builder2 = new SqlBuilder();
                var template2 = builder2.AddTemplate("SELECT イニシャル FROM T_担当 /**where**/");
                builder2.Where("担当ID = @担当ID", new { 担当ID = 担当ID });
                var イニシャル = connection.Query<string>(template2.RawSql, template2.Parameters).FirstOrDefault();

                QuoteNumber = 期 + "-" + 営業所ID + イニシャル + 作成日.ToString("MMdd");

                var builder3 = new SqlBuilder();
                var template3 = builder3.AddTemplate("SELECT Count(*) FROM T_見積 /**where**/");
                builder3.Where("見積番号 like @見積番号", new { 見積番号 = $"%{QuoteNumber}%" });
                var quote_cnt = connection.Query<int>(template3.RawSql, template3.Parameters).FirstOrDefault();
                
                if(quote_cnt > 0)
                {
                    QuoteNumber += (quote_cnt+1).ToString();
                }                

                return QuoteNumber;
            }
        }


         
        public IList<見積明細> GetQuoteBunrui(int 見積ID, int 履歴番号)
        {

            using (var connection = new SqlConnection(_connectionString))
            {

                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT T_見積明細.分類ID, IIf(MAX(T_分類.分類名) is null,MAX(T_見積明細.分類名),MAX(T_分類.分類名)) as 分類名,SUM(金額) as 単価 " +
                                                   "FROM T_見積明細 LEFT JOIN T_分類 on T_見積明細.分類ID = T_分類.分類ID " +
                                                   "LEFT JOIN T_見積分類表示順 on T_見積明細.見積ID = T_見積分類表示順.見積ID and T_見積明細.履歴番号 = T_見積分類表示順.履歴番号 and T_見積明細.分類ID = T_見積分類表示順.分類ID " +
                                                   "/**where**/ GROUP BY T_見積明細.分類ID,T_見積分類表示順.表示順 ORDER BY T_見積分類表示順.表示順");
                builder.Where("T_見積明細.見積ID = @見積ID and T_見積明細.履歴番号= @履歴番号", new { 見積ID = 見積ID,履歴番号 = 履歴番号 });
                var quote = connection.Query<見積明細>(template.RawSql, template.Parameters).ToList();

                return quote;
            }
        }


        public IList<見積明細> GetQuoteBunruiMeisai(int 見積ID, int 履歴番号,int 分類ID)
        {

            using (var connection = new SqlConnection(_connectionString))
            {

                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT * FROM T_見積明細 /**where**/ ORDER BY 連番");
                builder.Where("見積ID = @見積ID and 履歴番号= @履歴番号 and 分類ID=@分類ID", new { 見積ID = 見積ID, 履歴番号 = 履歴番号,分類ID=分類ID });
                var quote = connection.Query<見積明細>(template.RawSql, template.Parameters).ToList();

                return quote;
            }
        }


    }
}


