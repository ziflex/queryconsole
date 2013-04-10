// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsConnectionTab.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for SettingsConnectionTab.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace QueryConsole.Controls.Settings
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    public partial class SettingsConnectionTab : TabItem
    {
        #region Constructors and Destructors

        public SettingsConnectionTab()
        {
            this.InitializeComponent();
            this.InitializeObjects();
        }

        #endregion

        #region Public Methods and Operators

        public void InitializeObjects()
        {
            this.BindControls();
        }

        public void BindControls()
        {
            var collectionViewSrc = new CollectionViewSource();
            collectionViewSrc.Source = ((App)Application.Current).AppConfiguration.DbProviders;
            this.Resources.Add("collectionViewSrc", collectionViewSrc);
            this.ProvidersList.ItemsSource = "collectionViewSrc";
        }

        #endregion
    }
}