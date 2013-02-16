namespace QueryConsole.API.Models
{
    using System.Collections.Generic;

    public class DbProvider
    {
        #region Members

        private readonly string _name;

        private readonly string _value;

        private IEnumerable<DbConnectionString> _connectionStrings;

        #endregion

        #region Properties

        public string Name
        {
            get
            {
                return this._name;
            }
        }

        public string Value
        {
            get
            {
                return this._value;
            }
        }

        public IEnumerable<DbConnectionString> ConnectionStrings
        {
            get
            {
                return this._connectionStrings;
            }
        }

        #endregion

        #region Constructor

        public DbProvider(string name, string value, IEnumerable<DbConnectionString> connectionStrings)
        {
            this._name = name;
            this._value = value;
            this._connectionStrings = connectionStrings;
        }

        #endregion
    }
}
