using Dapper;
using Google.Apis.Util;
using Microsoft.Data.SqlClient;
using System.Data;
using rehome.Enums;
using rehome.Models;
using rehome.Models.DB;
using X.PagedList;
using rehome.Public;

namespace rehome.Services
{
    public interface IBunruiService
    {
        IList<分類> SearchBunruis(BunruiSearchConditions conditions);
        分類 GetBunrui(int BunruiID);

        void DeleteBunrui(int BunruiID);
        public 分類 RegistBunrui(BunruiDetailModel model);
    }

    public class BunruiService : ServiceBase, IBunruiService
    {
        private readonly ILogger<BunruiService> _logger;

        public BunruiService(ILogger<BunruiService> logger, IConfiguration configuration)
            : base(configuration)
        {
            _logger = logger;
        }

        public IList<分類> SearchBunruis(BunruiSearchConditions conditions)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string WhereStr = " where 1=1 ";
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT * FROM T_分類" +
                    "  /**where**/ order by ソート ");

                if (conditions != null)
                {

                    if (!string.IsNullOrEmpty(conditions.分類名))
                    {
                        builder.Where("分類名 like @分類名", new { 分類名 = $"%{conditions.分類名}%" });
                    }

                    if (!string.IsNullOrEmpty(conditions.理化学医療区分))
                    {
                        builder.Where("理化学医療区分 = @理化学医療区分", new { 理化学医療区分 = conditions.理化学医療区分 });
                    }



                }

                var result = connection.Query<分類>(template.RawSql, template.Parameters);
                return result.ToList();
            }
        }

        public 分類 GetBunrui(int BunruiID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT T_分類.* FROM T_分類 " +
                    "/**where**/");

                builder.Where("分類ID = @ID", new { ID = BunruiID });

                var result = connection.Query<分類>(template.RawSql, template.Parameters).FirstOrDefault();
                
                return result;
            }
        }



        public 分類 RegistBunrui(BunruiDetailModel model)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var tx = connection.BeginTransaction())
                {
                    try
                    {

                        var builder = new SqlBuilder();
                        var template = builder.AddTemplate("SELECT T_分類.* FROM T_分類 " +
                            "/**where**/");
                        builder.Where("ソート = @ソート", new { ソート = model.Bunrui.ソート });
                        if(model.Bunrui.分類ID != 0)
                        {
                            builder.Where("分類ID <> @分類ID", new { 分類ID = model.Bunrui.分類ID });
                        }
                        var result = connection.Query<分類>(template.RawSql, template.Parameters,tx).FirstOrDefault();
                        if(result != null)
                        {
                            var sql2 = "";
                            sql2 = "UPDATE  T_分類 Set ソート= (ソート + 1) where ソート >= @ソート";
                            var update2 = connection.Execute(sql2, model.Bunrui, tx);
                        }

                        var sql = "";
                        if (model.Mode != ViewMode.New)//更新モード
                        {
                            sql = "UPDATE  T_分類 Set 分類名=@分類名,理化学医療区分=@理化学医療区分,ソート=@ソート where 分類ID = @分類ID";

                            var update = connection.Execute(sql, model.Bunrui, tx);
                            tx.Commit();
                            return GetBunrui(model.Bunrui.分類ID);
                        }
                        else
                        { //新規モード
                            sql = "INSERT INTO T_分類 (分類名,理化学医療区分,ソート)" +
                                " VALUES (@分類名,@理化学医療区分,@ソート)";

                            var insert = connection.Execute(sql, model.Bunrui, tx);
                            tx.Commit();
                            var NewId = (int)connection.Query("SELECT @@IDENTITY as ID").First().ID;
                            return GetBunrui(NewId);
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


        public void DeleteBunrui(int BunruiID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var tx = connection.BeginTransaction())
                {
                    try
                    {
                        var sql = "";



                        sql = "DELETE FROM T_分類  WHERE 分類ID =" + BunruiID;

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
