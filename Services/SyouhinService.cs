using Dapper;
using Microsoft.Data.SqlClient;
using rehome.Enums;
using rehome.Models;
using rehome.Models.DB;
using X.PagedList;

namespace rehome.Services
{
    public interface ISyouhinService
    {
        IPagedList<商品> SearchSyouhins(SyouhinSearchConditions conditions, int page_size);
        IList<商品> SearchAjaxSyouhins(SyouhinSearchConditions conditions);
        商品 GetSyouhin(int SyouhinId);

        List<商品> GetKakaku(string rank);
        IList<商品> ListSyouhins();
        int RegistSyouhin(SyouhinDetailModel model);
    }

    public class SyouhinService : ServiceBase, ISyouhinService
    {
        private readonly ILogger<SyouhinService> _logger;

        public SyouhinService(ILogger<SyouhinService> logger, IConfiguration configuration)
            : base(configuration)
        {
            _logger = logger;
        }

        public IPagedList<商品> SearchSyouhins(SyouhinSearchConditions conditions, int page_size)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT * FROM T_商品 /**where**/ ");
                //builder.Where("削除フラグ = 0");
                if (conditions != null)
                {
                    if (conditions.商品ID != null)
                    {
                        builder.Where("商品ID= @商品ID", new { 商品ID = conditions.商品ID });
                    }
                    if (!string.IsNullOrEmpty(conditions.商品名))
                    {
                        builder.Where("商品名 like @商品名", new { 商品名 = $"%{conditions.商品名}%" });
                    }                    
                    if (conditions.削除フラグ == false)
                    {
                        builder.Where("coalesce(削除フラグ,0) = 0");
                    }
                }
                builder.OrderBy("商品ID asc");//仮に並び順は商品ID
                var result = connection.Query<商品>(template.RawSql, template.Parameters);

                return result.ToPagedList(conditions.page, page_size);
            }
        }
        public IList<商品> SearchAjaxSyouhins(SyouhinSearchConditions conditions)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT * FROM T_商品 /**where**/");
                
                if (conditions != null)
                {
                    if (conditions.商品ID != null)
                    {
                        builder.Where("商品ID= @商品ID", new { 商品ID = conditions.商品ID });
                    }
                    if (!string.IsNullOrEmpty(conditions.商品名))
                    {
                        builder.Where("商品名 like @商品名", new { 商品名 = $"%{conditions.商品名}%" });
                    }
                    
                    builder.Where("coalesce(削除フラグ,0) = 0");
                }
                builder.OrderBy("商品ID asc");//仮に並び順は商品ID
                var result = connection.Query<商品>(template.RawSql, template.Parameters);

                return result.ToList();
            }
        }

        public 商品 GetSyouhin(int SyouhinId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT * FROM T_商品 /**where**/");
                //builder.Where("削除フラグ = 0");
                builder.Where("商品ID = @商品ID", new { 商品ID = SyouhinId });

                var result = connection.Query<商品>(template.RawSql, template.Parameters).FirstOrDefault();
               
                return result;
            }
        }

        //価格表　where条件は変更する予定
        public List<商品> GetKakaku(string rank)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT 品番 ,商品名,'P-' + cast(カタログ as nvarchar) as 表示カタログ," +
                    //"case when openFLG=1 then 'OPEN' " +
                    //"else 定価 end AS 定価," +                    
                    "case '" + rank + "'" +
                    "when 'A' then 金額A " +
                    "when 'B' then 金額B " +
                    "when 'C' then 金額C end as 仕切価格 , 定価, " +
                    "FLOOR(定価 * 0.2) AS 保証, " +
                    "openFLG " +
                    "FROM T_商品 /**where**/");
                builder.Where("coalesce(削除フラグ,0) = 0 and カタログ >0 ");
                
                var result = connection.Query<商品>(template.RawSql, template.Parameters);

                return result.ToList();
            }
        }
        public IList<商品> ListSyouhins()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT * FROM T_商品　/**where**/");
                builder.Where("coalesce(削除フラグ,0) = 0");

                var result = connection.Query<商品>(template.RawSql, template.Parameters);
                return result.ToList();
            }
        }


        public int RegistSyouhin(SyouhinDetailModel model)
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
                            sql = "UPDATE  T_商品 Set 商品名=@商品名," +
                              "単価=@単価, 単位=@単位,仕入額=@仕入額," +
                              "削除フラグ=@削除フラグ " +
                              "WHERE 商品ID = @商品ID";
                            var update = connection.Execute(sql, model.Syouhin, tx);

                            tx.Commit();
                            return model.Syouhin.商品ID;
                        }
                        else
                        { //新規モード
                            sql = "INSERT INTO T_商品 (商品名,単価," +
                                "単位,仕入額,削除フラグ )" +
                                "VALUES (@商品名,@単価," +
                                "@単位,@仕入額,@削除フラグ )";
                            var update = connection.Execute(sql, model.Syouhin, tx);
                            tx.Commit();
                            var NewId = (int)connection.Query("SELECT @@IDENTITY as ID").First().ID;
                            return NewId;
                        }
                    }
                    catch (Exception)
                    {
                        tx.Rollback();
                        //return -1;
                        throw;
                    }
                }
            }
        }
    }
}