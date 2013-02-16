namespace QueryConsole.Controls
{
    using System;
    using System.Data;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;

    using QueryConsole.API.Data;
    using QueryConsole.API.Infrastructure;
    using QueryConsole.API.Models;
    using QueryConsole.Resources;

    /// <summary>
    /// Логика взаимодействия для QueryTabItem.xaml
    /// </summary>
    public partial class QueryTabItem : TabItem
    {
        #region Properties

        private bool IsResultRowHidden { get; set; }

        private DataSet _lastQueryResult { get; set; }

        public delegate void TabEvent(QueryTabItem tab);

        /// <summary>
        /// Вызывает метод для закрытия текущего таба
        /// </summary>
        public event TabEvent CloseEvent;

        /// <summary>
        /// Вызывает метод для закрытия всех табов кроме текущего
        /// </summary>
        public event TabEvent CloseAllButThisEvent;

        /// <summary>
        /// Вызывает метод для сохранения текущего таба
        /// </summary>
        public event TabEvent SaveEvent;

        /// <summary>
        /// Вызывает метод для сохранения текущего таба с выбором имени
        /// </summary>
        public event TabEvent SaveAsEvent;

        /// <summary>
        /// Вызывает метод для создания нового таба
        /// </summary>
        public event TabEvent NewEvent;

        /// <summary>
        /// Вызывает метод для создания нового таба на основании текущего
        /// </summary>
        public event TabEvent CloneEvent;

        public string FilePath { get; set; }

        public int ItemIndex { get; set; }

        #endregion

        #region Constructors

        public QueryTabItem(int index) : this(index, string.Empty, string.Empty, string.Empty) { }

        public QueryTabItem(int index, string name) : this(index, name, string.Empty, string.Empty) { }

        public QueryTabItem(int index, string name, string text) : this(index, name, text, string.Empty) { }

        public QueryTabItem(int index, string name, string text, string filePath)
        {
            this.InitializeComponent();

            this.ItemIndex         = index;
            this.Header            = name;
            this.QueryText.Text    = text;
            this.FilePath          = filePath;
            
            this.RegisterEvents();
            this.InitObjects();
            this.InitContextMenu();
            this.ChangeVisibilityResultRow(false);
        }

        #endregion

        #region Public Methods

        public void ExecQuery()
        {
            this.ValidateQuery();

            DateTime startTime = new DateTime();
            DateTime endTime = new DateTime();

            DbManager db = new DbManager(new DbConfiguration(((DbProvider)this.ProviderList.SelectedItem).Value, (DbConnectionString)this.ConStrList.SelectedItem));
            try
            {
                startTime = DateTime.Now;

                this._lastQueryResult = db.ExecQuery(this.GetQuery());

                endTime = DateTime.Now;

                try
                {
                    // Выводим время только в том случае, если запрос успешно завершен
                    this.ShowTimeSpan(startTime, endTime);

                    // Выводим результат запроса 
                    this.ShowResult(this._lastQueryResult);
                }
                catch (Exception)
                {
                    // Выводим если запрос был типа NonQuery 
                    this.ShowError("Command completed!");
                }
            }
            catch (Exception e)
            {
                this.ShowError(e.Message);
            }
            finally
            {
                db.Dispose();
            }

            this.ChangeVisibilityResultRow(true);
        }

        public FileInfo ExportToExcel(string name)
        {
            if (this.QueryResult.ItemsSource == null)
            {
                return new FileInfo(name);
            }

            var excelWriter = new ExcelWriter(this._lastQueryResult.Tables[0], name);
            return excelWriter.Execute();
        }

        public void SetHeader(string header)
        {
            ((ContentControl)this.Header).Content = header;
        }

        #endregion

        #region Private Methods

        private void InitContextMenu()
        {
            ContextMenu menu = new ContextMenu();

            MenuItem miClose = new MenuItem();
            miClose.Name = "miClose";
            miClose.Header = "Close";
            miClose.Click += (sender, args) => this.InvokeEventHandler(this.CloseEvent);
            menu.Items.Add(miClose);

            MenuItem miCloseOther = new MenuItem();
            miCloseOther.Name = "miCloseOther";
            miCloseOther.Header = "Close all BUT this";
            miCloseOther.Click += (sender, args) => this.InvokeEventHandler(this.CloseAllButThisEvent);
            menu.Items.Add(miCloseOther);

            Separator sSeparator = new Separator();
            menu.Items.Add(sSeparator);

            MenuItem miSave = new MenuItem();
            miSave.Name = "miSave";
            miSave.Header = "Save";
            miSave.Click += (sender, args) => this.InvokeEventHandler(this.SaveEvent);
            menu.Items.Add(miSave);

            MenuItem miSaveAs = new MenuItem();
            miSaveAs.Name = "miSaveAs";
            miSaveAs.Header = "Save As";
            miSaveAs.Click += (sender, args) => this.InvokeEventHandler(this.SaveAsEvent);
            menu.Items.Add(miSaveAs);

            Separator sSeparator2 = new Separator();
            menu.Items.Add(sSeparator2);

            MenuItem miNew = new MenuItem();
            miNew.Name = "miNew";
            miNew.Header = "New";
            miNew.Click += (sender, args) => this.InvokeEventHandler(this.NewEvent);
            menu.Items.Add(miNew);

            MenuItem miClone = new MenuItem();
            miClone.Name = "miClone";
            miClone.Header = "Clone";
            miClone.Click += (sender, args) => this.InvokeEventHandler(this.CloneEvent);
            menu.Items.Add(miClone);

            ContentControl contentControl = new ContentControl { Content = this.Header, ContextMenu = menu };
            contentControl.ContextMenuOpening += (sender, args) =>
            {
                ContentControl cc = (ContentControl)sender;

                // TODO: Найти способ получение элементов по их имени, а не по индексу
                // Save
                ((MenuItem)cc.ContextMenu.Items[3]).IsEnabled = !string.IsNullOrWhiteSpace(this.FilePath);
            };

            this.Header = contentControl;
        }

        private void ValidateQuery()
        {
            if (this.ProviderList.SelectedItem == null)
            {
                throw new ArgumentNullException(Resource.Q_ProviderSelectionError);
            }

            if (this.ConStrList.SelectedItem == null)
            {
                throw new ArgumentNullException(Resource.Q_ConStrSelectionError);
            }

            if (string.IsNullOrWhiteSpace(this.QueryText.Text))
            {
                throw new ArgumentNullException(Resource.Q_EmptyQueryError);
            }
        }

        private string GetQuery()
        {
            if (!string.IsNullOrWhiteSpace(this.QueryText.SelectedText))
            {
                return this.QueryText.SelectedText;
            }

            return this.QueryText.Text;
        }

        private void InitObjects()
        {
            this.ProviderList.ItemsSource = ((App)App.Current).AppConfiguration.DbProviders;
            if (this.ProviderList.Items.Count != 0)
            {
                this.ProviderList.SelectedItem = this.ProviderList.Items[0];
            }
        }

        private void ShowTimeSpan(DateTime startTime, DateTime endTime)
        {
            TimeSpan timeSpan = endTime - startTime;
            this.QueryTimeSpan.Content = timeSpan;
        }

        private void ShowError(string errMsg)
        {
            this.ErrorText.Text = errMsg;
            this.ChangeVisibilityResultGrid(this.ErrorText);
        }

        private void ShowResult(DataSet result)
        {
            this.QueryResult.ItemsSource = result.Tables[0].DefaultView;
            this.ChangeVisibilityResultGrid(this.QueryResult);
        }

        private void ChangeVisibilityResultGrid(object control)
        {
            this.QueryResult.Visibility = Visibility.Hidden;
            this.ErrorText.Visibility = Visibility.Hidden;

            if (control == this.QueryResult)
            {
                this.QueryResult.Visibility = Visibility.Visible;
            }

            if (control == this.ErrorText)
            {
                this.ErrorText.Visibility = Visibility.Visible;
            }
        }

        private void ChangeVisibilityResultRow(bool visibility)
        {
            this.IsResultRowHidden                 = !visibility;
            this.MainGrid.RowDefinitions[4].Height = visibility ? new GridLength(240, GridUnitType.Star) : new GridLength(0, GridUnitType.Star);
            this.btnHidden.Content                 = visibility ? "Hide" : "Show";
        }

        #endregion

        #region Events

        private void InvokeEventHandler(TabEvent e)
        {
            if (e != null)
            {
                e.Invoke(this);
            }
        }

        private void OnProviderChange(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems == null) || (e.AddedItems.Count == 0))
            {
                return;
            }

            this.ConStrList.ItemsSource = ((DbProvider)e.AddedItems[0]).ConnectionStrings;

            if (this.ConStrList.Items.Count != 0)
            {
                this.ConStrList.SelectedItem = this.ConStrList.Items[0];
            }
        }

        private void RegisterEvents()
        {
            this.ProviderList.SelectionChanged += this.OnProviderChange;
            this.btnRun.Click += (sender, args) => this.ExecQuery();
            this.btnClose.Click += (sender, args) => this.InvokeEventHandler(this.CloseEvent);
            this.btnHidden.Click += (sender, args) =>
                {
                    this.ChangeVisibilityResultRow(this.IsResultRowHidden);
                    this.QueryTimeSpan.Content = string.Empty;
                };
        }

        #endregion
    }
}
