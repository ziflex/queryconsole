// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConfiguration.cs" company="">
//   
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace QueryConsole.API.Models
{
    using System.Collections.ObjectModel;
    using System.IO;

    public interface IConfiguration
    {
        #region Public Properties

        ObservableCollection<DbProvider> DbProviders { get; }

        #endregion

        #region Public Methods and Operators

        void Load(FileInfo configuration);

        void Refresh();

        void Save();

        #endregion
    }
}