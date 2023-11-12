using Dapper;
using Microsoft.Data.SqlClient;
using rehome.Enums;
using rehome.Models;

namespace rehome.Services
{
    public interface IDropDownListService
    {
        IList<DropDownListModel> Get仕入先DropDownLists();
        IList<DropDownListModel> Get担当DropDownLists();

        IList<string> Get注文担当DropDownLists();
        IList<DropDownListModel> Get営業所DropDownLists();
        IList<DropDownListModel> GetエリアDropDownLists();
        IList<DropDownListModel> Get社員DropDownLists();

        IList<DropDownListModel> Get先方担当DropDownLists(int? KokyakuID);

        IList<DropDownListModel> Get導入商品DropDownLists(int? KokyakuID);

        IList<DropDownListModel> Get導入機械DropDownLists(int? KokyakuID);

        IList<DropDownListModel> Get所属DropDownLists();
        IList<DropDownListModel> GetグループDropDownLists();
        public IList<DropDownListModel> Get小分類DropDownLists(int? 中分類ID);
        public IList<DropDownListModel> Get中分類DropDownLists(int? 大分類ID);
        public IList<DropDownListModel> Get大分類DropDownLists();
        public IList<DropDownListModel> Getメーカー名DropDownLists();

        IList<DropDownListModel> Get分類DropDownLists();

        IList<DropDownListModel> Get自由分類DropDownLists(int 見積ID, int 履歴番号);
    }

    public class DropDownListService : ServiceBase, IDropDownListService
    {
        private readonly ILogger<DropDownListService> _logger;

        public DropDownListService(ILogger<DropDownListService> logger, IConfiguration configuration)
            : base(configuration)
        {
            _logger = logger;
        }

        public IList<DropDownListModel> Get分類DropDownLists()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                //理化学医療区分 = 理化学医療区分 ?? "理化学','医療";
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT 分類ID as Value,分類名 as Display FROM T_分類  order by ソート");
                var result = connection.Query<DropDownListModel>(template.RawSql, template.Parameters);

                return result.ToList();
            }
        }

        public IList<DropDownListModel> Get自由分類DropDownLists(int 見積ID,int 履歴番号)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT DISTINCT 分類ID as Value,分類名 as Display FROM T_見積明細 where 見積ID = " + 見積ID + " and 履歴番号 = " + 履歴番号 + " and 分類ID between 1000000 and 2000000 order by 分類ID");
                var result = connection.Query<DropDownListModel>(template.RawSql, template.Parameters);

                return result.ToList();
            }
        }
        public IList<DropDownListModel> Get仕入先DropDownLists()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT 仕入先ID as Value,仕入先名 as Display FROM T_仕入先 /**where**/");
                var result = connection.Query<DropDownListModel>(template.RawSql, template.Parameters);

                return result.ToList();
            }
        }


        public IList<DropDownListModel> Get担当DropDownLists()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT 担当ID as Value,氏名 as Display FROM T_担当 /**where**/");
                builder.Where("(T_担当.del_date is null or T_担当.del_date > @today)", new { today = DateTime.Now });
                var result = connection.Query<DropDownListModel>(template.RawSql, template.Parameters);

                return result.ToList();
            }
        }

        public IList<string> Get注文担当DropDownLists()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT coalesce(氏名,'') + ' ' + coalesce(tel,'') AS Display " +
                    " FROM T_担当 /**where**/");
                builder.Where("(T_担当.del_date is null or T_担当.del_date > @today)", new { today = DateTime.Now });
                var result = connection.Query<string>(template.RawSql, template.Parameters);

                return result.ToList();
            }
        }

        public IList<DropDownListModel> Get営業所DropDownLists()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT 営業所ID as Value,営業所名 as Display FROM T_営業所 /**where**/");
                var result = connection.Query<DropDownListModel>(template.RawSql, template.Parameters);

                return result.ToList();
            }
        }

        public IList<DropDownListModel> GetエリアDropDownLists()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT 担当ID as Value,氏名 as Display FROM T_担当 /**where**/");
                //var template = builder.AddTemplate("SELECT エリアID,エリア名 FROM T_エリア /**where**/");
                var result = connection.Query<DropDownListModel>(template.RawSql, template.Parameters);

                return result.ToList();
            }
        }
        public  IList<DropDownListModel> Get社員DropDownLists()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT 担当ID as Value,氏名 as Display FROM T_担当 /**where**/");
                //var template = builder.AddTemplate("SELECT エリアID,エリア名 FROM T_エリア /**where**/");
                var result = connection.Query<DropDownListModel>(template.RawSql, template.Parameters);

                return result.ToList();
            }
        }

        public IList<DropDownListModel> Get先方担当DropDownLists(int? KokyakuID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT 担当ID as Value,氏名 as Display FROM T_担当 /**where**/");
                //var template = builder.AddTemplate("SELECT エリアID,エリア名 FROM T_エリア /**where**/");
                var result = connection.Query<DropDownListModel>(template.RawSql, template.Parameters);

                return result.ToList();
            }
        }

        public IList<DropDownListModel> Get導入商品DropDownLists(int? KokyakuID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT 担当ID as Value,氏名 as Display FROM T_担当 /**where**/");
                //var template = builder.AddTemplate("SELECT エリアID,エリア名 FROM T_エリア /**where**/");
                var result = connection.Query<DropDownListModel>(template.RawSql, template.Parameters);

                return result.ToList();
            }
        }

        public IList<DropDownListModel> Get導入機械DropDownLists(int? KokyakuID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT 担当ID as Value,氏名 as Display FROM T_担当 /**where**/");
                //var template = builder.AddTemplate("SELECT エリアID,エリア名 FROM T_エリア /**where**/");
                var result = connection.Query<DropDownListModel>(template.RawSql, template.Parameters);

                return result.ToList();
            }
        }

        public IList<DropDownListModel> Get所属DropDownLists()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT 担当ID as Value,氏名 as Display FROM T_担当 /**where**/");
                //var template = builder.AddTemplate("SELECT エリアID,エリア名 FROM T_エリア /**where**/");
                var result = connection.Query<DropDownListModel>(template.RawSql, template.Parameters);

                return result.ToList();
            }
        }

        public IList<DropDownListModel> GetグループDropDownLists()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT 担当ID as Value,氏名 as Display FROM T_担当 /**where**/");
                //var template = builder.AddTemplate("SELECT エリアID,エリア名 FROM T_エリア /**where**/");
                var result = connection.Query<DropDownListModel>(template.RawSql, template.Parameters);

                return result.ToList();
            }
        }
        public IList<DropDownListModel> Get大分類DropDownLists()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT 担当ID as Value,氏名 as Display FROM T_担当 /**where**/");
                //var template = builder.AddTemplate("SELECT エリアID,エリア名 FROM T_エリア /**where**/");
                var result = connection.Query<DropDownListModel>(template.RawSql, template.Parameters);

                return result.ToList();
            }
        }
        public IList<DropDownListModel> Get中分類DropDownLists(int? 大分類ID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT 担当ID as Value,氏名 as Display FROM T_担当 /**where**/");
                //var template = builder.AddTemplate("SELECT エリアID,エリア名 FROM T_エリア /**where**/");
                var result = connection.Query<DropDownListModel>(template.RawSql, template.Parameters);

                return result.ToList();
            }
        }
        public IList<DropDownListModel> Get小分類DropDownLists(int? 中分類ID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT 担当ID as Value,氏名 as Display FROM T_担当 /**where**/");
                //var template = builder.AddTemplate("SELECT エリアID,エリア名 FROM T_エリア /**where**/");
                var result = connection.Query<DropDownListModel>(template.RawSql, template.Parameters);

                return result.ToList();
            }
        }
        public IList<DropDownListModel> Getメーカー名DropDownLists()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var builder = new SqlBuilder();
                var template = builder.AddTemplate("SELECT 担当ID as Value,氏名 as Display FROM T_担当 /**where**/");
                //var template = builder.AddTemplate("SELECT エリアID,エリア名 FROM T_エリア /**where**/");
                var result = connection.Query<DropDownListModel>(template.RawSql, template.Parameters);

                return result.ToList();
            }
        }
    }
}
