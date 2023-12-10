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
    public interface IHouzinService
    {
        法人 GetHouzin();
        public 法人 RegistHouzin(HouzinDetailModel model);

        public int RegistSetting(設定 model);
    }

    public class HouzinService : ServiceBase, IHouzinService
    {
        private readonly ILogger<HouzinService> _logger;

        public HouzinService(ILogger<HouzinService> logger, IConfiguration configuration)
            : base(configuration)
        {
            _logger = logger;
        }

      

        public 法人 GetHouzin()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT T_法人.* FROM T_法人 ");


                var result = connection.Query<法人>(template.RawSql, template.Parameters).FirstOrDefault();
                
                return result;
            }
        }



        public 法人 RegistHouzin(HouzinDetailModel model)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var tx = connection.BeginTransaction())
                {
                    try
                    {
                        var sql = "";

                        sql = "UPDATE  T_法人 Set 社名=@社名,代表名=@代表名,郵便番号=@郵便番号,住所=@住所,TEL=@TEL,FAX=@FAX";

                        var update = connection.Execute(sql, model.Houzin, tx);
                        tx.Commit();
                        return GetHouzin();
                        
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        public int RegistSetting(設定 model)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var tx = connection.BeginTransaction())
                {
                    try
                    {
                        var sql = "";

                        sql = "UPDATE  T_設定 Set 期=@期";

                        var update = connection.Execute(sql, model, tx);
                        tx.Commit();
                        return (int)model.期;

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
