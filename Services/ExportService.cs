using Dapper;
using Microsoft.Data.SqlClient;
using rehome.Enums;
using rehome.Models;
using rehome.Models.DB;
using X.PagedList;

namespace rehome.Services
{
    public interface IExportService
    {
       
        商品 GetSyouhin(int SyouhinId);

        List<商品> GetKakaku(string rank);
        IList<Export仕入帳> ListSiireCho(DateTime startdate, DateTime enddate);

        IList<Export売上表> ListUriage(DateTime startdate, DateTime enddate);
    }

    public class ExportService : ServiceBase, IExportService
    {
        private readonly ILogger<SyouhinService> _logger;

        public ExportService(ILogger<SyouhinService> logger, IConfiguration configuration)
            : base(configuration)
        {
            _logger = logger;
        }
        
        
        public IList<Export仕入帳> ListSiireCho(DateTime startdate, DateTime enddate)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                string sql = "select T.* ,金額 + 消費税 + 交通費 - 値引等 as 合計 ," +
                    "金額 + 消費税 + 交通費 - 値引等 - 相手負担 - 当社負担 as 振出額 from " +
                    "(SELECT MAX(仕入先名) AS 仕入先名, MAX(支払日) AS 支払日, MAX(インボイス番号) AS インボイス番号, MAX(分類) AS 分類, " +
                    "MAX(業種) AS 業種, MAX(銀行名) AS 銀行名, MAX(支店名) AS 支店名, MAX(口座区分) AS 口座区分, MAX(口座番号) AS 口座番号, MAX(口座名義) AS 口座名義, " +
                    "isnull(SUM(金額),0) AS 金額, isnull(SUM(消費税),0) AS 消費税, isnull(SUM(交通費),0) AS 交通費,  " +
                    "isnull(SUM(値引等),0) AS 値引等," +
                    " isnull(SUM(相手負担),0) AS 相手負担,  isnull(SUM(当社負担),0) AS 当社負担 " +
                    "FROM T_仕入帳 LEFT JOIN T_仕入先 ON T_仕入先.仕入先ID = T_仕入帳.仕入先ID " +
                    " /**where**/  GROUP BY T_仕入帳.仕入先ID) as T " +
                    "order by 支払日";
                var template = builder.AddTemplate(sql);
                builder.Where("T_仕入帳.日付 between @startdate and @enddate ", new { startdate = startdate, enddate = enddate });

                var result = connection.Query<Export仕入帳>(template.RawSql, template.Parameters);
                return result.ToList();
            }
        }

        public IList<Export売上表> ListUriage(DateTime startdate, DateTime enddate)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                string sql = "SELECT MONTH(T_見積.請求日) AS 計上月, T_見積.請求日 , T_担当.氏名 AS 担当, " +
                    "T_見積.種類, T_見積.受渡場所 AS 現場名, T_見積.部屋番号, T_見積.種類2, T_見積.JS番号," +
                    " T_見積.件名, T_入金.入金日 , T_入金.入金額, T_入金.振込手数料, T_入金.振込名義, T_入金.前受金, T_入金.備考, T_見積.顧客名 ," +
                    "T_見積.項目 ," +
                    "   CASE T_入金.入金種別 " +
                    "  WHEN 0 THEN '現金'" +
                    "  WHEN 1 THEN '振込'" +
                    "  WHEN 2 THEN 'PayPay'" +
                    "  WHEN 3 THEN 'その他'" +
                    "  WHEN 4 THEN 'カード'" +
                    "  WHEN 5 THEN '小切手'" +
                    "  WHEN 6 THEN '手形'  END AS 入金種別 " +                  
                    "FROM T_見積 LEFT JOIN T_担当 ON T_見積.担当ID = T_担当.担当ID " +
                    "LEFT JOIN T_入金 ON T_見積.見積ID = T_入金.見積ID" +
                    " /**where**/  " +
                    "order by 請求日 ";
                var template = builder.AddTemplate(sql);
                builder.Where("T_見積.請求日 between @startdate and @enddate ", new { startdate = startdate, enddate = enddate });

                var result = connection.Query<Export売上表>(template.RawSql, template.Parameters);
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

    }
}