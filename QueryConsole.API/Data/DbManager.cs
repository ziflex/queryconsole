namespace QueryConsole.API.Data
{
    using System;
    using System.Data;
    using System.Data.Common;

    using QueryConsole.API.Infrastructure;
    using QueryConsole.API.Models;

    public class DbManager : IDisposable 
    {
        #region Members

        private DbConfiguration _configuration;

        private DbProviderFactory _dpFactory;

        private bool disposed = false;

        #endregion

        #region Properties

        public DbConfiguration Configuration
        {
            get
            {
                return this._configuration;
            }
        }

        #endregion

        #region Constructor

        public DbManager(DbConfiguration configuration)
        {
            this._configuration = configuration;
            this._dpFactory     = DbProviderFactories.GetFactory(configuration.Provider);
        }

        #endregion

        #region Public Methods
        
        /// <summary>
        /// Выполняет запрос на основании переданного текста и возвращает таблицу данных
        /// </summary>
        /// <param name="queryString">string</param>
        /// <returns>DataTable</returns>
        public DataSet ExecQuery(string queryString)
        {
            DataSet result = new DataSet();
            using (var da = this._dpFactory.CreateDataAdapter())
            {
                var connection = this._dpFactory.CreateConnection();
                connection.ConnectionString = this.Configuration.ConnectionString.Value;
                
                var command = this._dpFactory.CreateCommand();
                command.Connection = connection;
                command.CommandText = QueryParser.Parse(queryString);

                da.SelectCommand = command;
                da.Fill(result);
            }

            return result;
        }

        public void Dispose()
        {
            this.CleanUp(true);

            GC.SuppressFinalize(this);
        }

        #endregion

        #region Private Methods

        private void CleanUp(bool disposing)
        {
            if (!this.disposed)
            {
                // Если disposing равно true, должно осуществляться
                // освобождение всех управляемых ресурсов.
                if (disposing)
                {
                    this._configuration = null;
                    this._dpFactory = null;
                }

                // Тут очистка неуправляемых ресурсов
            }

            this.disposed = true;
        }

        #endregion

        #region Destructor

        ~DbManager()
        {
            // Вызов спомогательного метода.
            // Значение false указывает на то, что
            // очистка была инициирована сборщиком мусора
            this.CleanUp(false);
        }

        #endregion
    }
}
