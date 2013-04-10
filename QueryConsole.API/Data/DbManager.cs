// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbManager.cs" company="">
//   
// </copyright>
// <summary>
//   The db manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace QueryConsole.API.Data
{
    using System;
    using System.Data;
    using System.Data.Common;

    using QueryConsole.API.Infrastructure;
    using QueryConsole.API.Models;

    public class DbManager : IDisposable
    {
        #region Fields

        private DbProviderFactory _dpFactory;

        private bool _disposed;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Creates new instance of DbManager
        /// </summary>
        /// <param name="configuration">Database configuration</param>
        public DbManager(DbConfiguration configuration)
        {
            this.Configuration = configuration;
            this._dpFactory = DbProviderFactories.GetFactory(configuration.Provider);
        }

        ~DbManager()
        {
            this.CleanUp(false);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Current database configuration
        /// </summary>
        public DbConfiguration Configuration { get; private set; }

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            this.CleanUp(true);

            GC.SuppressFinalize(this);
        }

        public DataSet ExecQuery(string queryString)
        {
            var result = new DataSet();
            using (DbDataAdapter da = this._dpFactory.CreateDataAdapter())
            {
                DbConnection connection = this._dpFactory.CreateConnection();
                connection.ConnectionString = this.Configuration.ConnectionString.Value;

                DbCommand command = this._dpFactory.CreateCommand();
                command.Connection = connection;
                command.CommandText = QueryParser.Parse(queryString);

                da.SelectCommand = command;
                da.Fill(result);
            }

            return result;
        }

        #endregion

        #region Methods

        private void CleanUp(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    this.Configuration = null;
                    this._dpFactory = null;
                }
            }

            this._disposed = true;
        }

        #endregion
    }
}