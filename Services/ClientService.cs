using Dapper;
using Microsoft.Data.SqlClient;
using rehome.Enums;
using rehome.Models;
using rehome.Models.DB;
using ServiceStack;
using X.PagedList;
using static Castle.MicroKernel.ModelBuilder.Descriptors.InterceptorDescriptor;

namespace rehome.Services
{
    public interface IClientService
    {
        public IList<顧客> SearchClients(ClientSearchConditions conditions);

        顧客 GetClient(int? 顧客ID);

        顧客 RegistClient(ClientDetailModel model);

        //void RegistClientSodan(ClientDetailModel model);

        //void RegistClientTantou(ClientDetailModel model);
        int GetclientCount();
        void DeleteClient(int ClientID);

    }

    public class ClientService : ServiceBase, IClientService
    {
        private readonly ILogger<ClientService> _logger;

        public ClientService(ILogger<ClientService> logger, IConfiguration configuration)
            : base(configuration)
        {
            _logger = logger;
        }


        public IList<顧客> SearchClients(ClientSearchConditions conditions)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string WhereStr = "";
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT RT_顧客.*  FROM RT_顧客 " +
                    " /**where**/ /**orderby**/");

                if (conditions != null)
                {

                    if (!string.IsNullOrEmpty(conditions.顧客名))
                    {
                        builder.Where("顧客名 like @顧客名", new { 顧客名 = $"%{conditions.顧客名}%" });
                    }

                    if (!string.IsNullOrEmpty(conditions.カナ))
                    {
                        builder.Where("カナ like @カナ", new { カナ = $"%{conditions.カナ}%" });
                    }

                    if (!string.IsNullOrEmpty(conditions.郵便番号))
                    {
                        builder.Where("郵便番号 like @郵便番号", new { 郵便番号 = $"%{conditions.郵便番号}%" });
                    }

                    if (!string.IsNullOrEmpty(conditions.住所))
                    {
                        builder.Where("isnull(住所1,'') + isnull(住所2,'') like @住所", new { 住所 = $"%{conditions.住所}%" });
                    }

                    if (!string.IsNullOrEmpty(conditions.TEL))
                    {
                        builder.Where("(電話番号1 like @TEL or 電話番号2 like @TEL)", new { TEL = $"%{conditions.TEL}%" });                        
                    }

                    builder.OrderBy("顧客ID desc");

                }

                var result = connection.Query<顧客>(template.RawSql, template.Parameters);

                return result.ToList();
            }
        }

        /// <summary>
        /// 請求済みの顧客の件数
        /// </summary>
        /// <returns></returns>
        public int GetclientCount()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT COUNT(DISTINCT RT_顧客.顧客ID) AS Clientcount " +
                                                "FROM RT_顧客 " +
                                                "JOIN T_見積 ON RT_顧客.顧客ID = T_見積.顧客ID " +
                                                "WHERE T_見積.見積ステータス = '請求'; ");
                                
                var result = connection.Query<int>(template.RawSql).FirstOrDefault();
                return result;
            }                        
        }

        public 顧客 GetClient(int? 顧客ID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT RT_顧客.* FROM RT_顧客 " +
                    " /**where**/ ");
           
                builder.Where("顧客ID = @顧客ID ", new { 顧客ID = 顧客ID });

                var result = connection.Query<顧客>(template.RawSql, template.Parameters).FirstOrDefault();
                return result;
            }
        }


public 顧客 RegistClient(ClientDetailModel model)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var tx = connection.BeginTransaction())
                {
                    try
                    {
                        var sql = "";
                        int 処理後顧客ID = -1;

                        if (model.Client.顧客ID == 0)
                        {
                            //新規
                            var queryInsert = "INSERT INTO RT_顧客 (顧客名,郵便番号,住所1,住所2,建物種別,電話番号1,電話番号2,FAX,メールアドレス,紹介者,備考," +
                                            "宛名印刷FLG,依頼者種別,ブラックリストFLG,知った理由,依頼理由,工事検討,玄関flg,リビングflg,キッチンflg,トイレflg,洗面所flg," +
                                            "風呂flg,居室flg,建具flg,窓flg,内装flg,外壁flg,屋根flg,その他,カナ, " +
                                            "法人名,詳細備考,依頼理由金額FLG,依頼理由信頼感FLG,依頼理由紹介業者FLG," +
                                            "依頼理由HPFLG,賃貸分譲区分,生年月日,連絡方法) " +
                                            " VALUES (@顧客名,@郵便番号,@住所1,@住所2,@建物種別,@電話番号1,@電話番号2,@FAX,@メールアドレス,@紹介者,@備考," +
                                            "@宛名印刷FLG,@依頼者種別,@ブラックリストFLG,@知った理由,@依頼理由,@工事検討," +
                                            "@玄関flg,@リビングflg,@キッチンflg,@トイレflg,@洗面所flg," +
                                            "@風呂flg,@居室flg,@建具flg,@窓flg,@内装flg,@外壁flg,@屋根flg,@その他,@カナ,"+
                                            "@法人名,@詳細備考,@依頼理由金額FLG,@依頼理由信頼感FLG,@依頼理由紹介業者FLG," +
                                            "@依頼理由HPFLG,@賃貸分譲区分,@生年月日,@連絡方法)";

                            var insert = connection.Execute(queryInsert, model.Client, tx);

                            tx.Commit();

                            処理後顧客ID = (int)connection.Query("SELECT @@IDENTITY as ID").First().ID;
                           
                        }
                        else
                        {
                            //更新
                            var queryUpdate = "UPDATE RT_顧客 SET 顧客名=@顧客名,郵便番号=@郵便番号,住所1=@住所1,住所2=@住所2,建物種別=@建物種別,"+
                                            "電話番号1=@電話番号1,電話番号2=@電話番号2,FAX = @FAX,メールアドレス=@メールアドレス," +
                                              "紹介者=@紹介者,宛名印刷FLG=@宛名印刷FLG,依頼者種別=@依頼者種別,ブラックリストFLG=@ブラックリストFLG," +
                                              "知った理由=@知った理由,依頼理由=@依頼理由,工事検討=@工事検討,玄関flg=@玄関flg," +
                                              "リビングflg=@リビングflg,キッチンflg=@キッチンflg,トイレflg=@トイレflg,洗面所flg=@洗面所flg,風呂flg=@風呂flg," +
                                              "備考=@備考,居室flg=@居室flg,カナ=@カナ,建具flg=@建具flg,窓flg=@窓flg,内装flg=@内装flg,外壁flg=@外壁flg," +
                                              "屋根flg=@屋根flg,その他=@その他, " +
                                              "法人名=@法人名,詳細備考=@詳細備考,依頼理由金額FLG=@依頼理由金額FLG,依頼理由信頼感FLG=@依頼理由信頼感FLG," +
                                              "依頼理由紹介業者FLG=@依頼理由紹介業者FLG,依頼理由HPFLG=@依頼理由HPFLG," +
                                              "賃貸分譲区分=@賃貸分譲区分,生年月日=@生年月日 ,連絡方法=@連絡方法 " +
                                              " WHERE 顧客ID = @顧客ID";

                            var result = connection.Execute(queryUpdate, model.Client, tx);


                            tx.Commit();

                            処理後顧客ID = model.Client.顧客ID;

                        }

                        return GetClient(処理後顧客ID);
                        
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }


        //public void RegistClientTantou(ClientDetailModel model)
        //{
        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        connection.Open();
        //        using (var tx = connection.BeginTransaction())
        //        {
        //            try
        //            {

        //                //顧客担当登録
        //                var queryDelete = "DELETE FROM RT_顧客担当 WHERE 顧客ID=" + model.Client.顧客ID;

        //                var delete = connection.Execute(queryDelete, null, tx);


        //                if (model.顧客担当リスト != null)
        //                {
        //                    foreach (var item in model.顧客担当リスト)
        //                    {
        //                        var queryInsert2 = "INSERT INTO RT_顧客担当 (顧客ID,担当ID,担当種別) VALUES (" + model.Client.顧客ID + ", @担当ID,@担当種別)";

        //                        var Insert2 = connection.Execute(queryInsert2, item, tx);
        //                    }
        //                }
        //                tx.Commit();

        //            }
        //            catch (Exception ex)
        //            {
        //                tx.Rollback();
        //                throw;
        //            }
        //        }
        //    }
        //}



        public void DeleteClient(int ClientID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var tx = connection.BeginTransaction())
                {
                    try
                    {

                        //削除

                        var queryDetailsDelete = "DELETE FROM RT_顧客 WHERE 顧客ID = " + ClientID;

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


    }
}


