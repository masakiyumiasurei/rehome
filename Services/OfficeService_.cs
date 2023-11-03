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
    public interface IOfficeService
    {
        IList<営業所> SearchOffices(OfficeSearchConditions conditions);
        営業所 GetOffice(int OfficeID);

        void DeleteOffice(int OfficeID);
        public 営業所 RegistOffice(OfficeDetailModel model);
    }

    public class OfficeService : ServiceBase, IOfficeService
    {
        private readonly ILogger<OfficeService> _logger;

        public OfficeService(ILogger<OfficeService> logger, IConfiguration configuration)
            : base(configuration)
        {
            _logger = logger;
        }

        public IList<営業所> SearchOffices(OfficeSearchConditions conditions)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string WhereStr = " where 1=1 ";
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT * FROM T_営業所" +
                    "  /**where**/ ");

                if (conditions != null)
                {

                    if (!string.IsNullOrEmpty(conditions.営業所名))
                    {
                        builder.Where("営業所名 like @営業所名", new { 営業所名 = $"%{conditions.営業所名}%" });
                    }

                    if (!string.IsNullOrEmpty(conditions.郵便番号))
                    {
                        builder.Where("郵便番号 like @郵便番号", new { 郵便番号 = $"%{conditions.郵便番号}%" });
                    }

                    if (!string.IsNullOrEmpty(conditions.住所))
                    {
                        builder.Where("住所 like @住所", new { 住所 = $"%{conditions.住所}%" });
                    }

                    if (!string.IsNullOrEmpty(conditions.肩書))
                    {
                        builder.Where("肩書 like @肩書", new { 肩書 = $"%{conditions.肩書}%" });
                    }

                    if (!string.IsNullOrEmpty(conditions.TEL))
                    {
                        builder.Where("TEL like @TEL", new { TEL = $"%{conditions.TEL}%" });
                    }

                    if (!string.IsNullOrEmpty(conditions.FAX))
                    {
                        builder.Where("FAX like @FAX", new { FAX = $"%{conditions.FAX}%" });
                    }


                }

                var result = connection.Query<営業所>(template.RawSql, template.Parameters);
                return result.ToList();
            }
        }

        public 営業所 GetOffice(int OfficeID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT T_営業所.* FROM T_営業所 " +
                    "/**where**/");

                builder.Where("営業所ID = @ID", new { ID = OfficeID });

                var result = connection.Query<営業所>(template.RawSql, template.Parameters).FirstOrDefault();
                
                return result;
            }
        }



        public 営業所 RegistOffice(OfficeDetailModel model)
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
                            sql = "UPDATE T_営業所 Set 営業所名=@営業所名,郵便番号=@郵便番号,住所=@住所,肩書=@肩書,TEL=@TEL,FAX=@FAX where 営業所ID = @営業所ID";

                            var update = connection.Execute(sql, model.Office, tx);
                            tx.Commit();
                            return GetOffice(model.Office.営業所ID);
                        }
                        else
                        { //新規モード
                            sql = "INSERT INTO T_営業所 (営業所名,郵便番号,住所,肩書,TEL,FAX)" +
                                " VALUES (@営業所名,@郵便番号,@住所,@肩書,@TEL,@FAX)";

                            var insert = connection.Execute(sql, model.Office, tx);
                            tx.Commit();
                            var NewId = (int)connection.Query("SELECT @@IDENTITY as ID").First().ID;
                            return GetOffice(NewId);
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


        public void DeleteOffice(int OfficeID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var tx = connection.BeginTransaction())
                {
                    try
                    {
                        var sql = "";



                        sql = "DELETE FROM T_営業所  WHERE 営業所ID =" + OfficeID;

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
