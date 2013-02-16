namespace QueryConsole.API.Models
{
    public class User
    {
        #region Members

        private readonly string _userid;

        private readonly string _password;

        #endregion

        #region Properties

        public string UserId
        {
            get
            {
                return this._userid;
            }
        }

        public string Password
        {
            get
            {
                return this._password;
            }
        }

        #endregion

        #region Constructors

        public User(string userid, string password)
        {
            this._userid = userid;
            this._password = password;
        }

        #endregion
    }
}
