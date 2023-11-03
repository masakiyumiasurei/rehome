namespace rehome.DB
{
    public class DbConnection
    {
        private const string CONNECTION_SETTING_NAME = "dapperConnection";
        private static DbConnection _connection;
        private readonly string _connectionString;

        private DbConnection()
        {
            //_connectionString = Configuration.[CONNECTION_SETTING_NAME].ConnectionString;
        }

        public static string ConnectionString()
        {
            if (_connection == null)
            {
                _connection = new DbConnection();
            }

            return _connection._connectionString;
        }
    }
}
