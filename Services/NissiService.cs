using Dapper;
using Google.Apis.Util;
using Microsoft.Data.SqlClient;
using System.Data;
using rehome.Enums;
using rehome.Models;
using rehome.Models.Nissi;
using rehome.Models.DB;
using X.PagedList;

namespace rehome.Services
{
    public interface INissiService
    {
        //IPagedList<日誌> IdListSearchNissis(NissiSearchConditions conditions, int page_size);
        IList<日誌> GetNissis(int? 顧客ID);
                
        NissiIndexModel SearchNissis(NissiSearchConditions conditions);


        NissiDetailModel GetNissi(int? NissiID);    
        int RegistNissi(NissiDetailModel model);
                
        void DeleteNissi(int NissiID);

      //  NissiKobetuDetailModel GetNissiKobetu(int? NissiID);
       // int RegistNissiKobetu(NissiKobetuDetailModel model);

     //   void RegistNissiKobetuSodan(NissiKobetuDetailModel model);
      //  void RegistNissiKobetuTantou(NissiKobetuDetailModel model);
        void DeleteNissiKobetu(int NissiID);

       // NissiTokubetuDetailModel GetNissiTokubetu(int? NissiID);
       // int RegistNissiTokubetu(NissiTokubetuDetailModel model);

       // void RegistNissiTokubetuSodan(NissiTokubetuDetailModel model);
       // void RegistNissiTokubetuTantou(NissiTokubetuDetailModel model);
        void DeleteNissiTokubetu(int NissiID);
    }

    public class NissiService : ServiceBase, INissiService
    {
        private readonly ILogger<NissiService> _logger;

        public NissiService(ILogger<NissiService> logger, IConfiguration configuration)
            : base(configuration)
        {
            _logger = logger;
        }

        //public IPagedList<日誌> IListSearchNissis(NissiSearchConditions conditions, int page_size)
        //{
        //    return SearchNissis(conditions).ToPagedList(conditions.page, page_size);
        //}


//**************************************** 一覧・検索 ********************************************************************

        //顧客画面の日誌一覧の取得
        public IList<日誌> GetNissis(int? 顧客ID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT RT_日誌.*,T_担当.氏名 FROM RT_日誌 left join T_担当 " +
                    "on RT_日誌.担当ID=T_担当.担当ID " +
                    "/**where**/ " +
                    "order by 対応日 desc");
                builder.Where("顧客ID = @顧客ID", new { 顧客ID = 顧客ID });

                var result = connection.Query<日誌>(template.RawSql, template.Parameters).ToList();

               
                return result;
            }
        }
        

        public NissiIndexModel SearchNissis(NissiSearchConditions conditions)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string WhereStr = "";
                bool joukenflg = false;
                connection.Open();
                var nissiIndexModel = new NissiIndexModel();
                var builder = new SqlBuilder();
                var Outer_builder = new SqlBuilder();
                //var template = builder.AddTemplate(
                var unionQuery = builder.AddTemplate(
     "SELECT distinct * FROM " +
     "(SELECT T_日誌.日誌ID, '相談' as 支援区分, 対応日, 相談内容区分1, 相談内容区分2, " +
     "相談内容_質問内容 as 内容, B.顧客名, T_日誌.備考," +
     "業務区分,  相談手段,相談内容_運営状況, 対応内容, " +
     "'' as 入院基本料,'' as 事前調整事項,'' as 対応者立場,'' as 対応者関係,'' as 対応者姿勢,'' as 対応者関心,'' as 訪問成果,'' as 支援課題,'' as 支援提案,'' as 個別支援特記事項, " +
     "'' as 取組状況区分,'' as 取組状況,'' as 労働時間課題,'' as 医療機関感想,'' as その他特記事項, " +
     "'' as 実施場所,'' as 当日議題,'' as 資料医療機関,'' as 資料勤改センター,'' as 課題対応医療機関,'' as 課題対応勤改センター,'' as 支援反応,'' as 支援成果," +
     "'' as PDCA,'' as step1,'' as step2,'' as step3,'' as step4,'' as step5,'' as step6,'' as step7,'' as 特別支援特記事項,'' as todo医療機関,'' as todo勤改センター,'' as 相談事項,'' as その他,'' as 次回訪問 " +
     "FROM T_日誌  left join RT_顧客 B on T_日誌.顧客ID = b.顧客ID " +
     "left join T_日誌担当 on T_日誌担当.日誌ID = T_日誌.日誌ID and T_日誌担当.支援区分 = '相談' " +
     " /**where**/ " +

     "UNION SELECT T_個別支援日誌.日誌ID, '個別支援' as 支援区分, 対応日, 相談内容区分1, 相談内容区分2, " +
     "訪問目的 as 内容, B.顧客名,T_個別支援日誌.備考," +
     "'' as 業務区分,'' as 相談手段, '' as 相談内容_運営状況, '' as 対応内容, " +
     "入院基本料,事前調整事項,対応者立場,対応者関係,対応者姿勢,対応者関心,訪問成果,支援課題,支援提案,特記事項 as 個別支援特記事項, " +
     "取組状況区分,取組状況,労働時間課題,医療機関感想,その他特記事項, " +
     "'' as 実施場所,'' as 当日議題,'' as 資料医療機関,'' as 資料勤改センター,'' as 課題対応医療機関,'' as 課題対応勤改センター,'' as 支援反応,'' as 支援成果," +
     "'' as PDCA,'' as step1,'' as step2,'' as step3,'' as step4,'' as step5,'' as step6,'' as step7,'' as 特別支援特記事項,'' as todo医療機関,'' as todo勤改センター,'' as 相談事項,'' as その他,'' as 次回訪問 " +
     "FROM T_個別支援日誌  left join RT_顧客 B on T_個別支援日誌.顧客ID = b.顧客ID " +
     "left join T_日誌担当 on T_日誌担当.日誌ID = T_個別支援日誌.日誌ID and T_日誌担当.支援区分 = '個別支援' " +
     " /**where**/ " +

     "UNION SELECT T_特別支援日誌.日誌ID, '特別支援' as 支援区分, 対応日, 相談内容区分1, 相談内容区分2," +
     "主な発言 as 内容, B.顧客名, T_特別支援日誌.備考," +
     "'' as 業務区分,'' as 相談手段, '' as 相談内容_運営状況, '' as 対応内容, " +
     "'' as 入院基本料,'' as 事前調整事項,'' as 対応者立場,'' as 対応者関係,'' as 対応者姿勢,'' as 対応者関心,'' as 訪問成果,'' as 支援課題,'' as 支援提案,'' as 個別支援特記事項, " +
     "'' as 取組状況区分,'' as 取組状況,'' as 労働時間課題,'' as 医療機関感想,'' as その他特記事項, " +
     "実施場所,当日議題,資料医療機関,資料勤改センター,課題対応医療機関,課題対応勤改センター,支援反応,支援成果," +
     "PDCA,step1,step2,step3,step4,step5,step6,step7,特記事項 as 特別支援特記事項,todo医療機関,todo勤改センター,相談事項,その他,次回訪問 " +
     "FROM T_特別支援日誌  left join RT_顧客 B on T_特別支援日誌.顧客ID = b.顧客ID " +
     "left join T_日誌担当 on T_日誌担当.日誌ID = T_特別支援日誌.日誌ID and T_日誌担当.支援区分 = '特別支援' " +
     " /**where**/ " +
     ") as T ");//order by 対応日 desc");
                
                if (conditions != null)
                {
                    //if (conditions.日誌ID != null)
                    //{
                    //    builder.Where("日誌ID= @日誌ID", new { 日誌ID = conditions.日誌ID });
                    //    WhereStr += " and T_日誌.日誌ID=" + conditions.日誌ID;
                    //}
                    if (conditions.担当ID != null)
                    {
                        builder.Where("担当ID= @担当ID", new { 担当ID = conditions.担当ID });
                        //WhereStr += " and 担当ID=" + conditions.担当ID;
                        joukenflg = true;
                    }
 
                    if (conditions.対応日_start != null)
                    {
                        builder.Where("対応日>= @対応日_start", new { 対応日_start = conditions.対応日_start });
                        //WhereStr += " and 対応日>='" + conditions.対応日_start + "'";
                        joukenflg = true;
                    }
                    if (conditions.対応日_end != null)
                    {
                        builder.Where("対応日<= @対応日_end", new { 対応日_end = conditions.対応日_end });
                        //WhereStr += " and 対応日<='" + conditions.対応日_end + "'";
                        joukenflg = true;
                    }
                    if (!string.IsNullOrEmpty(conditions.対応内容))
                    {
                        builder.Where("対応内容 like @対応内容", new { 対応内容 = $"%{conditions.対応内容}%" });
                        //WhereStr += " and (対応内容 like '%" + conditions.対応内容 + "%')";
                        joukenflg = true;
                    }            
                    if (!string.IsNullOrEmpty(conditions.顧客名))
                    {
                        builder.Where("顧客名 like @顧客名", new { 顧客名 = $"%{conditions.顧客名}%" });
                        //WhereStr += " and (顧客名 like '%" + conditions.顧客名 + "%')";
                        joukenflg = true;
                    }
                    if (!string.IsNullOrEmpty(conditions.相談内容区分))
                    {
                        builder.Where("(相談内容区分1 like @相談内容区分 or 相談内容区分2 like @相談内容区分)", new { 相談内容区分 = $"%{conditions.相談内容区分}%" });
                        joukenflg = true;
                    }
                }

                // ストアドプロシージャの実行
                //var storedProcedure = "PrcAllNissi";
                //var param = new DynamicParameters();
                //param.Add("@WhereStr", WhereStr);
                //var result = connection.Query<日誌>(storedProcedure, param, commandType: CommandType.StoredProcedure);

                string sql = unionQuery.RawSql;

                if (!string.IsNullOrEmpty(conditions.支援区分))
                {
                    //builder.Where("支援区分= @支援区分", new { 支援区分 = conditions.支援区分 });
                    WhereStr += " and 支援区分='" + conditions.支援区分 + "'";
                }
                if (!string.IsNullOrEmpty(conditions.相談手段))
                {
                    //builder.Where("相談手段= @相談手段", new { 相談手段 = conditions.相談手段 });
                    WhereStr += " and 相談手段='" + conditions.相談手段 + "'";
                    joukenflg = true;
                }
                if (!string.IsNullOrEmpty(conditions.相談内容_運営状況))
                {
                    //builder.Where("相談内容_運営状況 like @相談内容_運営状況", new { 相談内容_運営状況 = $"%{conditions.相談内容_運営状況}%" });
                    WhereStr += " and (相談内容_運営状況 like '%" + conditions.相談内容_運営状況 + "%')";
                    joukenflg = true;
                }
                if (!string.IsNullOrEmpty(conditions.相談内容_質問内容))
                {
                    //builder.Where("相談内容_質問内容 like @相談内容_質問内容", new { 相談内容_質問内容 = $"%{conditions.相談内容_質問内容}%" });
                    WhereStr += " and (相談内容_質問内容 like '%" + conditions.相談内容_質問内容 + "%')";
                    joukenflg = true;
                }
                var template = builder.AddTemplate(unionQuery.RawSql + " where 1=1 " + WhereStr + " order by 対応日 desc");


                if (joukenflg == false && WhereStr=="")
                {
                    nissiIndexModel.mess = "検索条件を入れてください";
                    return nissiIndexModel;
                }

                nissiIndexModel.Nissis = connection.Query<日誌表示>(template.RawSql, template.Parameters).ToList();


                if (nissiIndexModel.Nissis.Count() >0 )
                {
                    foreach (var item in nissiIndexModel.Nissis)
                    {
                        var builder2 = new SqlBuilder();
                        var template2 = builder2.AddTemplate("SELECT T_担当.* FROM T_日誌担当 INNER JOIN T_担当 on T_日誌担当.担当ID = T_担当.担当ID /**where**/ order by T_日誌担当.担当ID");
                        builder2.Where("T_日誌担当.日誌ID = @日誌ID and T_日誌担当.支援区分 = @支援区分", new { 日誌ID = item.日誌ID, 支援区分 = item.支援区分 });
                        item.担当リスト = connection.Query<担当>(template2.RawSql, template2.Parameters).ToList();

                    }
                }
                else
                {
                    nissiIndexModel.mess = "検索の対象が見つかりませんでした";
                }
                return nissiIndexModel;

            }
        }


        //******** 日誌詳細画面情報の取得 *******

        public NissiDetailModel GetNissi(int? NissiID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                NissiDetailModel model =new NissiDetailModel();
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT RT_日誌.*, RT_顧客.顧客名 FROM RT_日誌 " +
                    "left join RT_顧客 on RT_日誌.顧客ID=RT_顧客.顧客ID /**where**/");
                builder.Where("日誌ID = @日誌ID", new { 日誌ID = NissiID });

                model.Nissi = connection.Query<日誌>(template.RawSql, template.Parameters).FirstOrDefault();
                
                return model;
            }
        }


        
        public int RegistNissi(NissiDetailModel model)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var tx = connection.BeginTransaction())
                {
                    try
                    {
                        var sql = "";
                        if (model.Mode != ViewMode.New)//更新モード
                        {
                            sql = "UPDATE  RT_日誌 Set 顧客ID=@顧客ID,日誌区分=@日誌区分," +
                                  "対応日=@対応日,登録日=@登録日,内容=@内容," +
                                  "担当ID=@担当ID " +
                                  " WHERE 日誌ID = @日誌ID";

                            var update = connection.Execute(sql, model.Nissi, tx);
                            tx.Commit();
                            return (int)model.Nissi.日誌ID;
                        }
                        else
                        { //新規モード
                            sql = "INSERT INTO RT_日誌 (顧客ID,日誌区分,対応日,登録日,内容,担当ID)" +
                                " VALUES (@顧客ID,@日誌区分,@対応日,@登録日,@内容,@担当ID)";

                            var insert = connection.Execute(sql, model.Nissi, tx);
                            tx.Commit();
                            var NewId = (int)connection.Query("SELECT @@IDENTITY as ID").First().ID;

                            return (NewId);
                        }
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        
        public void DeleteNissi(int NissiID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var tx = connection.BeginTransaction())
                {
                    try
                    {
                        var sql = "";

                        sql = "DELETE FROM T_日誌  WHERE 日誌ID =" + NissiID;

                        var delete = connection.Execute(sql, null, tx);

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

        public void DeleteNissiKobetu(int NissiID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var tx = connection.BeginTransaction())
                {
                    try
                    {
                        var sql = "";

                        sql = "DELETE FROM T_個別支援日誌  WHERE 日誌ID =" + NissiID;

                        var delete = connection.Execute(sql, null, tx);

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



        public void DeleteNissiTokubetu(int NissiID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var tx = connection.BeginTransaction())
                {
                    try
                    {
                        var sql = "";

                        sql = "DELETE FROM T_特別支援日誌  WHERE 日誌ID =" + NissiID;

                        var delete = connection.Execute(sql, null, tx);

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

    }
}
