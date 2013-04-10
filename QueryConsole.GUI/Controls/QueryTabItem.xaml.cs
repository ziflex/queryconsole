// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryTabItem.xaml.cs" company="">
//   
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace QueryConsole.Controls
{
    #region

    using System;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;

    using QueryConsole.API.Data;
    using QueryConsole.API.Infrastructure;
    using QueryConsole.API.Models;
    using QueryConsole.Resources;

    #endregion

    public partial class QueryTabItem : TabItem
    {
        #region Fields

        /// <summary>
        /// Minimize / Maximize result field attribute
        /// </summary>
        private bool _isResultRowHidden;

        /// <summary>
        /// Query result
        /// </summary>
        private DataSet _lastQueryResult;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Create new instance of query tab
        /// </summary>
        /// <param name="index">current tab index</param>
        public QueryTabItem(int index)
            : this(index, string.Empty, string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// Create new instance of query tab
        /// </summary>
        /// <param name="index">current tab index</param>
        /// <param name="name">tab's name</param>
        public QueryTabItem(int index, string name)
            : this(index, name, string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// Create new instance of query tab
        /// </summary>
        /// <param name="index">current tab index</param>
        /// <param name="name">tab's name</param>
        /// <param name="text">query text</param>
        public QueryTabItem(int index, string name, string text)
            : this(index, name, text, string.Empty)
        {
        }

        /// <summary>
        /// Create new instance of query tab
        /// </summary>
        /// <param name="index">current tab index</param>
        /// <param name="name">tab's name</param>
        /// <param name="text">query text</param>
        /// <param name="filePath">saved query file path</param>
        public QueryTabItem(int index, string name, string text, string filePath)
        {
            this.InitializeComponent();

            this.ItemIndex = index;
            this.Header = name;
            this.QueryText.Text = text;
            this.FilePath = filePath;

            this.BindEventHandlers();
            this.InitObjects();
            this.InitContextMenu();
            this.ChangeVisibilityResultRow(false);
        }

        #endregion

        #region Delegates

        /// <summary>
        /// for tab's events
        /// </summary>
        /// <param name="tab"></param>
        public delegate void TabEvent(QueryTabItem tab);

        #endregion

        #region Public Events

        /// <summary>
        /// Invokes to create new instance of query tab based on current
        /// </summary>
        public event TabEvent CloneEvent;

        /// <summary>
        /// Invokes to close all tabs but this
        /// </summary>
        public event TabEvent CloseAllButThisEvent;

        /// <summary>
        /// Invokes to close current tab
        /// </summary>
        public event TabEvent CloseEvent;

        /// <summary>
        /// Invokes to create new instance of query tab 
        /// </summary>
        public event TabEvent NewEvent;

        /// <summary>
        /// Invokes to save query text from current tab and create name for it
        /// </summary>
        public event TabEvent SaveAsEvent;

        /// <summary>
        /// Invokes to save query text from current tab (update if it has been loaded before)
        /// </summary>
        public event TabEvent SaveEvent;

        #endregion

        #region Public Properties

        /// <summary>
        /// File path for saved query 
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Current index in a tab collection
        /// </summary>
        public int ItemIndex { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Executes current query
        /// </summary>
        public void ExecQuery()
        {
            this.ValidateQuery();

            var db =
                new DbManager(
                    new DbConfiguration(
                        ((DbProvider)this.ProviderList.SelectedItem).Value, 
                        (DbConnectionString)this.ConStrList.SelectedItem));
            try
            {
                // start timer to measure execution time
                Stopwatch timer = Stopwatch.StartNew();

                this._lastQueryResult = db.ExecQuery(this.GetQuery());

                timer.Stop();

                try
                {
                    // show only if query succeeded
                    this.ShowTimeSpan(timer.ElapsedMilliseconds);

                    // show query result
                    this.ShowResult(this._lastQueryResult);
                }
                catch (Exception)
                {
                    // NoneQuery execution (update / delete)
                    this.ShowError(Resource.Q_Result_NonQuery);
                }
            }
            catch (Exception e)
            {
                // something wrong with query
                this.ShowError(e.Message);
            }
            finally
            {
                db.Dispose();
            }

            // show result field
            this.ChangeVisibilityResultRow(true);
        }

        /// <summary>
        /// Exports query result to excel file
        /// </summary>
        /// <param name="name">File name</param>
        /// <returns>File info</returns>
        public FileInfo ExportToExcel(string name)
        {
            if (this.QueryResult.ItemsSource == null)
            {
                return new FileInfo(name);
            }

            // create new instance of excel writer class and delegate all work
            var excelWriter = new ExcelWriter(this._lastQueryResult.Tables[0], name);
            return excelWriter.Execute();
        }

        /// <summary>
        /// Set tab new header
        /// </summary>
        /// <param name="header"></param>
        public void SetHeader(string header)
        {
            ((ContentControl)this.Header).Content = header;
        }

        #endregion

        #region Methods

        private void BindEventHandlers()
        {
            this.ProviderList.SelectionChanged += this.OnProviderChange;
            this.btnRun.Click += (sender, args) => this.ExecQuery();
            this.btnClose.Click += (sender, args) => this.InvokeEventHandler(this.CloseEvent);
            this.btnHidden.Click += (sender, args) =>
                {
                    this.ChangeVisibilityResultRow(this._isResultRowHidden);
                    this.QueryTimeSpan.Content = string.Empty;
                };
        }

        /// <summary>
        /// Changes visibility of result grid.
        /// </summary>
        /// <param name="control">
        /// The control.
        /// </param>
        private void ChangeVisibilityResultGrid(object control)
        {
            this.QueryResult.Visibility = Visibility.Hidden;
            this.ErrorText.Visibility = Visibility.Hidden;

            // query succeeded - show result field
            if (control == this.QueryResult)
            {
                this.QueryResult.Visibility = Visibility.Visible;
            }

            // query failed - show error field
            if (control == this.ErrorText)
            {
                this.ErrorText.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Minimize / Maximize of result/error fields
        /// </summary>
        /// <param name="visibility"></param>
        private void ChangeVisibilityResultRow(bool visibility)
        {
            this._isResultRowHidden = !visibility;

            // minimize / maximize result field
            this.MainGrid.RowDefinitions[4].Height = visibility
                                                         ? new GridLength(240, GridUnitType.Star)
                                                         : new GridLength(0, GridUnitType.Star);

            // change button's text
            this.btnHidden.Content = visibility ? "Hide" : "Show";

            // if visibility - false, we don't need grid splitter
            this.GridSplitter.IsEnabled = visibility;
        }

        /// <summary>
        /// Returns query text from tab
        /// </summary>
        /// <returns></returns>
        private string GetQuery()
        {
            // return selected text
            if (!string.IsNullOrWhiteSpace(this.QueryText.SelectedText))
            {
                return this.QueryText.SelectedText;
            }

            // return all text
            return this.QueryText.Text;
        }

        /// <summary>
        /// Initializes context menu
        /// </summary>
        private void InitContextMenu()
        {
            var menu = new ContextMenu();

            var miClose = new MenuItem();
            miClose.Name = "miClose";
            miClose.Header = "Close";
            miClose.Click += (sender, args) => this.InvokeEventHandler(this.CloseEvent);
            menu.Items.Add(miClose);

            var miCloseOther = new MenuItem();
            miCloseOther.Name = "miCloseOther";
            miCloseOther.Header = "Close all BUT this";
            miCloseOther.Click += (sender, args) => this.InvokeEventHandler(this.CloseAllButThisEvent);
            menu.Items.Add(miCloseOther);

            var sSeparator = new Separator();
            menu.Items.Add(sSeparator);

            var miSave = new MenuItem();
            miSave.Name = "miSave";
            miSave.Header = "Save";
            miSave.Click += (sender, args) => this.InvokeEventHandler(this.SaveEvent);
            menu.Items.Add(miSave);

            var miSaveAs = new MenuItem();
            miSaveAs.Name = "miSaveAs";
            miSaveAs.Header = "Save As";
            miSaveAs.Click += (sender, args) => this.InvokeEventHandler(this.SaveAsEvent);
            menu.Items.Add(miSaveAs);

            var sSeparator2 = new Separator();
            menu.Items.Add(sSeparator2);

            var miNew = new MenuItem();
            miNew.Name = "miNew";
            miNew.Header = "New";
            miNew.Click += (sender, args) => this.InvokeEventHandler(this.NewEvent);
            menu.Items.Add(miNew);

            var miClone = new MenuItem();
            miClone.Name = "miClone";
            miClone.Header = "Clone";
            miClone.Click += (sender, args) => this.InvokeEventHandler(this.CloneEvent);
            menu.Items.Add(miClone);

            var contentControl = new ContentControl { Content = this.Header, ContextMenu = menu };
            contentControl.ContextMenuOpening += (sender, args) =>
                {
                    var cc = (ContentControl)sender;

                    // Save
                    ((MenuItem)cc.ContextMenu.Items[3]).IsEnabled = !string.IsNullOrWhiteSpace(this.FilePath);
                };

            this.Header = contentControl;
        }

        /// <summary>
        /// Initializes working objects.
        /// </summary>
        private void InitObjects()
        {
            this.ProviderList.ItemsSource = ((App)Application.Current).AppConfiguration.DbProviders;
            if (this.ProviderList.Items.Count != 0)
            {
                this.ProviderList.SelectedItem = this.ProviderList.Items[0];
            }
        }

        /// <summary>
        /// Wrapper for invoking event handlers
        /// </summary>
        /// <param name="e"></param>
        private void InvokeEventHandler(TabEvent e)
        {
            if (e != null)
            {
                e.Invoke(this);
            }
        }

        /// <summary>
        /// Handles provider changing
        /// </summary>
        /// <param name="sender">Provider list</param>
        /// <param name="e">args</param>
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

        /// <summary>
        /// Shows duration of query execution
        /// </summary>
        /// <param name="miliseconds">duration</param>
        private void ShowTimeSpan(long miliseconds)
        {
            this.QueryTimeSpan.Content = string.Format("Duration: {0} (ms)", miliseconds);
        }

        /// <summary>
        /// Validates query text before execution
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// </exception>
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

        #endregion
    }
}