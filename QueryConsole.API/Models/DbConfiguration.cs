// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbConfiguration.cs" company="">
//   
// </copyright>
// <summary>
//   The db configuration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace QueryConsole.API.Models
{
    public class DbConfiguration
    {
        #region Constructors and Destructors
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DbConfiguration"/> class. 
        /// </summary>
        /// <param name="provider"> provider name
        /// </param>
        /// <param name="connStr">
        /// connection string
        /// </param>
        public DbConfiguration(string provider, DbConnectionString connStr)
        {
            this.Provider = provider;
            this.ConnectionString = connStr;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Current connection string
        /// </summary>
        public DbConnectionString ConnectionString { get; private set; }

        /// <summary>
        /// Current provider name
        /// </summary>
        public string Provider { get; private set; }

        #endregion
    }
}