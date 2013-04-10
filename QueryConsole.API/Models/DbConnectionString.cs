// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbConnectionString.cs" company="">
//   
// </copyright>
// <summary>
//   The db connection string.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace QueryConsole.API.Models
{
    public class DbConnectionString
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DbConnectionString"/> class. 
        /// </summary>
        /// <param name="name">
        /// connection string name
        /// </param>
        /// <param name="value">
        /// connection string value
        /// </param>
        public DbConnectionString(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        #endregion

        #region Public Properties

        public string Name { get; private set; }

        public string Value { get; private set; }

        #endregion
    }
}