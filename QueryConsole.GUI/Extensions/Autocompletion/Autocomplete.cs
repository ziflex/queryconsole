// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Autocomplete.cs" company="">
//   
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace QueryConsole.Extensions.Autocompletion
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using QueryConsole.API.Infrastructure;

    #endregion

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Autocomplete
    {
        #region Fields

        private readonly ObservableStringBuilder _lastWords;

        private readonly ListBox _listBox;

        private readonly TextBox _textBox;

        #endregion

        #region Constructors and Destructors

        public Autocomplete(TextBox textBox)
        {
            this._textBox = textBox;

            var parent = this._textBox.Parent as Grid;
            if (parent == null)
            {
                throw new Exception("This control must be put in Grid control");
            }

            // list of autocomplete
            this._listBox = new ListBox();

            // add listbox to parent
            parent.Children.Add(this._listBox);

            // set listbox's view
            this._listBox.MaxHeight = 100;
            this._listBox.MinWidth = 100;
            this._listBox.HorizontalAlignment = HorizontalAlignment.Left;
            this._listBox.VerticalAlignment = VerticalAlignment.Top;
            this._listBox.Visibility = Visibility.Collapsed;

            // last chars of started word
            this._lastWords = new ObservableStringBuilder();
        }

        #endregion

        #region Public Properties

        private IEnumerable<string> _dataSource;

        public IEnumerable<string> DataSource
        {
            get
            {
                return this._dataSource;
            }

            set
            {
                if (value != null && value.Any())
                {
                    this.BindEvents();
                }
                else
                {
                    // don't listen events if there is no autocompletion content
                    this.UnbindEvents();
                }

                this._dataSource = value;
            }
        }

        public bool IsVisible
        {
            get
            {
                return this._listBox.Visibility == Visibility.Visible;
            }

            set
            {
                this._listBox.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds listeners
        /// </summary>
        private void BindEvents()
        {
            this._textBox.PreviewTextInput += this.ListenPreviewTextInput;
            this._textBox.PreviewKeyDown += this.ListenKeyDown;

            this._listBox.GotFocus += this.OnGotFocus;

            this._lastWords.ContentChanged += this.Run;
        }

        /// <summary>
        /// Removes all listeneres
        /// </summary>
        private void UnbindEvents()
        {
            this._textBox.PreviewTextInput -= this.ListenPreviewTextInput;
            this._textBox.PreviewKeyDown -= this.ListenKeyDown;

            this._listBox.GotFocus -= this.OnGotFocus;

            this._lastWords.ContentChanged -= this.Run;
        }

        /// <summary>
        /// Handles input keys
        /// </summary>
        /// <param name="key">Input key</param>
        /// <returns>Handled or not</returns>
        private bool HandleKey(Key key)
        {
            bool result = false;

            switch (key)
            {
                case Key.Back:
                    if (this._lastWords.Length > 0)
                    {
                        this._lastWords.Remove(this._lastWords.Length - 1, 1);
                    }
                    else
                    {
                        // get last word previously inputted
                        var words = this.GetCurrentLineText().Split(' ');
                        var lastWord = words[words.Length - 1];

                        if (!string.IsNullOrWhiteSpace(lastWord))
                        {
                            // we remove last char as it will be after native handling
                            this._lastWords.Append(lastWord.Remove(lastWord.Length - 1));
                        }
                    }

                    break;
                case Key.Down:
                    if (this.IsVisible)
                    {
                        this.ChangeSelectedIndex(this._listBox.SelectedIndex + 1);
                        result = true;
                    }

                    break;
                case Key.Up:
                    if (this.IsVisible)
                    {
                        this.ChangeSelectedIndex(this._listBox.SelectedIndex - 1);
                        result = true;
                    }

                    break;
                case Key.PageDown:
                    if (this.IsVisible)
                    {
                        this.ChangeSelectedIndex(this._listBox.SelectedIndex + 1);
                        result = true;
                    }

                    break;
                case Key.PageUp:
                    if (this.IsVisible)
                    {
                        this.ChangeSelectedIndex(this._listBox.SelectedIndex - 1);
                        result = true;
                    }

                    break;
                case Key.Home:
                    if (this.IsVisible)
                    {
                        this.ChangeSelectedIndex(0);
                        result = true;
                    }

                    break;
                case Key.End:
                    if (this.IsVisible)
                    {
                        this.ChangeSelectedIndex(this._listBox.Items.Count - 1);
                        result = true;
                    }

                    break;
                case Key.Space:
                    if (this.IsVisible)
                    {
                        this.InsertWord();
                        result = true;
                    }
                    else
                    {
                        this._lastWords.Clear();
                    }

                    break;
                case Key.Enter:
                    if (this.IsVisible)
                    {
                        this.InsertWord();
                        result = true;
                    }
                    else
                    {
                        this._lastWords.Clear();
                    }

                    break;
                case Key.Tab:
                    if (this.IsVisible)
                    {
                        this.InsertWord();
                        result = true;
                    }

                    break;
            }

            return result;
        }

        /// <summary>
        /// Changes selection item in list box
        /// </summary>
        /// <param name="newIndex">item new index</param>
        private void ChangeSelectedIndex(int newIndex)
        {
            if (newIndex < 0)
            {
                newIndex = this._listBox.Items.Count + newIndex;
            }
            else if (newIndex > (this._listBox.Items.Count - 1))
            {
                newIndex = 0;
            }

            this._listBox.SelectedIndex = newIndex;
            this._listBox.ScrollIntoView(this._listBox.SelectedItem);
        }

        private void InsertWord(string text, int removeFrom, int removeCount)
        {
            // remove inputed chars and insert completed word
            this._textBox.Text = this._textBox.Text.Remove(removeFrom, removeCount).Insert(removeFrom, text);

            // set caret after inserted word
            this._textBox.CaretIndex = removeFrom + text.Length;
        }

        private void InsertWord()
        {
            if (this._listBox.SelectedIndex == -1)
            {
                return;
            }

            string text = string.Format("{0} ", this._listBox.SelectedItem);
            int removeFrom = this._textBox.CaretIndex > 0 ? this._textBox.CaretIndex - this._lastWords.Length : 0;

            this.InsertWord(text, removeFrom, this._lastWords.Length);

            this.IsVisible = false;
            this._lastWords.Clear();
        }

        private void ListenKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = this.HandleKey(e.Key);
        }

        private void ListenPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            this._lastWords.Append(e.Text);
        }

        /// <summary>
        /// Finds mathced words
        /// </summary>
        /// <param name="text">inputted chars</param>
        /// <returns>list of matched words</returns>
        private IEnumerable<string> Match(string text)
        {
            return this.DataSource.Where(
                i => !string.IsNullOrWhiteSpace(text) && i.ToUpper().StartsWith(text.ToUpper()));
        }

        /// <summary>
        /// On each getting focus inputs selected word
        /// </summary>
        /// <param name="sender">list box</param>
        /// <param name="e">args</param>
        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            this._listBox.SelectedItem = ((ListBoxItem)e.OriginalSource).Content;
            this.InsertWord();
            this._textBox.Focus();
        }

        /// <summary>
        /// Refresh list box location
        /// </summary>
        private void ResetLocation()
        {
            Rect rect = this._textBox.GetRectFromCharacterIndex(this._textBox.CaretIndex);
            double left = rect.X >= 5 ? rect.X : 5;
            double top = rect.Y >= 20 ? rect.Y + 20 : 20;
            left += this._textBox.Padding.Left;
            top += this._textBox.Padding.Top;

            // if true we should set offset to prevent list box drawing out of the viewport
            if ((left + this._listBox.MinWidth) > this._textBox.ViewportWidth)
            {
                left -= (left + this._listBox.MinWidth) - this._textBox.ViewportWidth;
            }

            this._listBox.SetCurrentValue(FrameworkElement.MarginProperty, new Thickness(left, top, 0, 0));
        }

        /// <summary>
        /// Run autocompletion
        /// </summary>
        /// <param name="sender">Text box</param>
        /// <param name="eventArgs">args</param>
        private void Run(object sender, EventArgs eventArgs)
        {
            // don't change position if listbox is opened 
            if (!this.IsVisible)
            {
                this.ResetLocation();
            }

            // match input text
            this.Run(this._lastWords.ToString());
        }

        /// <summary>
        /// Populates list box datasource by matched words
        /// </summary>
        /// <param name="word"></param>
        private void Run(string word)
        {
            // find matching words
            this._listBox.ItemsSource = this.Match(word);
            this._listBox.SelectedIndex = 0;

            // show listbox if it doesn't empty
            this.IsVisible = this._listBox.HasItems;
        }

        private string GetCurrentLineText()
        {
            int lineIndex = this._textBox.GetLineIndexFromCharacterIndex(this._textBox.CaretIndex);
            return this._textBox.GetLineText(lineIndex);
        }

        #endregion
    }
}