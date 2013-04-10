// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsMainWindow.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for SettingsMainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace QueryConsole.Controls.Settings
{
    using System.Windows;
    using System.Windows.Input;

    public partial class SettingsMainWindow : Window
    {
        #region Fields

        private ICommand _cmdClose;

        private ICommand _cmdSave;

        #endregion

        #region Constructors and Destructors

        public SettingsMainWindow()
        {
            this.InitializeComponent();
            this.InitializeObjects();
        }

        #endregion

        #region Methods

        private void BindCommands()
        {
            this.CommandBindings.Add(
                new CommandBinding(
                    this._cmdSave, 
                    (sender, args) =>
                        {
                            this.Save();
                            this.Close();
                        }));
            this.CommandBindings.Add(new CommandBinding(this._cmdClose, (sender, args) => this.Close()));
        }

        private void BindControls()
        {
            this.btnSave.Command = this._cmdSave;
            this.btnCancel.Command = this._cmdClose;
        }

        private void InitCommands()
        {
            this._cmdSave = new RoutedCommand("SaveCommand", this.GetType());
            this._cmdClose = new RoutedCommand("CloseCommand", this.GetType());
        }

        private void InitControls()
        {
            this.Menu.Items.Add(new SettingsConnectionTab());
        }

        private void InitializeObjects()
        {
            this.InitCommands();
            this.BindCommands();
            this.InitControls();
            this.BindControls();
        }

        private void Save()
        {
        }

        #endregion
    }
}