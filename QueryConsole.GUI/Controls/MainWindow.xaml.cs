// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="">
//   
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace QueryConsole.Controls
{
    #region

    using System.IO;
    using System.Text;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Input;

    using QueryConsole.Controls.Settings;

    using MessageBox = System.Windows.MessageBox;
    using Resource = QueryConsole.Resources.Resource;

    #endregion

    /// <summary>
    /// Application window
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        private ICommand _cmdCloneQuery;

        private ICommand _cmdCloseAllQueries;

        private ICommand _cmdCloseAllQueriesButThis;

        private ICommand _cmdCloseQuery;

        private ICommand _cmdConfigureSettings;

        private ICommand _cmdExecuteQuery;

        private ICommand _cmdExit;

        private ICommand _cmdExportToExcel;

        private ICommand _cmdNewQuery;

        private ICommand _cmdOpenQuery;

        private ICommand _cmdSaveAllQueries;

        private ICommand _cmdSaveQuery;

        private ICommand _cmdSaveQueryAs;

        private int _itemIndexCounter;

        private OpenFileDialog _openDialog;

        private SaveFileDialog _saveDialog;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// starting point
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

            // initilize objects for work
            this.InitializeObjects();

            // create new tab as default
            this.CreateNewTab(this.GenerateNewTabName());

            // create mouse events handlers
            this.BindMouseEventHandlers();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Binds menu commands to handlers
        /// </summary>
        private void BindCommands()
        {
            // create new tab
            this.CommandBindings.Add(new CommandBinding(this._cmdNewQuery, (sender, args) => this.NewTab()));

            // clone current tab
            this.CommandBindings.Add(
                new CommandBinding(
                    this._cmdCloneQuery, 
                    (sender, args) => this.CloneTab((QueryTabItem)this.QueryTabControl.SelectedItem)));

            // open saved query in new tab
            this.CommandBindings.Add(new CommandBinding(this._cmdOpenQuery, (sender, args) => this.OpenQuery()));

            // close current tab
            this.CommandBindings.Add(
                new CommandBinding(
                    this._cmdCloseQuery, 
                    (sender, args) => this.CloseTab((QueryTabItem)this.QueryTabControl.SelectedItem)));

            // close all tabs
            this.CommandBindings.Add(
                new CommandBinding(this._cmdCloseAllQueries, (sender, args) => this.CloseAllTabs()));

            // close all tabs but current
            this.CommandBindings.Add(
                new CommandBinding(
                    this._cmdCloseAllQueriesButThis, 
                    (sender, args) => this.CloseAllTabsButThis((QueryTabItem)this.QueryTabControl.SelectedItem)));

            // save query in current tab
            this.CommandBindings.Add(
                new CommandBinding(
                    this._cmdSaveQuery, 
                    (sender, args) => this.SaveQuery((QueryTabItem)this.QueryTabControl.SelectedItem)));

            // save query and create a new name for it in current tab
            this.CommandBindings.Add(
                new CommandBinding(
                    this._cmdSaveQueryAs, 
                    (sender, args) => this.SaveQueryAs((QueryTabItem)this.QueryTabControl.SelectedItem)));

            // save all queries in all tabs
            this.CommandBindings.Add(
                new CommandBinding(this._cmdSaveAllQueries, (sender, args) => this.SaveAllQueries()));

            // execute query in current tab
            this.CommandBindings.Add(
                new CommandBinding(
                    this._cmdExecuteQuery, 
                    (sender, args) => this.Execute((QueryTabItem)this.QueryTabControl.SelectedItem)));

            // export query result to excel file
            this.CommandBindings.Add(
                new CommandBinding(
                    this._cmdExportToExcel, 
                    (sender, args) => this.ExportToExcel((QueryTabItem)this.QueryTabControl.SelectedItem)));

            // open configuration window
            this.CommandBindings.Add(
                new CommandBinding(this._cmdConfigureSettings, (sender, args) => this.OpenSettingsWindow()));

            // close app
            this.CommandBindings.Add(new CommandBinding(this._cmdExit, (sender, args) => this.Close()));
        }

        /// <summary>
        /// Binds commands to hot keys
        /// </summary>
        private void BindInputs()
        {
            // new query via ctrl + n
            this.InputBindings.Add(new KeyBinding(this._cmdNewQuery, Key.N, ModifierKeys.Control));

            // open query via ctrl + o
            this.InputBindings.Add(new KeyBinding(this._cmdOpenQuery, Key.O, ModifierKeys.Control));

            // close query via ctrl + f4
            this.InputBindings.Add(new KeyBinding(this._cmdCloseQuery, Key.F4, ModifierKeys.Control));

            // save query via ctrl + s
            this.InputBindings.Add(new KeyBinding(this._cmdSaveQuery, Key.S, ModifierKeys.Control));

            // execute query via f5
            this.InputBindings.Add(new KeyBinding(this._cmdExecuteQuery, Key.F5, ModifierKeys.None));
        }

        /// <summary>
        /// Binds menu items to commands
        /// </summary>
        private void BindMenuItems()
        {
            // create new tab
            this.miNew.Command = this._cmdNewQuery;

            // clone current tab
            this.miClone.Command = this._cmdCloneQuery;

            // open saved query in new tab
            this.miOpen.Command = this._cmdOpenQuery;

            // close current tab
            this.miClose.Command = this._cmdCloseQuery;

            // close all tabs
            this.miCloseAll.Command = this._cmdCloseAllQueries;

            // close all tabs but current
            this.miCloseAllButThis.Command = this._cmdCloseAllQueriesButThis;

            // save query in current tab
            this.miSave.Command = this._cmdSaveQuery;

            // save query and creat a new name for it in current tab
            this.miSaveAs.Command = this._cmdSaveQueryAs;

            // save all queries in all tabs
            this.miSaveAll.Command = this._cmdSaveAllQueries;

            // execute query in current tab
            this.miExec.Command = this._cmdExecuteQuery;

            // export query result to excel file
            this.miExportToExcel.Command = this._cmdExportToExcel;

            // open configuration window
            this.miConfigure.Command = this._cmdConfigureSettings;
            this.smSettings.Visibility = Visibility.Hidden; // unimplemented feature

            // close app
            this.miExit.Command = this._cmdExit;
        }

        /// <summary>
        /// create new tab using a parent's content
        /// </summary>
        /// <param name="tab"></param>
        private void CloneTab(QueryTabItem tab)
        {
            this.CreateNewTab(this.GenerateNewTabName(), tab.QueryText.Text);
        }

        private void CloseAllTabs()
        {
            this.QueryTabControl.Items.Clear();
        }

        private void CloseAllTabsButThis(QueryTabItem tab)
        {
            // remove all tabs except current
            while (this.QueryTabControl.Items.Count > 1)
            {
                for (int i = 0; i < this.QueryTabControl.Items.Count; i++)
                {
                    // check index of current tab and delete if current index differs
                    int index = this.QueryTabControl.Items.IndexOf(tab);
                    if (i != index)
                    {
                        this.QueryTabControl.Items.RemoveAt(i);
                    }
                }
            }
        }

        private void CloseTab(QueryTabItem tab)
        {
            this.QueryTabControl.Items.Remove(tab);
        }

        private void CreateNewTab(string name, string text = null, string filePath = null)
        {
            // increment global index counter for naming new tabs
            this._itemIndexCounter++;

            var newTab = new QueryTabItem(this._itemIndexCounter, name, text, filePath);
            newTab.CloseEvent += this.CloseTab;
            newTab.CloseAllButThisEvent += this.CloseAllTabsButThis;
            newTab.SaveEvent += this.SaveQuery;
            newTab.SaveAsEvent += this.SaveQueryAs;
            newTab.NewEvent += tab => this.NewTab();
            newTab.CloneEvent += this.CloneTab;

            // add new tab to collection
            this.QueryTabControl.Items.Add(newTab);
            this.QueryTabControl.SelectedItem = newTab;
        }

        private void Execute(QueryTabItem tab)
        {
            if (tab == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(tab.QueryText.Text))
            {
                return;
            }

            tab.ExecQuery();
        }

        private void ExportToExcel(QueryTabItem tab)
        {
            if (tab == null)
            {
                return;
            }

            this._saveDialog.Filter = "Excel 2007 documents (.xlsx)|*.xlsx";
            this._saveDialog.DefaultExt = "xlsx";
            DialogResult result = this._saveDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                FileInfo fileInfo = tab.ExportToExcel(this._saveDialog.FileName);
                string msg = fileInfo.Exists ? Resource.Export_Success : Resource.Export_Fail;
                MessageBox.Show(msg);
            }
        }

        private string GenerateNewTabName()
        {
            this._itemIndexCounter++;

            string name = string.Format("Query {0}", this._itemIndexCounter);
            return name;
        }

        /// <summary>
        /// Initialize commands
        /// </summary>
        private void InitCommands()
        {
            this._cmdNewQuery = new RoutedCommand("NewQueryCommand", this.GetType());
            this._cmdCloneQuery = new RoutedCommand("CloneQueryCommand", this.GetType());
            this._cmdOpenQuery = new RoutedCommand("OpenQueryCommand", this.GetType());
            this._cmdCloseQuery = new RoutedCommand("CloseQueryCommand", this.GetType());
            this._cmdCloseAllQueries = new RoutedCommand("CloseAllQueries", this.GetType());
            this._cmdCloseAllQueriesButThis = new RoutedCommand("CloseAllQueriesButThisCommand", this.GetType());
            this._cmdSaveQuery = new RoutedCommand("SaveQueryCommand", this.GetType());
            this._cmdSaveQueryAs = new RoutedCommand("SaveQueryAs", this.GetType());
            this._cmdSaveAllQueries = new RoutedCommand("SaveAllQueriesCommand", this.GetType());
            this._cmdExecuteQuery = new RoutedCommand("ExecuteCommand", this.GetType());
            this._cmdExportToExcel = new RoutedCommand("ExportToExcelCommand", this.GetType());
            this._cmdConfigureSettings = new RoutedCommand("ConfigureSettings", this.GetType());
            this._cmdExit = new RoutedCommand("ExitCommand", this.GetType());
        }

        /// <summary>
        /// Initialize save / open file dialogs
        /// </summary>
        private void InitFileDialogs()
        {
            this._saveDialog = new SaveFileDialog
                {
                   CheckPathExists = true, CreatePrompt = true, OverwritePrompt = true 
                };
            this._openDialog = new OpenFileDialog();
        }

        private void InitializeObjects()
        {
            this.InitFileDialogs();
            this.InitCommands();
            this.BindCommands();
            this.BindMenuItems();
            this.BindInputs();
        }

        /// <summary>
        /// Create new tab
        /// </summary>
        private void NewTab()
        {
            this.CreateNewTab(this.GenerateNewTabName());
        }

        private void OpenFile(string filePath, string fileName)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException();
            }

            using (var sr = new StreamReader(filePath))
            {
                this.CreateNewTab(fileName, sr.ReadToEnd(), filePath);
                sr.Close();
            }
        }

        private void OpenQuery()
        {
            if (this._openDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.OpenFile(this._openDialog.FileName, this._openDialog.SafeFileName);
            }
        }

        private void OpenSaveDialog(QueryTabItem tab)
        {
            this._saveDialog.FileName = tab.Name;

            // Open dialog
            this._saveDialog.Filter = "Text documents (.txt)|*.txt";
            this._saveDialog.DefaultExt = ".txt";
            DialogResult result = this._saveDialog.ShowDialog();

            // Pressed OK
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // Save document
                FileInfo fileInfo = this.SaveFile(this._saveDialog.FileName, tab.QueryText.Text);
                tab.FilePath = fileInfo.FullName;
                tab.SetHeader(fileInfo.Name);
            }
        }

        private void OpenSettingsWindow()
        {
            var settings = new SettingsMainWindow();
            settings.ShowDialog();
        }

        private void SaveAllQueries()
        {
            foreach (QueryTabItem item in this.QueryTabControl.Items.SourceCollection)
            {
                this.OpenSaveDialog(item);
            }
        }

        private FileInfo SaveFile(string filePath, string fileBody)
        {
            // create new file
            using (FileStream fs = File.Create(filePath))
            {
                byte[] info = new UTF8Encoding(true).GetBytes(fileBody);
                fs.Write(info, 0, info.Length);
            }

            return new FileInfo(filePath);
        }

        private void SaveQuery(QueryTabItem tab)
        {
            if (tab == null)
            {
                return;
            }

            // loaded or new
            if (string.IsNullOrWhiteSpace(tab.FilePath))
            {
                // new
                this.OpenSaveDialog(tab);
            }
            else
            {
                // loaded. just rewrite
                this.SaveFile(tab.FilePath, tab.QueryText.Text);
            }
        }

        private void SaveQueryAs(QueryTabItem tab)
        {
            this.OpenSaveDialog(tab);
        }

        /// <summary>
        /// Bind mouse events to handlers
        /// </summary>
        private void BindMouseEventHandlers()
        {
            this.miNew.IsEnabled = true;

            // for each menu set accessibility on mouse clicking

            // menu "File"
            this.smFile.PreviewMouseDown += (sender, args) =>
            {
                bool isEnable = this.QueryTabControl.Items.Count != 0;
                this.miClone.IsEnabled = isEnable;
                this.miClose.IsEnabled = isEnable;
                this.miCloseAll.IsEnabled = isEnable;
                this.miCloseAllButThis.IsEnabled = isEnable;
                this.miSave.IsEnabled = isEnable;
                this.miSaveAs.IsEnabled = isEnable;
                this.miSaveAll.IsEnabled = isEnable;

                // additional check
                if (isEnable)
                {
                    var queryTabItem = (QueryTabItem)this.QueryTabControl.SelectedItem;
                    this.miCloseAllButThis.IsEnabled = this.QueryTabControl.Items.Count > 1;
                    this.miSaveAll.IsEnabled = this.QueryTabControl.Items.Count > 1;
                    this.miSave.IsEnabled = !string.IsNullOrWhiteSpace(queryTabItem.FilePath);
                }
            };

            // menu "Query"
            this.smQuery.PreviewMouseDown += (sender, args) =>
            {
                bool isEnable = this.QueryTabControl.Items.Count != 0;
                this.miExec.IsEnabled = isEnable;
                this.miExportToExcel.IsEnabled = isEnable;

                // additional check
                if (isEnable)
                {
                    var queryTabItem = (QueryTabItem)this.QueryTabControl.SelectedItem;

                    // disable for empty query text
                    this.miExec.IsEnabled = !string.IsNullOrWhiteSpace(queryTabItem.QueryText.Text);

                    // disable for null results
                    this.miExportToExcel.IsEnabled = queryTabItem.QueryResult.ItemsSource != null;
                }
            };

            this.EventLabel.MouseDoubleClick += (sender, args) => this.NewTab();
        }

        #endregion
    }
}