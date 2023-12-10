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
    public interface ITantouService
    {
        IList<担当> SearchTantous(TantouSearchConditions conditions);
        担当 GetTantou(int TantouID);

        IList<担当営業所> GetTantouOffice(int TantouID);
        担当 LoginTantou(string loginID);

        void DeleteTantou(int TantouID);
        public 担当 RegistTantou(TantouDetailModel model);

        public void RegistTantouOffice(TantouDetailModel model);
    }

    public class TantouService : ServiceBase, ITantouService
    {
        private readonly ILogger<TantouService> _logger;

        public TantouService(ILogger<TantouService> logger, IConfiguration configuration)
            : base(configuration)
        {
            _logger = logger;
        }

        public IList<担当> SearchTantous(TantouSearchConditions conditions)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string WhereStr = " where 1=1 ";
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT T_担当.* FROM T_担当 " +
                    "  /**where**/ /**orderby**/ ");

                if (conditions != null)
                {

                    if (!string.IsNullOrEmpty(conditions.氏名))
                    {
                        builder.Where("氏名 like @氏名", new { 氏名 = $"%{conditions.氏名}%" });
                    }
                                       

                    if (!string.IsNullOrEmpty(conditions.tel))
                    {
                        builder.Where("tel like @tel", new { tel = $"%{conditions.tel}%" });
                    }

                    if (conditions.退職者検索 != true)
                    {
                        builder.Where("(del_date is null or del_date > @today)", new { today = DateTime.Now });
                    }

                }

                var result = connection.Query<担当>(template.RawSql, template.Parameters);
                return result.ToList();
            }
        }

        public 担当 GetTantou(int TantouID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT T_担当.* FROM T_担当 " +
                    "/**where**/");

                builder.Where("担当ID = @担当ID", new { 担当ID = TantouID });

                var result = connection.Query<担当>(template.RawSql, template.Parameters).FirstOrDefault();
                
                return result;
            }
        }

        public IList<担当営業所> GetTantouOffice(int TantouID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT T_担当営業所.*,T_営業所.営業所名 FROM T_担当営業所 left join T_営業所 on T_担当営業所.営業所ID = T_営業所.営業所ID " +
                    "/**where**/");

                builder.Where("T_担当営業所.担当ID = @担当ID", new { 担当ID = TantouID });

                var result = connection.Query<担当営業所>(template.RawSql, template.Parameters).ToList();

                return result;
            }
        }


        public 担当 LoginTantou(string loginID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT T_担当.* FROM T_担当 " +
                    "/**where**/");

                builder.Where("loginID = @loginID", new { loginID = loginID });
                builder.Where("(del_date is null or del_date > @today)", new { today = DateTime.Now });

                var result = connection.Query<担当>(template.RawSql, template.Parameters).FirstOrDefault();

                return result;
            }
        }

        public 担当 RegistTantou(TantouDetailModel model)
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
                            sql = "UPDATE  T_担当 Set 氏名=@氏名,auth=@auth,admin=@admin,loginID=@loginID," +
                                "pass=@pass,tel=@tel,イニシャル=@イニシャル,営業所ID=@営業所ID," +
                                "del_date=@del_date,assistant=@assistant " +
                                " where 担当ID = @担当ID";

                            var update = connection.Execute(sql, model.Tantou, tx);
                            tx.Commit();
                            return GetTantou(model.Tantou.担当ID);
                        }
                        else
                        { //新規モード
                            sql = "INSERT INTO T_担当 (氏名,auth,admin,loginID,pass,tel,イニシャル,営業所ID,del_date,assistant)" +
                                " VALUES (@氏名,@auth,@admin,@loginID,@pass,@tel,@イニシャル,@営業所ID,@del_date,@assistant)";

                            var insert = connection.Execute(sql, model.Tantou, tx);
                            tx.Commit();
                            var NewId = (int)connection.Query("SELECT @@IDENTITY as ID").First().ID;
                            return GetTantou(NewId);
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


        public void RegistTantouOffice(TantouDetailModel model)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var tx = connection.BeginTransaction())
                {
                    try
                    {
                        var queryDelete = "DELETE FROM T_担当営業所 WHERE 担当ID=" + model.Tantou.担当ID;

                        var delete = connection.Execute(queryDelete, null, tx);


                        if (model.担当営業所リスト != null)
                        {
                            foreach (var item in model.担当営業所リスト)
                            {
                                var queryInsert2 = "INSERT INTO T_担当営業所 (担当ID,営業所ID) VALUES (" + model.Tantou.担当ID + ",@営業所ID)";

                                var Insert2 = connection.Execute(queryInsert2, item, tx);



                            }
                        }

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

        public void DeleteTantou(int TantouID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var tx = connection.BeginTransaction())
                {
                    try
                    {
                        var sql = "";

                        sql = "DELETE FROM T_担当  WHERE 担当ID =" + TantouID;

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
