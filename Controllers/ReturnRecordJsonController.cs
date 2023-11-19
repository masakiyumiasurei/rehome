using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

namespace rehome.Controllers
{
    public class ReturnRecordJsonController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger<ReturnRecordJsonController> _logger;

        public ReturnRecordJsonController(ILogger<ReturnRecordJsonController> logger, IConfiguration configuration)
        {
            // appsettings.jsonファイルから接続文字列を取得
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        [HttpPost]
        public ActionResult ReturnRecordJson(string Table, string WhereSql)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "select * from " + Table + " where " + WhereSql;
                dynamic result = connection.Query(query);
                
                return Json(result);
            }
        }

    }
}