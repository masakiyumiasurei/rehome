using Dapper;
using Google.Apis.Util;
using Microsoft.Data.SqlClient;
using System.Data;
using rehome.Enums;
using rehome.Models;
using rehome.Models.DB;
using X.PagedList;
using rehome.Public;
using Castle.Core.Internal;

namespace rehome.Services
{
    public interface ISiireService
    {
        IList<仕入先> SearchSiires(SiireSearchConditions conditions);
        仕入先 GetSiire(int SiireID);

        void DeleteSiire(int SiireID);
        public 仕入先 RegistSiire(SiireDetailModel model);
    }

    public class SiireService : ServiceBase, ISiireService
    {
        private readonly ILogger<SiireService> _logger;

        public SiireService(ILogger<SiireService> logger, IConfiguration configuration)
            : base(configuration)
        {
            _logger = logger;
        }

        public IList<仕入先> SearchSiires(SiireSearchConditions conditions)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string WhereStr = " where 1=1 ";
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT * FROM T_仕入先" +
                    "  /**where**/ /**orderby**/ ");

                if (conditions != null)
                {

                    if (!string.IsNullOrEmpty(conditions.仕入先名))
                    {
                        builder.Where("仕入先名 like @仕入先名", new { 仕入先名 = $"%{conditions.仕入先名}%" });
                    }

                    if (!string.IsNullOrEmpty(conditions.肩書))
                    {
                        builder.Where("肩書 like @肩書", new { 肩書 = $"%{conditions.肩書}%" });
                    }

                    if (!string.IsNullOrEmpty(conditions.郵便番号))
                    {
                        builder.Where("郵便番号 like @郵便番号", new { 郵便番号 = $"%{conditions.郵便番号}%" });
                    }

                    if (!string.IsNullOrEmpty(conditions.住所))
                    {
                        builder.Where("住所 like @住所", new { 住所 = $"%{conditions.住所}%" });
                    }

                    if (!string.IsNullOrEmpty(conditions.TEL))
                    {
                        builder.Where("TEL like @TEL", new { TEL = $"%{conditions.TEL}%" });
                    }

                    if (!string.IsNullOrEmpty(conditions.FAX))
                    {
                        builder.Where("FAX like @FAX", new { FAX = $"%{conditions.FAX}%" });
                    }
                    if (!string.IsNullOrEmpty(conditions.業種.ToString()))
                    {
                        builder.Where("業種 = @業種", new { 業種 = conditions.業種 });
                    }
                }

                var result = connection.Query<仕入先>(template.RawSql, template.Parameters);
                return result.ToList();
            }
        }

        public 仕入先 GetSiire(int SiireID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT T_仕入先.* FROM T_仕入先 " +
                    "/**where**/");

                builder.Where("仕入先ID = @ID", new { ID = SiireID });

                var result = connection.Query<仕入先>(template.RawSql, template.Parameters).FirstOrDefault();
                
                return result;
            }
        }



        public 仕入先 RegistSiire(SiireDetailModel model)
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
                            sql = "UPDATE  T_仕入先 Set 仕入先名=@仕入先名,郵便番号=@郵便番号,住所=@住所,肩書=@肩書,TEL=@TEL,FAX=@FAX," +
                                "銀行名=@銀行名,支店名=@支店名,口座区分=@口座区分,口座番号=@口座番号," +
                                "口座名義=@口座名義,インボイス番号=@インボイス番号,業種=@業種," +
                                "支払日=@支払日,分類=@分類,当社負担FLG=@当社負担FLG " +
                                " where 仕入先ID = @仕入先ID";

                            var update = connection.Execute(sql, model.Siire, tx);
                            tx.Commit();
                            return GetSiire(model.Siire.仕入先ID);
                        }
                        else
                        { //新規モード
                            sql = "INSERT INTO T_仕入先 (仕入先名,郵便番号,住所,肩書,TEL,FAX," +
                                "銀行名,支店名,口座区分,口座番号,口座名義,インボイス番号,業種,支払日,分類,当社負担FLG) " +
                                " VALUES (@仕入先名,@郵便番号,@住所,@肩書,@TEL,@FAX," +
                                "@銀行名,@支店名,@口座区分,@口座番号,@口座名義,@インボイス番号,@業種,@支払日,@分類,@当社負担FLG)";

                            var insert = connection.Execute(sql, model.Siire, tx);
                            tx.Commit();
                            var NewId = (int)connection.Query("SELECT @@IDENTITY as ID").First().ID;
                            return GetSiire(NewId);
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


        public void DeleteSiire(int SiireID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var tx = connection.BeginTransaction())
                {
                    try
                    {
                        var sql = "";



                        sql = "DELETE FROM T_仕入先  WHERE 仕入先ID =" + SiireID;

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
