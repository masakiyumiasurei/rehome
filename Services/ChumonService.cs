using Dapper;
using Microsoft.Data.SqlClient;
using rehome.Enums;
using rehome.Models;
using rehome.Models.DB;
using X.PagedList;

namespace rehome.Services
{
    public interface IChumonService
    {
        public IList<注文> SearchChumon(int 見積ID, int 履歴番号);

        public ChumonIndexModel CalcChumon(int 見積ID, int 履歴番号);

        public ChumonCreateModel RegistChumon(ChumonCreateModel model);

        public ChumonCreateModel GetChumon(int 注文ID);

        public ChumonCreateModel CopyChumon(int 注文ID);
        
        void DeleteChumon(int 注文ID);

        string GetQuoteNumber(int 営業所ID, int 担当ID, DateTime 作成日);

        IList<見積明細> GetQuoteBunrui(int 見積ID, int 履歴番号);

        IList<見積明細> GetQuoteBunruiMeisai(int 見積ID, int 履歴番号, int 分類ID);

        public int GetMaxeda(int 見積ID, int 履歴番号);
               

    }

    public class ChumonService : ServiceBase, IChumonService
    {
        private readonly ILogger<ChumonService> _logger;
        private int ChumonId;

        public ChumonService(ILogger<ChumonService> logger, IConfiguration configuration)
            : base(configuration)
        {
            _logger = logger;
         }


        public IList<注文> SearchChumon(int 見積ID, int 履歴番号)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string WhereStr = "";
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT * FROM T_注文 left join T_仕入先 on T_注文.仕入先ID=T_仕入先.仕入先ID " +
                    " /**where**/ /**orderby**/");

                builder.Where("見積ID = @見積ID and 履歴番号=@履歴番号", new { 見積ID = 見積ID, 履歴番号 = 履歴番号 });
                builder.OrderBy("枝番");

                var result = connection.Query<注文>(template.RawSql, template.Parameters);

                return result.ToList();
            }
        }

        public ChumonIndexModel CalcChumon(int 見積ID, int 履歴番号)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT sum(金額) as 金額計,sum(原価) as 原価計," +
                    "sum(見込原価) as 見込原価計 FROM T_注文    /**where**/");
           
                builder.Where("見積ID = @見積ID and 履歴番号=@履歴番号", new { 見積ID = 見積ID, 履歴番号 = 履歴番号 });

                var result = connection.Query<ChumonIndexModel>(template.RawSql, template.Parameters).FirstOrDefault();

                return result;
            }
        }

        public ChumonCreateModel GetChumon(int 注文ID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                ChumonCreateModel result = new ChumonCreateModel();
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT T_注文.*,T_仕入先.仕入先名 FROM T_注文 left join T_仕入先 " +
                    " on T_注文.仕入先ID = T_仕入先.仕入先ID  /**where**/");

                builder.Where("注文ID = @注文ID ", new { 注文ID = 注文ID});

                 result.Chumon = connection.Query<注文>(template.RawSql, template.Parameters).FirstOrDefault();

                if (result != null)
                {
                    // 見積明細を取得する
                    builder = new SqlBuilder();
                    template = builder.AddTemplate("SELECT * FROM T_注文明細 /**where**/ ORDER BY 枝番");

                    builder.Where("注文ID = @注文ID ", new { 注文ID = 注文ID });
                    var meisais = connection.Query<注文明細>(template.RawSql, template.Parameters);
                    result.ChumonMeisai = meisais.ToList();//これだとOK 

                }

                return result;
            }
        }

        public ChumonCreateModel CopyChumon(int 注文ID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                ChumonCreateModel result = new ChumonCreateModel();
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT 納期,納入先,弊社担当者, " +
                    "T_仕入先.仕入先名 FROM T_注文 left join T_仕入先 " +
                    " on T_注文.仕入先ID = T_仕入先.仕入先ID  /**where**/");

                builder.Where("注文ID = @注文ID ", new { 注文ID = 注文ID });

                result.Chumon = connection.Query<注文>(template.RawSql, template.Parameters).FirstOrDefault();

                if (result != null)
                {
                    // 見積明細を取得する
                    builder = new SqlBuilder();
                    template = builder.AddTemplate("SELECT 件名,摘要 FROM T_注文明細 /**where**/ ORDER BY 枝番");

                    builder.Where("注文ID = @注文ID ", new { 注文ID = 注文ID });
                    var meisais = connection.Query<注文明細>(template.RawSql, template.Parameters);
                    result.ChumonMeisai = meisais.ToList();//これだとOK 

                }
                return result;
            }
        }

        public int GetMaxeda(int 見積ID, int 履歴番号)
        {
            var connection = new SqlConnection(_connectionString);
                connection.Open();
            int maxeda = 0;
            var template = "SELECT coalesce(max(枝番),0) FROM T_注文  where 見積ID = @見積ID and 履歴番号 = @履歴番号";
            maxeda = connection.Query<int>(template, new { 見積ID = 見積ID, 履歴番号 = 履歴番号 }).FirstOrDefault();
            maxeda += 1;
            return maxeda;
        }

    
        public ChumonCreateModel RegistChumon(ChumonCreateModel model)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var tx = connection.BeginTransaction())
            {
                try
                {   //新規
                    if (model.Mode == ViewMode.New)
                    {
                        //var template = "SELECT max(枝番) FROM T_注文  where 見積ID = @見積ID and 履歴番号 = @履歴番号";
                        //int? maxeda = connection.Query<int?>(template, new { 見積ID = model.Quote.見積ID, 履歴番号 = model.Quote.履歴番号 }, transaction: tx).FirstOrDefault();
                        //maxeda = maxeda ?? 0;

                        var queryInsert = "INSERT INTO T_注文 (見積ID,履歴番号,枝番,仕入先ID,納期,納入先,支払締め," +
                                "支払日,摘要,金額,原価,見込原価,作成日,弊社担当者,自由仕入先名) " +
                                            "VALUES ( @見積ID,@履歴番号,@枝番,@仕入先ID,@納期,@納入先,@支払締め," +
                                            "@支払日,@摘要,@金額,@原価,@見込原価,@作成日,@弊社担当者,@自由仕入先名)";

                        var insert = connection.Execute(queryInsert, new
                        {
                            見積ID = model.Quote.見積ID,
                            履歴番号 = model.Quote.履歴番号,
                            枝番 = model.Chumon.枝番,
                            仕入先ID = model.Chumon.仕入先ID,
                            納期 = model.Chumon.納期,
                            納入先 = model.Chumon.納入先,
                            支払締め = model.Chumon.支払締め,
                            支払日 = model.Chumon.支払日,
                            摘要 = model.Chumon.摘要,
                            金額 = model.Chumon.金額,
                            原価 = model.Chumon.原価,
                            見込原価 = model.Chumon.見込原価,
                            作成日 = model.Chumon.作成日,
                            弊社担当者 = model.Chumon.弊社担当者,
                            自由仕入先名= model.Chumon.自由仕入先名
                        }, tx);

                            ChumonId = (int)connection.Query("SELECT @@IDENTITY as ID", transaction: tx).First().ID;
                    }                        
                    else if (model.Mode == ViewMode.Edit)  //更新処理
                    {           
                        var queryUpdate = "UPDATE T_注文 SET 納期=@納期,納入先=@納入先,支払締め=@支払締め,支払日=@支払日, " +
                                            "金額=@金額, 原価=@原価, 見込原価=@見込原価, 摘要=@摘要 ,作成日=@作成日," +
                                            "弊社担当者=@弊社担当者,自由仕入先名=@自由仕入先名,枝番=@枝番 " +
                                            " WHERE 注文ID = @注文ID";

                        var result = connection.Execute(queryUpdate, model.Chumon, tx);

                        ChumonId = model.Chumon.注文ID;
                    }

                        // 注文明細をdelete insert する　新規、更新　共通処理

                    var queryDetailsDelete = "DELETE FROM T_注文明細 WHERE 注文ID =" + ChumonId;
                    var deleteDatails = connection.Execute(queryDetailsDelete, transaction: tx);

                    if (model.ChumonMeisai != null)
                    {

                        int i = 1;

                        for (var idx = 0; idx < model.ChumonMeisai.Count(); idx++)
                        {
                            if (model.ChumonMeisai[idx].件名 != null && model.ChumonMeisai[idx].摘要 != null)
                            {

                                var queryDetailInsert = "INSERT INTO T_注文明細 (注文ID,枝番,件名,摘要,数量,単価,原価,見込原価,金額計,原価計,見込原価計) " +
                                    " VALUES (@注文ID,@枝番,@件名,@摘要,@数量,@単価,@原価,@見込原価,@金額計,@原価計,@見込原価計)";

                                var insertDetail = connection.Execute(queryDetailInsert, new
                                {
                                    注文ID = ChumonId,
                                    枝番 = i,
                                    件名 = model.ChumonMeisai[idx].件名,
                                    摘要 = model.ChumonMeisai[idx].摘要,
                                    数量 = model.ChumonMeisai[idx].数量,
                                    単価 = model.ChumonMeisai[idx].単価,
                                    原価 = model.ChumonMeisai[idx].原価,
                                    見込原価 = model.ChumonMeisai[idx].見込原価,
                                    金額計 = model.ChumonMeisai[idx].金額計,
                                    原価計 = model.ChumonMeisai[idx].原価計,
                                    見込原価計 = model.ChumonMeisai[idx].見込原価計

                                }, tx);

                                i++;
                            }
                        }
                    }
                    tx.Commit();
                    return GetChumon(ChumonId);
                        
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    throw;
                }
            }
        }
    }

        public void DeleteChumon(int 注文ID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var tx = connection.BeginTransaction())
                {
                    try
                    {
                        //削除
                        var queryDelete = "DELETE FROM T_注文 WHERE 注文ID = " + 注文ID ;

                        var delete = connection.Execute(queryDelete, null, tx);


                        var queryDetailsDelete = "DELETE FROM T_注文明細 WHERE 注文ID = " + 注文ID;

                        var deleteDatails = connection.Execute(queryDetailsDelete, null, tx);

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

        public string GetQuoteNumber(int 営業所ID,int 担当ID,DateTime 作成日)
        {
            using (var connection = new SqlConnection(_connectionString))
            {

                var QuoteNumber = "";

                connection.Open();

                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT 期 FROM T_設定");
                var 期 = connection.Query<int>(template.RawSql, template.Parameters).FirstOrDefault();

                var builder2 = new SqlBuilder();
                var template2 = builder2.AddTemplate("SELECT イニシャル FROM T_担当 /**where**/");
                builder2.Where("担当ID = @担当ID", new { 担当ID = 担当ID });
                var イニシャル = connection.Query<string>(template2.RawSql, template2.Parameters).FirstOrDefault();

                QuoteNumber = 期 + "-" + 営業所ID + イニシャル + 作成日.ToString("MMdd");

                var builder3 = new SqlBuilder();
                var template3 = builder3.AddTemplate("SELECT Count(*) FROM T_見積 /**where**/");
                builder3.Where("見積番号 like @見積番号", new { 見積番号 = $"%{QuoteNumber}%" });
                var quote_cnt = connection.Query<int>(template3.RawSql, template3.Parameters).FirstOrDefault();
                
                if(quote_cnt > 0)
                {
                    QuoteNumber += (quote_cnt+1).ToString();
                }               

                return QuoteNumber;
            }
        }

        public IList<見積明細> GetQuoteBunrui(int 見積ID, int 履歴番号)
        {

            using (var connection = new SqlConnection(_connectionString))
            {

                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT T_見積明細.分類ID, IIf(MAX(T_分類.分類名) is null,MAX(T_見積明細.分類名),MAX(T_分類.分類名)) as 分類名,SUM(金額) as 単価 FROM T_見積明細 LEFT JOIN T_分類 on T_見積明細.分類ID = T_分類.分類ID /**where**/ GROUP BY T_見積明細.分類ID ORDER BY T_見積明細.分類ID");
                builder.Where("見積ID = @見積ID and 履歴番号= @履歴番号", new { 見積ID = 見積ID,履歴番号 = 履歴番号 });
                var quote = connection.Query<見積明細>(template.RawSql, template.Parameters).ToList();

                return quote;
            }
        }


        public IList<見積明細> GetQuoteBunruiMeisai(int 見積ID, int 履歴番号,int 分類ID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT * FROM T_見積明細 /**where**/ ORDER BY 連番");
                builder.Where("見積ID = @見積ID and 履歴番号= @履歴番号 and 分類ID=@分類ID", new { 見積ID = 見積ID, 履歴番号 = 履歴番号,分類ID=分類ID });
                var quote = connection.Query<見積明細>(template.RawSql, template.Parameters).ToList();

                return quote;
            }
        }
    }
}


