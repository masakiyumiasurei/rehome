using Dapper;
using Microsoft.Data.SqlClient;
using rehome.Models.DB;

namespace rehome.Public
{
    public class FunctionClass
    {
        private readonly string _connectionString;
        public FunctionClass(string connectionString)
        {
            _connectionString = connectionString;
        }
        public  T GetValue<T>(string strsql)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var result = connection.Query<T>(strsql).FirstOrDefault();
                return result;
            }
        }
    }
}
