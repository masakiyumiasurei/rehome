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
     "SELECT distinct 日誌ID,  日誌区分, 対応日, 内容,  B.顧客名,氏名 as 担当名 FROM " +
     "FROM RT_日誌  left join RT_顧客 B on RT_日誌.顧客ID = b.顧客ID " +
     "left join T_担当 on T_担当.担当ID = T_日誌.担当ID " +
     " /**where**/ " +
     "order by 対応日 desc");
                
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
                        builder.Where("内容 like @内容", new { 内容 = $"%{conditions.対応内容}%" });
                        //WhereStr += " and (対応内容 like '%" + conditions.対応内容 + "%')";
                        joukenflg = true;
                    }            
                    if (!string.IsNullOrEmpty(conditions.顧客名))
                    {
                        builder.Where("顧客名 like @顧客名", new { 顧客名 = $"%{conditions.顧客名}%" });
                        //WhereStr += " and (顧客名 like '%" + conditions.顧客名 + "%')";
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
                
                var template = builder.AddTemplate(unionQuery.RawSql + " where 1=1 " + WhereStr + " order by 対応日 desc");


                if (joukenflg == false && WhereStr=="")
                {
                    nissiIndexModel.mess = "検索条件を入れてください";
                    return nissiIndexModel;
                }

                nissiIndexModel.Nissis = connection.Query<日誌>(template.RawSql, template.Parameters).ToList();


                if (nissiIndexModel.Nissis.Count() ==0 )
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
