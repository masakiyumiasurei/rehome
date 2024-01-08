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
    public interface IPayService
    {
        public IList<仕入帳> SearchPay(PaySearchConditions conditions);

        仕入帳 GetPay(int 仕入ID);

        Help GetHelp();

        Help RegistHelp(Help model);

        仕入帳 RegistPay(PayCreateModel model);

        void DeletePay(int 仕入ID); 
        
 
    }

    public class PayService : ServiceBase, IPayService
    {
        private readonly ILogger<QuoteService> _logger;

        public PayService(ILogger<QuoteService> logger, IConfiguration configuration)
            : base(configuration)
        {
            _logger = logger;
        }


        public IList<仕入帳> SearchPay(PaySearchConditions conditions)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string WhereStr = "";
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT * " +
                    "FROM T_仕入帳 T LEFT JOIN  T_仕入先 T2 ON T.仕入先ID = T2.仕入先ID " +
                    " /**where**/ /**orderby**/");

                builder.OrderBy("T.日付 DESC");
                //開発中ログインID付与　注意！！
                //conditions.LoginID = 1;
                             

                if (conditions != null)
                {

                    //if (conditions.営業所ID != null)
                    //{
                    //    builder.Where("T_見積.営業所ID= @営業所ID", new { 営業所ID = conditions.営業所ID });

                    //}
                    //if (conditions.担当ID != null)
                    //{
                                 
                    //}
                  
                    if (!string.IsNullOrEmpty(conditions.仕入先名))
                    {
                        builder.Where("T2.仕入先名 like @仕入先名", new { 仕入先名 = $"%{ conditions.仕入先名}%" });

                    }                                      

                    

                    if (conditions.日付start != null)
                    {
                        builder.Where("T.日付>= @日付start", new { 日付start = conditions.日付start });

                    }
                    if (conditions.日付end != null)
                    {
                        builder.Where("T.日付<= @日付end", new { 日付end = conditions.日付end });
    
                    }

                }

                var result = connection.Query<仕入帳>(template.RawSql, template.Parameters);

                return result.ToList();
            }
        }
         
        public 仕入帳 GetPay(int 仕入ID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT * " +
                    "FROM T_仕入帳 T LEFT JOIN  T_仕入先 T2 ON T.仕入先ID = T2.仕入先ID " +
                    " /**where**/ ");

                builder.Where("T.仕入ID = @仕入ID ", new { 仕入ID = 仕入ID});

                var result = connection.Query<仕入帳>(template.RawSql, template.Parameters).FirstOrDefault();

                return result;
            }           
        }

        public Help GetHelp()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT * " +
                    "FROM T_Help"); 
                //    " /**where**/ ");

                //builder.Where();

                var result = connection.Query<Help>(template.RawSql).FirstOrDefault();

                return result;
            }
        }

        public Help RegistHelp(Help model)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var tx = connection.BeginTransaction())
                {
                    try
                    {
                        var sql = "";
                        sql = "update T_Help set help=@help ";
                            
                        var result = connection.Execute(sql,  model , tx);
                        tx.Commit();
                        return GetHelp();
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

 

        public 仕入帳 RegistPay(PayCreateModel model)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                

                connection.Open();
                using (var tx = connection.BeginTransaction())
                {
                    try
                    {
                        var sql = "";                       

                        if (model.Mode == ViewMode.New)
                        {
                            //新規
                            var queryInsert = "INSERT INTO T_仕入帳 (仕入先ID,金額,消費税,交通費,値引等,相手負担,当社負担,日付,time_stamp) " +
                        "VALUES (@仕入先ID,@金額,@消費税,@交通費,@値引等,@相手負担,@当社負担,@日付,getdate())";

                            var insert = connection.Execute(queryInsert, model.Pay, tx);
                            tx.Commit();
                            var NewId = (int)connection.Query("SELECT @@IDENTITY as ID").First().ID;
                            return GetPay(NewId);
                            
                        }
                        else 
                        {
                            //更新
                            var queryUpdate = "UPDATE T_仕入帳 SET 仕入先ID=@仕入先ID,金額=@金額," +
                                "消費税=@消費税,交通費=@交通費,値引等=@値引等,相手負担=@相手負担,当社負担=@当社負担,日付=@日付,time_stamp = getdate()" +
                                " WHERE 仕入ID = @仕入ID";

                            var result = connection.Execute(queryUpdate, model.Pay, tx);
                            tx.Commit();
                            return GetPay(model.Pay.仕入ID);

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

        public void DeletePay(int 仕入ID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var tx = connection.BeginTransaction())
                {
                    try
                    {
                        //削除
                        var queryDelete = "DELETE FROM T_仕入帳 WHERE 仕入ID = " + 仕入ID ;
                        var delete = connection.Execute(queryDelete, null, tx);

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


