namespace rehome.Services
{
    public abstract class ServiceBase
    {
        private const string CONNECTION_SETTING_KEY = "DefaultConnection";
        protected readonly string _connectionString;

        public ServiceBase(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString(CONNECTION_SETTING_KEY);
        }
    }
}
