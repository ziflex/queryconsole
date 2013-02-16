namespace QueryConsole.Controls
{
    using System;
    using System.IO;
    using System.Text;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Input;

    using MessageBox = System.Windows.MessageBox;

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties

        private ICommand _cmdNewQuery;

        private ICommand _cmdCloneQuery;

        private ICommand _cmdOpenQuery;

        private ICommand _cmdCloseQuery;

        private ICommand _cmdCloseAllQueries;

        private ICommand _cmdCloseAllQueriesButThis;

        private ICommand _cmdSaveQuery;

        private ICommand _cmdSaveQueryAs;

        private ICommand _cmdSaveAllQueries;

        private ICommand _cmdExecuteQuery;

        private ICommand _cmdExportToExcel;

        private ICommand _cmdExit;

        private SaveFileDialog SaveDialog { get; set; }

        private OpenFileDialog OpenDialog { get; set; }

        private int ItemIndexCounter { get; set; }

        #endregion

        #region Constructor

        public MainWindow()
        {
            this.InitializeComponent();

            // Создаем объекты для работы
            this.InitializeObjects();

            // Создаем новую закладку
            this.CreateNewTab(this.GenerateNewTabName());

            // Создаем подписки на события
            this.SubscribeEventHandlers();
        }

        private void InitializeObjects()
        {
            this.InitFileDialogs();
            this.InitCommands();
            this.BindCommands();
            this.InitMenu();
            this.BindMenuItems();
            this.BindInputs();
        }

        private void InitFileDialogs()
        {
            this.SaveDialog = new SaveFileDialog
                {
                    CheckPathExists = true,
                    CreatePrompt = true,
                    OverwritePrompt = true
                };
            this.OpenDialog = new OpenFileDialog();
        }

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
            this._cmdExit = new RoutedCommand("ExitCommand", this.GetType());
        }

        private void BindCommands()
        {
            this.CommandBindings.Add(new CommandBinding(this._cmdNewQuery, (sender, args) => this.NewTab()));
            this.CommandBindings.Add(new CommandBinding(this._cmdCloneQuery, (sender, args) => this.CloneTab((QueryTabItem)this.QueryTabControl.SelectedItem)));
            this.CommandBindings.Add(new CommandBinding(this._cmdOpenQuery, (sender, args) => this.OpenQuery()));
            this.CommandBindings.Add(new CommandBinding(this._cmdCloseQuery, (sender, args) => this.CloseTab((QueryTabItem)this.QueryTabControl.SelectedItem)));
            this.CommandBindings.Add(new CommandBinding(this._cmdCloseAllQueries, (sender, args) => this.CloseAllTabs()));
            this.CommandBindings.Add(new CommandBinding(this._cmdCloseAllQueriesButThis, (sender, args) => this.CloseAllTabsButThis((QueryTabItem)this.QueryTabControl.SelectedItem)));
            this.CommandBindings.Add(new CommandBinding(this._cmdSaveQuery, (sender, args) => this.SaveQuery((QueryTabItem)this.QueryTabControl.SelectedItem)));
            this.CommandBindings.Add(new CommandBinding(this._cmdSaveQueryAs, (sender, args) => this.SaveQueryAs((QueryTabItem)this.QueryTabControl.SelectedItem)));
            this.CommandBindings.Add(new CommandBinding(this._cmdSaveAllQueries, (sender, args) => this.SaveAllQueries()));
            this.CommandBindings.Add(new CommandBinding(this._cmdExecuteQuery, (sender, args) => this.Execute((QueryTabItem)this.QueryTabControl.SelectedItem)));
            this.CommandBindings.Add(new CommandBinding(this._cmdExportToExcel, (sender, args) => this.ExportToExcel((QueryTabItem)this.QueryTabControl.SelectedItem)));
            this.CommandBindings.Add(new CommandBinding(this._cmdExit, (sender, args) => this.Close()));
        }

        private void InitMenu()
        {
            this.miNew.IsEnabled = true;
            this.smFile.PreviewMouseDown += (sender, args) =>
            {
                bool isEnable = this.QueryTabControl.Items.Count != 0;
                this.miClose.IsEnabled = isEnable;
                this.miCloseAll.IsEnabled = isEnable;
                this.miSave.IsEnabled = isEnable;
                this.miSaveAs.IsEnabled = isEnable;
                this.miSaveAll.IsEnabled = isEnable;

                // Доп проверка для нового файла
                if (isEnable)
                {
                    QueryTabItem queryTabItem = (QueryTabItem)this.QueryTabControl.SelectedItem;
                    this.miSave.IsEnabled = !string.IsNullOrWhiteSpace(queryTabItem.FilePath);
                }
            };

            this.smQuery.PreviewMouseDown += (sender, args) =>
            {
                bool isEnable = this.QueryTabControl.Items.Count != 0;
                this.miExec.IsEnabled = isEnable;

                if (isEnable)
                {
                    QueryTabItem queryTabItem = (QueryTabItem)this.QueryTabControl.SelectedItem;
                    this.miExec.IsEnabled = !string.IsNullOrWhiteSpace(queryTabItem.QueryText.Text);
                    this.miExportToExcel.IsEnabled = queryTabItem.QueryResult.ItemsSource != null;
                }
            };
        }

        private void BindMenuItems()
        {
            // Menu
            this.miNew.Command = this._cmdNewQuery;
            this.miClone.Command = this._cmdCloneQuery;
            this.miOpen.Command = this._cmdOpenQuery;
            this.miClose.Command = this._cmdCloseQuery;
            this.miCloseAll.Command = this._cmdCloseAllQueries;
            this.miCloseAllButThis.Command = this._cmdCloseAllQueriesButThis;
            this.miSave.Command = this._cmdSaveQuery;
            this.miSaveAs.Command = this._cmdSaveQueryAs;
            this.miSaveAll.Command = this._cmdSaveAllQueries;
            this.miExec.Command = this._cmdExecuteQuery;
            this.miExportToExcel.Command = this._cmdExportToExcel;
            this.miExit.Command = this._cmdExit;
        }

        private void BindInputs()
        {
            this.InputBindings.Add(new KeyBinding(this._cmdNewQuery, Key.N, ModifierKeys.Control));
            this.InputBindings.Add(new KeyBinding(this._cmdOpenQuery, Key.O, ModifierKeys.Control));
            this.InputBindings.Add(new KeyBinding(this._cmdCloseQuery, Key.F4, ModifierKeys.Control));
            this.InputBindings.Add(new KeyBinding(this._cmdSaveQuery, Key.S, ModifierKeys.Control));
            this.InputBindings.Add(new KeyBinding(this._cmdExecuteQuery, Key.F5, ModifierKeys.None));
        }

        #endregion

        #region Common Methods

        private void CreateNewTab(string name, string text = null, string filePath = null)
        {
            this.ItemIndexCounter++;
            QueryTabItem newTab            = new QueryTabItem(this.ItemIndexCounter, name, text, filePath);
            newTab.CloseEvent              += this.CloseTab;
            newTab.CloseAllButThisEvent    += this.CloseAllTabsButThis;
            newTab.SaveEvent               += this.SaveQuery;
            newTab.SaveAsEvent             += this.SaveQueryAs;
            newTab.NewEvent                += tab => this.NewTab();
            newTab.CloneEvent              += this.CloneTab;

            this.QueryTabControl.Items.Add(newTab);
            this.QueryTabControl.SelectedItem = newTab;
        }

        private FileInfo SaveFile(string filePath, string fileBody)
        {
            // Создаем новый файл
            using (FileStream fs = File.Create(filePath))
            {
                byte[] info = new UTF8Encoding(true).GetBytes(fileBody);
                fs.Write(info, 0, info.Length);
            }

            return new FileInfo(filePath);
        }

        private void OpenFile(string filePath, string fileName)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException();
            }

            using (StreamReader sr = new StreamReader(filePath))
            {
                this.CreateNewTab(fileName, sr.ReadToEnd(), filePath);
                sr.Close();
            }
        }

        private void OpenSaveDialog(QueryTabItem tab)
        {
            this.SaveDialog.FileName  = tab.Name;

            // Открываем диалог
            this.SaveDialog.Filter = "Text documents (.txt)|*.txt";
            this.SaveDialog.DefaultExt = ".txt";
            DialogResult result = this.SaveDialog.ShowDialog();

            // Если нажали "ОК"
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // Сохраняем документ
                var fileInfo = this.SaveFile(this.SaveDialog.FileName, tab.QueryText.Text);
                tab.FilePath = fileInfo.FullName;
                tab.SetHeader(fileInfo.Name);
            }
        }

        private void NewTab()
        {
            this.CreateNewTab(this.GenerateNewTabName());
        }

        private void CloneTab(QueryTabItem tab)
        {
            this.CreateNewTab(this.GenerateNewTabName(), tab.QueryText.Text);
        }

        /// <summary>
        /// Удаляет из коллекции переданный таб
        /// </summary>
        /// <param name="sender">
        /// object
        /// </param>
        /// <param name="e">
        /// EventArgs
        /// </param>
        private void CloseTab(QueryTabItem tab)
        {
            this.QueryTabControl.Items.Remove(tab);
        }

        private void CloseAllTabs()
        {
            this.QueryTabControl.Items.Clear();
        }

        /// <summary>
        /// Удаляет из коллекции все табы кроме переданного
        /// </summary>
        /// <param name="tab">object</param>
        /// <param name="e">EventArgs</param>
        private void CloseAllTabsButThis(QueryTabItem tab)
        {
            while (this.QueryTabControl.Items.Count > 1)
            {
                for (int i = 0; i < this.QueryTabControl.Items.Count; i++)
                {
                    var index = this.QueryTabControl.Items.IndexOf(tab);
                    if (i != index)
                    {
                        this.QueryTabControl.Items.RemoveAt(i);
                    }
                }
            }
        }

        private void OpenQuery()
        {
            if (this.OpenDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.OpenFile(this.OpenDialog.FileName, this.OpenDialog.SafeFileName);
            }
        }

        private void SaveQuery(QueryTabItem tab)
        {
            if (tab == null)
            {
                return;
            }

            // Определяем - сохраненный или загруженный
            if (string.IsNullOrWhiteSpace(tab.FilePath))
            {
                // Новый. Открываем диалог.
                this.OpenSaveDialog(tab);
            }
            else
            {
                // Старый. Просто перезаписываем файл
                this.SaveFile(tab.FilePath, tab.QueryText.Text);
            }
        }

        private void SaveQueryAs(QueryTabItem tab)
        {
            this.OpenSaveDialog(tab);
        }

        private void SaveAllQueries()
        {
            foreach (QueryTabItem item in this.QueryTabControl.Items.SourceCollection)
            {
                this.OpenSaveDialog(item);
            }
        }

        private void Execute(QueryTabItem tab)
        {
            if (tab == null)
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

            this.SaveDialog.Filter = "Excel 2007 documents (.xlsx)|*.xlsx";
            this.SaveDialog.DefaultExt = "xlsx";
            DialogResult result = this.SaveDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                var fileInfo = tab.ExportToExcel(this.SaveDialog.FileName);
                var msg = fileInfo.Exists ? "Экспорт успешно выполнен!" : "Не удалось выполнить экспорт!";
                MessageBox.Show(msg);
            }
        }

        private string GenerateNewTabName()
        {
            this.ItemIndexCounter++;
            string name = string.Format("Query {0}", this.ItemIndexCounter);
            return name;
        }

        #endregion

        #region Events

        private void SubscribeEventHandlers()
        {
            this.EventLabel.MouseDoubleClick += (sender, args) => this.NewTab();
        }

        #endregion
    }
}
