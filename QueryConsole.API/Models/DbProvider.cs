// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbProvider.cs" company="">
//   
// </copyright>
// <summary>
//   The db provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace QueryConsole.API.Models
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class DbProvider
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DbProvider"/> class. 
        /// </summary>
        /// <param name="name">
        /// Provider name
        /// </param>
        /// <param name="value">
        /// Provider value
        /// </param>
        /// <param name="connectionStrings">
        /// List of connections
        /// </param>
        public DbProvider(string name, string value, ObservableCollection<DbConnectionString> connectionStrings)
            : this(name, value, connectionStrings, new List<string>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbProvider"/> class. 
        /// </summary>
        /// <param name="name">
        /// Provider name
        /// </param>
        /// <param name="value">
        /// Provider value
        /// </param>
        /// <param name="connectionStrings">
        /// List of connections
        /// </param>
        /// <param name="autocompleteSource">
        /// Collection of words for autocomplete
        /// </param>
        public DbProvider(string name, string value, ObservableCollection<DbConnectionString> connectionStrings, IEnumerable<string> autocompleteSource)
        {
            this.Name = name;
            this.Value = value;
            this.ConnectionStrings = connectionStrings;
            this.AutocompleteSource = autocompleteSource;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Collection of connection strings
        /// </summary>
        public ObservableCollection<DbConnectionString> ConnectionStrings { get; private set; }

        /// <summary>
        /// Current name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Current value
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Collection of words for autocomplete
        /// </summary>
        public IEnumerable<string> AutocompleteSource { get; private set; }

        #endregion
    }
}