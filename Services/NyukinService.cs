using Dapper;
using Microsoft.Data.SqlClient;
using rehome.Enums;
using rehome.Models;
using rehome.Models.DB;
using X.PagedList;
using System.Data;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Office2010.Excel;
using MigraDocCore.DocumentObjectModel;

namespace rehome.Services
{
    public interface INyukinService
    {
        public IList<見積> SearchNyukins(NyukinSearchConditions conditions);

        List<入金> GetNyukin(int 見積ID, int 履歴番号);

        int RegistNyukin(NyukinIndexModel model);

        void DeleteNyukin(int 見積ID, int 履歴番号);

        int CountNyukin(int 見積ID, int 履歴番号);

        public IList<請求合計一覧> GetTotal(NyukinSearchConditions conditions);

    }

    public class NyukinService : ServiceBase, INyukinService
    {
        private readonly ILogger<NyukinService> _logger;

        public NyukinService(ILogger<NyukinService> logger, IConfiguration configuration)
            : base(configuration)
        {
            _logger = logger;
        }

        public IList<請求合計一覧> GetTotal(NyukinSearchConditions conditions)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string WhereStr = "";
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("select '総合計' as 項目 , sum(見積金額) as 合計額 FROM T_見積 /**where**/　" +
                    "union all " +
                    "SELECT 項目, sum(見積金額) as 合計額 FROM T_見積 /**where**/ group by 項目 "
                    );
                                

                builder.Where("T_見積.見積ステータス = '請求' ");

                if (conditions != null)
                {

                    if (conditions.請求日start != null)
                    {
                        builder.Where("T_見積.請求日>= @請求日start", new { 請求日start = conditions.請求日start });
                    }
                    if (conditions.請求日end != null)
                    {
                        builder.Where("T_見積.請求日<= @請求日end", new { 請求日end = conditions.請求日end });
                    }
                }
                var result = connection.Query<請求合計一覧>(template.RawSql, template.Parameters);
                return result.ToList();
            }
        }
            public IList<見積> SearchNyukins(NyukinSearchConditions conditions)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string WhereStr = "";
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT T_見積.* , isnull(T.入金合計,0) as 入金合計, T.最終入金日, " +
                    "case when 見積金額-isnull(入金合計,0)>0 then '未納' " +
                    " else '' end as 納付状況 " +
                    "FROM T_見積 " +
                    "LEFT JOIN (SELECT 見積ID,履歴番号,isnull(SUM(入金額),0) + isnull(SUM(振込手数料),0) + isnull(SUM(前受金),0) AS 入金合計 ," +
                    "max(入金日) as 最終入金日 " +
                    "FROM T_入金 GROUP BY 見積ID,履歴番号) AS T " +
                    "ON T_見積.見積ID = T.見積ID and T_見積.履歴番号 = T.履歴番号 " +
                    " /**where**/ /**orderby**/");

                builder.OrderBy("T_見積.作成日 DESC");

                builder.Where("T_見積.見積ステータス = '請求' ");

                if (conditions != null)
                {

                    if (!string.IsNullOrEmpty(conditions.案件名))
                    {
                        builder.Where("T_見積.件名 like @案件名", new { 案件名 = $"%{conditions.案件名}%" });
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



        public List<入金> GetNyukin(int 見積ID, int 履歴番号)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT T_入金.* FROM T_見積 " +
                    "left join T_入金 on T_見積.見積ID = T_入金.見積ID and T_見積.履歴番号 = T_入金.履歴番号 " +
                    "/**where**/");

                builder.Where("T_見積.見積ID = @見積ID and T_見積.履歴番号=@履歴番号 ", new { 見積ID = 見積ID, 履歴番号 = 履歴番号 });

                var result = connection.Query<入金>(template.RawSql, template.Parameters).ToList();
                return result;
            }
        }

        public int CountNyukin(int 見積ID, int 履歴番号)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT * FROM T_入金 " +
                    "/**where**/");
                builder.Where("見積ID = @見積ID and 履歴番号=@履歴番号 ", new { 見積ID = 見積ID, 履歴番号 = 履歴番号 });

                int result = connection.Query<int>(template.RawSql, template.Parameters).Count();

                return result;
            }
        }


        public int RegistNyukin(NyukinIndexModel model)
        {
            int result = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var tx = connection.BeginTransaction())
                {
                    try
                    {

                        for (var idx = 0; idx < model.Nyukins.Count(); idx++)
                        {
                            //多分前受金か入金額のnullチェックは変更になると思われる
                            if (!model.Nyukins[idx].削除FLG)
                            {
                                if (model.Nyukins[idx].入金日 != null &&
                                (model.Nyukins[idx].入金額 != null || model.Nyukins[idx].前受金 != null))
                                {
                                    var merge = "MERGE INTO T_入金 AS target " +
                                    "USING(select @入金ID as 入金ID, " +
                                    "@見積ID as 見積ID, " +
                                    "@履歴番号 as 履歴番号, " +
                                    "@入金日 as 入金日," +
                                    "@入金額 as 入金額," +
                                    "@振込手数料 as 振込手数料," +
                                    "@振込名義 as 振込名義," +
                                    "@前受金 as 前受金," +
                                    "@入金種別 as 入金種別," +
                                    "@備考 as 備考)　AS source " +
                                    "ON target.入金ID = source.入金ID " +
                                    "WHEN MATCHED THEN UPDATE SET " +
                                    "見積ID = source.見積ID, 履歴番号 = source.履歴番号,入金日 = source.入金日,入金額 = source.入金額,振込手数料 = source.振込手数料, " +
                                    "振込名義 = source.振込名義, 前受金 = source.前受金,入金種別 = source.入金種別,登録日 = getdate()," +
                                    "備考 = source.備考 " +
                                    "WHEN NOT MATCHED THEN " +
                                    "INSERT(見積ID, 履歴番号, 入金日,入金額, 振込手数料," +
                                    " 振込名義, 前受金,入金種別, 登録日,備考) " +
                                    "VALUES(source.見積ID, source.履歴番号, source.入金日,source.入金額, source.振込手数料," +
                                    "source.振込名義, source.前受金,source.入金種別,getdate(),source.備考); ";


                                    result = connection.Execute(merge,
                                        new
                                        {
                                            入金ID = model.Nyukins[idx].入金ID,
                                            見積ID = model.見積ID,
                                            履歴番号 = model.履歴番号,
                                            入金日 = model.Nyukins[idx].入金日,
                                            入金額 = model.Nyukins[idx].入金額,
                                            振込手数料 = model.Nyukins[idx].振込手数料,
                                            振込名義 = model.Nyukins[idx].振込名義,
                                            前受金 = model.Nyukins[idx].前受金,
                                            入金種別 = model.Nyukins[idx].入金種別,
                                            備考 = model.Nyukins[idx].備考
                                        }
                                        , tx);
                                }
                            }

                            if (model.Nyukins[idx].削除FLG)
                            {
                                var queryDelete = "DELETE FROM T_入金 WHERE 見積ID=@見積ID and 履歴番号=@履歴番号 and 入金ID=@入金ID";

                                var delete = connection.Execute(queryDelete,
                                    new
                                    {
                                        入金ID = model.Nyukins[idx].入金ID,
                                        見積ID = model.見積ID,
                                        履歴番号 = model.履歴番号
                                    }, tx);
                            }

                        }
                        //入金の登録後に見積テーブルの更新があるかもしれない。仕様変更のために残しておく

                        //var queryUpdate = "UPDATE T_見積 SET 見積番号=@見積番号,最新FLG=@最新FLG,担当ID=@担当ID," +
                        //    "顧客ID=@顧客ID,顧客名=@顧客名,敬称=@敬称,件名=@件名,納期=@納期,項目=@項目," +
                        //    "見積区分=@見積区分, 支払条件=@支払条件,有効期限=@有効期限," +
                        //    "見積金額=@見積金額,見込原価=@見込原価,見込利益=@見込利益," +
                        //    "作成日=@作成日,完了予定日=@完了予定日,受注確度=@受注確度,time_stamp=@time_stamp, " +
                        //    "非課税名称=@非課税名称,非課税額=@非課税額,値引名称=@値引名称,値引額=@値引額," +
                        //    "備考=@備考,single=@single,入金種別=@入金種別,入金日=@入金日,入金締日=@入金締日," +
                        //    "見積ステータス=@見積ステータス,取引年月日=@取引年月日 " +
                        //    " WHERE 見積ID = @見積ID and 履歴番号 =@履歴番号";

                        //var result2 = connection.Execute(queryUpdate, model.Quotes, tx);


                        tx.Commit();

                        return result;

                    }
                    catch (Exception ex)
                    {

                        tx.Rollback();
                        return -1;
                    }
                }
            }
        }

        public void DeleteNyukin(int 見積ID, int 履歴番号)
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


