namespace QueryConsole.API.Models
{
    public class DbConfiguration
    {
        #region Members

        private readonly string _provider;

        private readonly DbConnectionString _connectionString;

        #endregion

        #region Properies

        public string Provider
        {
            get
            {
                return this._provider;
            }
        }

        public DbConnectionString ConnectionString
        {
            get
            {
                return this._connectionString;
            }
        }

        #endregion

        #region Constructor

        public DbConfiguration(string provider, DbConnectionString connStr)
        {
            this._provider = provider;
            this._connectionString = connStr;
        }

        #endregion
    }
}
