namespace QueryConsole.API.Models
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Xml.Linq;

    using QueryConsole.Resources;

    public class AppConfiguration
    {
        #region Members

        private readonly string ConfigPath;

        private IEnumerable<DbProvider> _providers;

        #endregion

        #region Properties

        public IEnumerable<DbProvider> DbProviders
        {
            get
            {
                return this._providers;
            }
        }

        #endregion

        #region Constructors

        public AppConfiguration(string configPath)
        {
            this.ConfigPath = configPath;
            this.Init();
        }

        private void Init()
        {
            XDocument doc = XDocument.Load(this.ConfigPath);
            this.ValidateConfFile(doc);

            XElement settings = doc.Element("settings");
            this._providers = this.GetProviders(settings);
        }

        #endregion

        #region Private Methods

        private void ValidateConfFile(XDocument doc)
        {
            if (doc.Element("settings") == null)
            {
                throw new ConfigurationException(Resource.Conf_ElementDoesntExistFormat("settings"));
            }

            if (doc.Element("settings").Element("providers") == null)
            {
                throw new ConfigurationException(Resource.Conf_ElementDoesntExistFormat("providers"));
            }
        }

        private void ValidateProviderElement(XElement provider)
        {
            if (provider == null)
            {
                throw new ConfigurationException(Resource.Conf_ElementDoesntExistFormat("provider"));
            }

            if (provider.Attribute("name") == null)
            {
                throw new ConfigurationException(Resource.Conf_AttributeDoesntExistFormat("name", "provider"));
            }

            if (provider.Attribute("value") == null)
            {
                throw new ConfigurationException(Resource.Conf_AttributeDoesntExistFormat("value", "provider"));
            }
        }

        private void ValidateConnStrElement(XElement connStr)
        {
            if (connStr == null)
            {
                throw new ConfigurationException(Resource.Conf_ElementDoesntExistFormat("connectionString"));
            }

            if (connStr.Attribute("name") == null)
            {
                throw new ConfigurationException(Resource.Conf_AttributeDoesntExistFormat("name", "connectionString"));
            }

            if (connStr.Attribute("value") == null)
            {
                throw new ConfigurationException(Resource.Conf_AttributeDoesntExistFormat("value", "connectionString"));
            }
        }

        private IEnumerable<DbProvider> GetProviders(XElement settings)
        {
            List<DbProvider> result = new List<DbProvider>();

            XElement providers = settings.Element("providers");

            foreach (XElement provider in providers.Elements())
            {
                this.ValidateProviderElement(provider);
                
                List<DbConnectionString> connStrList = new List<DbConnectionString>();

                foreach (XElement connStr in provider.Elements())
                {
                    this.ValidateConnStrElement(connStr);
                    connStrList.Add(new DbConnectionString(connStr.Attribute("name").Value, connStr.Attribute("value").Value));
                }

                result.Add(new DbProvider(provider.Attribute("name").Value, provider.Attribute("value").Value, connStrList));
            }

            return result;
        }

        #endregion
    }
}
