// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppConfiguration.cs" company="">
//   
// </copyright>
// <summary>
//   The app configuration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace QueryConsole.API.Models
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    using QueryConsole.Resources;

    public class AppConfiguration : IConfiguration
    {
        private const string AutocompleteDefault = "default.autocomplete";

        #region Fields

        private FileInfo _configFile;

        #endregion

        #region Public Properties

        /// <summary>
        /// Collection of database providers
        /// </summary>
        public ObservableCollection<DbProvider> DbProviders { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Loads configuration file
        /// </summary>
        /// <param name="configuration">configuration file info</param>
        public void Load(FileInfo configuration)
        {
            this._configFile = configuration;
            this.Refresh();
        }

        /// <summary>
        /// Reloads configuration file
        /// </summary>
        public void Refresh()
        {
            if (!this._configFile.Exists)
            {
                return;
            }

            XDocument doc = XDocument.Load(this._configFile.FullName);
            this.ValidateConfFile(doc);

            XElement settings = doc.Element("settings");
            this.DbProviders = this.GetProviders(settings);
        }

        /// <summary>
        /// Saves changes in configuration file
        /// </summary>
        public void Save()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets providers
        /// </summary>
        /// <param name="settings">node</param>
        /// <returns>Provider collection</returns>
        private ObservableCollection<DbProvider> GetProviders(XElement settings)
        {
            var result = new ObservableCollection<DbProvider>();

            XElement providers = settings.Element("providers");

            foreach (XElement provider in providers.Elements())
            {
                this.ValidateProviderElement(provider);

                var connStrList = new ObservableCollection<DbConnectionString>();

                foreach (XElement connStr in provider.Elements())
                {
                    this.ValidateConnStrElement(connStr);
                    connStrList.Add(
                        new DbConnectionString(connStr.Attribute("name").Value, connStr.Attribute("value").Value));
                }

                result.Add(
                    new DbProvider(provider.Attribute("name").Value, provider.Attribute("value").Value, connStrList, this.GetAutocompleteSource(provider)));
            }

            return result;
        }

        /// <summary>
        /// Reads xml-attribute and gets collection
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        private IEnumerable<string> GetAutocompleteSource(XElement provider)
        {
            var attr = provider.Attribute("autocomplete");
            var filePath = string.Format("{0}\\{1}", this._configFile.DirectoryName, attr != null ? attr.Value : AutocompleteDefault);
            return this.GetAutocompleteSource(new FileInfo(filePath));
        }

        /// <summary>
        /// Reads filed and populates collection
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private IEnumerable<string> GetAutocompleteSource(FileInfo file)
        {
            if (!file.Exists)
            {
                return new List<string>();
            }

            string text;

            using (var reader = file.OpenText())
            {
                text = reader.ReadToEnd();
            }

            IEnumerable<string> result = null;

            if (!string.IsNullOrWhiteSpace(text))
            {
                result = new List<string>(text.Split(new[] { ',' }).Select(i => i.Trim().Replace("\r\n", string.Empty).ToUpper()).OrderBy(i => i));
            }

            return result ?? new List<string>();
        }

        /// <summary>
        /// Validates configuration file
        /// </summary>
        /// <param name="doc">configuration xml-file</param>
        private void ValidateConfFile(XDocument doc)
        {
            if (doc.Element("settings") == null)
            {
                throw new ConfigurationException(string.Format(Resource.Conf_ElementDoesntExist, "settings"));
            }

            if (doc.Element("settings").Element("providers") == null)
            {
                throw new ConfigurationException(string.Format(Resource.Conf_ElementDoesntExist, "providers"));
            }
        }

        /// <summary>
        /// Validates connection string node
        /// </summary>
        /// <param name="connStr">xml-node</param>
        private void ValidateConnStrElement(XElement connStr)
        {
            if (connStr == null)
            {
                throw new ConfigurationException(string.Format(Resource.Conf_ElementDoesntExist, "connectionString"));
            }

            if (connStr.Attribute("name") == null)
            {
                throw new ConfigurationException(string.Format(Resource.Conf_AttributeDoesntExist, "name", "connectionString"));
            }

            if (connStr.Attribute("value") == null)
            {
                throw new ConfigurationException(string.Format(Resource.Conf_AttributeDoesntExist, "value", "connectionString"));
            }
        }

        /// <summary>
        /// Validates provider node
        /// </summary>
        /// <param name="provider">xml-node</param>
        private void ValidateProviderElement(XElement provider)
        {
            if (provider == null)
            {
                throw new ConfigurationException(string.Format(Resource.Conf_ElementDoesntExist, "provider"));
            }

            if (provider.Attribute("name") == null)
            {
                throw new ConfigurationException(string.Format(Resource.Conf_AttributeDoesntExist, "name", "provider"));
            }

            if (provider.Attribute("value") == null)
            {
                throw new ConfigurationException(string.Format(Resource.Conf_AttributeDoesntExist, "value", "provider"));
            }
        }

        #endregion
    }
}