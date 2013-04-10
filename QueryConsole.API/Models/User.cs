// --------------------------------------------------------------------------------------------------------------------
// <copyright file="User.cs" company="">
//   
// </copyright>
// <summary>
//   The user.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace QueryConsole.API.Models
{
    public class User
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class. 
        /// </summary>
        /// <param name="id">
        /// Id
        /// </param>
        /// <param name="password">
        /// Password
        /// </param>
        public User(string id, string password)
        {
            this.Id = id;
            this.Password = password;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Current password
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// Current Id
        /// </summary>
        public string Id { get; private set; }

        #endregion
    }
}