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
    public interface INyukinService
    {
        public IList<見積> SearchNyukins(NyukinSearchConditions conditions);

        List<入金> GetNyukin(int 見積ID, int 履歴番号);
        
        //見積 RegistNyukin(NyukinCreateModel model);

        void DeleteNyukin(int 見積ID, int 履歴番号);      
   
            }

    public class NyukinService : ServiceBase, INyukinService
    {
        private readonly ILogger<NyukinService> _logger;

        public NyukinService(ILogger<NyukinService> logger, IConfiguration configuration)
            : base(configuration)
        {
            _logger = logger;
        }


        public IList<見積> SearchNyukins(NyukinSearchConditions conditions)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string WhereStr = "";
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT T_見積.* , T.入金合計, T.最終入金日 " +
                    "FROM T_見積 LEFT JOIN (SELECT 見積ID,履歴番号,SUM(入金額) AS 入金合計 ,max(入金日) as 最終入金日 " +
                    "FROM T_入金 GROUP BY 見積ID,履歴番号) AS T " +
                    "ON T_見積.見積ID = T.見積ID and T_見積.履歴番号 = T.履歴番号 " +
                    " /**where**/ /**orderby**/");
                 
                builder.OrderBy("T_見積.見積ID DESC");
                    
                if (conditions != null)
                {
                                                
                    if (!string.IsNullOrEmpty(conditions.案件名))
                    {
                        builder.Where("T_見積.件名 like @案件名", new { 案件名 = $"%{ conditions.案件名}%" });
                    }

                    if (!string.IsNullOrEmpty(conditions.顧客名))
                    {
                        builder.Where("T_見積.顧客名 like @顧客名", new { 顧客名 = $"%{conditions.顧客名}%" });
                    }
                                     

                    if (conditions.請求日start != null)
                    {
                        builder.Where("T_見積.請求日>= @請求日start", new { 請求日start = conditions.請求日start });
                    }
                    if (conditions.請求日end != null)
                    {
                        builder.Where("T_見積.請求日<= @請求日end", new { 請求日end = conditions.請求日end });    
                    }

                }

                var result = connection.Query<見積>(template.RawSql, template.Parameters);
                return result.ToList();
            }
        }


    
        public List<入金> GetNyukin(int 見積ID,int 履歴番号)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT T_見積.*, T_担当.氏名 as 担当者名, RT_顧客.顧客名 FROM T_見積 " +
                    "left join T_入金 on T_見積.見積ID = T_入金.見積ID and T_見積.履歴番号 = T_入金.履歴番号 " +
                    "/**where**/");

                builder.Where("見積ID = @見積ID and 履歴番号=@履歴番号 ", new { 見積ID = 見積ID, 履歴番号 = @履歴番号 });

                var result = connection.Query<入金>(template.RawSql, template.Parameters).ToList();
                return result;
            }           
        }

        //public 見積 GetNyukin(int 見積ID, int 履歴番号)
        //{
        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        connection.Open();
        //        var builder = new SqlBuilder();
        //        var template = builder.AddTemplate("SELECT T_見積.*, T_担当.氏名 as 担当者名, RT_顧客.顧客名 FROM T_見積 " +
        //            "left join T_担当 on T_見積.担当ID = T_担当.担当ID " +
        //            "left join RT_顧客 on T_見積.顧客ID = RT_顧客.顧客ID " +
        //            "/**where**/");
           
        //        builder.Where("見積ID = @見積ID and 履歴番号=@履歴番号", new { 見積ID = 見積ID, 履歴番号 = @履歴番号 });

        //        var result = connection.Query<見積>(template.RawSql, template.Parameters).FirstOrDefault();

        //        if (result != null)
        //        {
        //            // 見積明細を取得する
        //            builder = new SqlBuilder();
        //            template = builder.AddTemplate("SELECT * FROM T_見積明細 /**where**/ ORDER BY 連番");

        //            builder.Where("見積ID = @見積ID and 履歴番号=@履歴番号", new { 見積ID = 見積ID, 履歴番号 = @履歴番号 });
        //            var quotations = connection.Query<見積明細>(template.RawSql, template.Parameters);
        //            result.見積明細リスト = quotations.ToList();//これだとOK  

        //            builder = new SqlBuilder();
        //            template = builder.AddTemplate("SELECT T_見積分類表示順.見積ID,T_見積分類表示順.履歴番号,T_見積分類表示順.分類ID, " +
        //                                           "COALESCE(T_見積分類表示順.表示順,99999) as 表示順,IIf(COALESCE(B.分類名,'')<>'',B.分類名,T_分類.分類名) as 分類名 " +
        //                                           "FROM T_見積分類表示順 " +
        //                                           "left join T_分類 on T_見積分類表示順.分類ID = T_分類.分類ID " +
        //                                           "left join (SELECT DISTINCT 分類ID,分類名 FROM T_見積明細 WHERE 見積ID = @見積ID and 履歴番号 = @履歴番号) as B on B.分類ID = T_見積分類表示順.分類ID " +
        //                                           "/**where**/ORDER BY 表示順");

        //            builder.Where("見積ID = @見積ID and 履歴番号=@履歴番号", new { 見積ID = 見積ID, 履歴番号 = @履歴番号 });
        //            var bunruis = connection.Query<見積分類表示順>(template.RawSql, template.Parameters);
        //            result.見積分類表示順リスト = bunruis.ToList();//これだとOK  

        //        }

        //        return result;
        //    }
        //}


        //public 見積 RegistNyukin(NyukinIndexModel model)
        //{
        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        connection.Open();
        //        using (var tx = connection.BeginTransaction())
        //        {
        //            try
        //            {
        //                var sql = "";
        //                int 処理後見積ID = -1;
        //                int 処理後履歴番号 = -1;

        //                if (model.Mode == ViewMode.New)
        //                {
        //                    //新規
        //                    sql = "SELECT (isnull(MAX(見積ID),0) + 1) as 見積ID FROM T_見積 ";

        //                    var result = connection.QuerySingle(sql, null, tx);

        //                    model.Quote.見積ID = result.見積ID;
        //                    model.Quote.履歴番号 = 1;
        //                    model.Quote.time_stamp = DateTime.Now;
        //                    処理後見積ID = model.Quote.見積ID;
        //                    処理後履歴番号 = model.Quote.履歴番号;

        //                    var queryInsert = "INSERT INTO T_見積 (見積ID,履歴番号,見積番号,最新FLG,担当ID,顧客ID," +
        //                        "顧客名,敬称,件名,納期,項目,見積区分,受渡場所,支払条件,有効期限," +
        //                        "見積金額,見込原価,見込利益,作成日,完了予定日,受注確度," +
        //                        "time_stamp,非課税名称,非課税額,値引名称,値引額,備考,single,入金種別," +
        //                        "入金日,入金締日,見積ステータス,取引年月日) " +
        //                   "VALUES ( @見積ID,@履歴番号,@見積番号,@最新FLG,@担当ID,@顧客ID," +
        //                   "@顧客名,@敬称,@件名,@納期,@項目,@見積区分,@受渡場所,@支払条件,@有効期限," +
        //                   "@見積金額,@見込原価,@見込利益,@作成日,@完了予定日,@受注確度," +
        //                   "@time_stamp,@非課税名称,@非課税額,@値引名称,@値引額,@備考,@single,@入金種別," +
        //                   "@入金日,@入金締日,@見積ステータス,@取引年月日)";

        //                    var insert = connection.Execute(queryInsert, model.Quote, tx);

        //                }
        //                else if (model.Mode == ViewMode.Edit)
        //                {
        //                    //更新
        //                    var queryUpdate = "UPDATE T_見積 SET 見積番号=@見積番号,最新FLG=@最新FLG,担当ID=@担当ID," +
        //                        "顧客ID=@顧客ID,顧客名=@顧客名,敬称=@敬称,件名=@件名,納期=@納期,項目=@項目," +
        //                        "見積区分=@見積区分, 支払条件=@支払条件,有効期限=@有効期限," +
        //                        "見積金額=@見積金額,見込原価=@見込原価,見込利益=@見込利益," +
        //                        "作成日=@作成日,完了予定日=@完了予定日,受注確度=@受注確度,time_stamp=@time_stamp, " +
        //                        "非課税名称=@非課税名称,非課税額=@非課税額,値引名称=@値引名称,値引額=@値引額," +
        //                        "備考=@備考,single=@single,入金種別=@入金種別,入金日=@入金日,入金締日=@入金締日," +
        //                        "見積ステータス=@見積ステータス,取引年月日=@取引年月日 " +
        //                        " WHERE 見積ID = @見積ID and 履歴番号 =@履歴番号";

        //                    var result = connection.Execute(queryUpdate, model.Quote, tx);
        //                    model.Quote.time_stamp = DateTime.Now;
        //                    処理後見積ID = model.Quote.見積ID;
        //                    処理後履歴番号 = model.Quote.履歴番号;

        //                    var queryDetailsDelete = "DELETE FROM T_見積明細 WHERE 見積ID = @見積ID and 履歴番号 = @履歴番号";

        //                    var deleteDatails = connection.Execute(queryDetailsDelete, model.Quote, tx);

        //                    var queryDetailsDelete2 = "DELETE FROM T_見積分類表示順 WHERE 見積ID = @見積ID and 履歴番号 = @履歴番号";

        //                    var deleteDatail2s = connection.Execute(queryDetailsDelete2, model.Quote, tx);



        //                }
        //                else if (model.Mode == ViewMode.ReEstimate)
        //                {
        //                    //再見積

        //                    model.Quote.最新FLG = false;
        //                    var queryUpdate = "UPDATE T_見積 SET 最新FLG = @最新FLG WHERE 見積ID = @見積ID";

        //                    var update = connection.Execute(queryUpdate, model.Quote, tx);

        //                    sql = "SELECT (MAX(履歴番号) + 1) as 履歴番号 FROM T_見積 WHERE 見積ID = @見積ID";

        //                    var result = connection.QuerySingle(sql, model.Quote, tx);

        //                    model.Quote.最新FLG = true;
        //                    model.Quote.履歴番号 = result.履歴番号;
        //                    model.Quote.作成日 = DateTime.Now;
        //                    model.Quote.time_stamp = DateTime.Now;
        //                    処理後見積ID = model.Quote.見積ID;
        //                    処理後履歴番号 = model.Quote.履歴番号;

        //                    var queryInsert = "INSERT INTO T_見積 (見積ID,履歴番号,見積番号,最新FLG,担当ID," +
        //                        "営業所ID,顧客名,敬称,件名,納期,項目,見積区分,受渡場所,支払条件,有効期限," +
        //                        "理化学医療区分,見積金額,見込原価,見込利益,作成日,完了予定日,受注確度," +
        //                        "time_stamp,非課税名称,非課税額,値引名称,値引額,備考,single,入金種別," +
        //                        "入金日,入金締日,見積ステータス,期,注文摘要,取引年月日) " +
        //                        "VALUES ( @見積ID,@履歴番号,@見積番号,@最新FLG,@担当ID," +
        //                        "@営業所ID,@顧客名,@敬称,@件名,@納期,@項目,@見積区分,@受渡場所,@支払条件,@有効期限," +
        //                        "@理化学医療区分,@見積金額,@見込原価,@見込利益,@作成日,@完了予定日,@受注確度," +
        //                        "@time_stamp,@非課税名称,@非課税額,@値引名称,@値引額,@備考,@single,@入金種別," +
        //                        "@入金日,@入金締日,@見積ステータス,@期,@注文摘要,@取引年月日)";

        //                    var insert = connection.Execute(queryInsert, model.Quote, tx);

        //                }
        //                else if (model.Mode == ViewMode.Copy)
        //                {
        //                    //複写
        //                    sql = "SELECT (MAX(見積ID) + 1) as 見積ID FROM T_見積 ";

        //                    var result = connection.QuerySingle(sql, null, tx);

        //                    model.Quote.見積ID = result.見積ID;
        //                    model.Quote.履歴番号 = 1;
        //                    model.Quote.作成日 = DateTime.Now;
        //                    model.Quote.time_stamp = DateTime.Now;
        //                    処理後見積ID = model.Quote.見積ID;
        //                    処理後履歴番号 = model.Quote.履歴番号;

        //                    var queryInsert = "INSERT INTO T_見積 (見積ID,履歴番号,見積番号,最新FLG,担当ID,営業所ID," +
        //                        "顧客名,敬称,件名,納期,項目,見積区分,受渡場所,支払条件,有効期限," +
        //                        "理化学医療区分,見積金額,見込原価,見込利益,作成日,完了予定日,受注確度," +
        //                        "time_stamp,非課税名称,非課税額,値引名称,値引額,備考,single,入金種別," +
        //                        "入金日,入金締日,見積ステータス,期,注文摘要,取引年月日) " +
        //                        "VALUES ( @見積ID,@履歴番号,@見積番号,@最新FLG,@担当ID,@営業所ID," +
        //                        "@顧客名,@敬称,@件名,@納期,@項目,@見積区分,@受渡場所,@支払条件,@有効期限," +
        //                        "@理化学医療区分,@見積金額,@見込原価,@見込利益,@作成日,@完了予定日,@受注確度," +
        //                        "@time_stamp,@非課税名称,@非課税額,@値引名称,@値引額,@備考,@single,@入金種別," +
        //                        "@入金日,@入金締日,@見積ステータス,@期,@注文摘要,@取引年月日)";

        //                    var insert = connection.Execute(queryInsert, model.Quote, tx);

        //                }

        //                if (model.Quote.見積明細リスト != null)
        //                {

        //                    //var i = 1;

        //                    List<見積明細> MeisaiList = model.Quote.見積明細リスト.OrderBy(a => a.分類ID).ThenBy(a => a.連番).ToList();

        //                    for (var idx=0;idx< MeisaiList.Count();idx++)
        //                    {
        //                        if (MeisaiList[idx].削除FLG == false && (MeisaiList[idx].商品名 != null || (MeisaiList[idx].数量 != null && MeisaiList[idx].数量 != 0) || (MeisaiList[idx].金額 != null && MeisaiList[idx].金額 != 0) || (MeisaiList[idx].単価 != null && MeisaiList[idx].単価 != 0) || MeisaiList[idx].単位 != null))
        //                        {
        //                            var queryDetailInsert = "INSERT INTO T_見積明細 (見積ID,履歴番号,分類ID,分類名," +
        //                                "連番,商品名,数量,単位,単価,金額,見込原価,備考) " +
        //                                "                                    VALUES (@見積ID,@履歴番号,@分類ID,@分類名," +
        //                                "@連番,@商品名,@数量,@単位,@単価,@金額,@見込原価,@備考)";

        //                            MeisaiList[idx].見積ID = model.Quote.見積ID;
        //                            MeisaiList[idx].履歴番号 = model.Quote.履歴番号;

        //                            var insertDetail = connection.Execute(queryDetailInsert, MeisaiList[idx], tx);
        //                            //i++;
        //                        }

        //                    }
        //                }


        //                if (model.Quote.見積分類表示順リスト != null)
        //                {

        //                    List<見積分類表示順> BunruiList = model.Quote.見積分類表示順リスト.OrderBy(a => a.表示順).ToList();

        //                    for (var idx = 0; idx < BunruiList.Count(); idx++)
        //                    {
        //                        var queryBunruiInsert = "INSERT INTO T_見積分類表示順 (見積ID,履歴番号,分類ID,表示順) " +
        //                                                                      "VALUES (@見積ID,@履歴番号,@分類ID,@表示順)";

        //                        BunruiList[idx].見積ID = model.Quote.見積ID;
        //                        BunruiList[idx].履歴番号 = model.Quote.履歴番号;

        //                        var insertBunrui = connection.Execute(queryBunruiInsert, BunruiList[idx], tx);
        //                    }
        //                }

                        
        //                tx.Commit();

        //                return GetQuote(処理後見積ID, 処理後履歴番号);
                        
        //            }
        //            catch (Exception ex)
        //            {
        //                tx.Rollback();
        //                throw;
        //            }
        //        }
        //    }
        //}

        public void DeleteNyukin(int 見積ID,int 履歴番号)
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

     
    }
}


