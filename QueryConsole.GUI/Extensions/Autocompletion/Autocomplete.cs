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

            // default empty source
            this.DataSource = new List<string>();

            this.BindEvents();
        }

        #endregion

        #region Public Properties

        public IEnumerable<string> DataSource { get; set; }

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

        private void BindEvents()
        {
            this._textBox.PreviewTextInput += this.ListenPreviewTextInput;
            this._textBox.PreviewKeyDown += this.ListenKeyDown;

            this._listBox.GotFocus += this.OnGotFocus;

            this._lastWords.ContentChanged += (sender, args) => this.Run();
        }

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
                        int lineIndex = this._textBox.GetLineIndexFromCharacterIndex(this._textBox.CaretIndex);
                        string lineText = this._textBox.GetLineText(lineIndex);
                        string lastWord = lineText.Split(' ').LastOrDefault();

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

        private void InsertWord(string text, int removeFrom)
        {
            this._textBox.Text = this._textBox.Text.Remove(removeFrom);
            this._textBox.AppendText(text);
            this._textBox.CaretIndex = this._textBox.Text.Length;
        }

        private void InsertWord()
        {
            if (this._listBox.SelectedIndex == -1)
            {
                return;
            }

            string text = string.Format("{0} ", this._listBox.SelectedItem);
            int removeFrom = this._textBox.CaretIndex > 0 ? this._textBox.CaretIndex - this._lastWords.Length : 0;

            this.InsertWord(text, removeFrom);

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

        private IEnumerable<string> Match(string text)
        {
            return this.DataSource.Where(
                i => !string.IsNullOrWhiteSpace(text) && i.ToUpper().StartsWith(text.ToUpper()));
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            this._listBox.SelectedItem = ((ListBoxItem)e.OriginalSource).Content;
            this.InsertWord();
            this._textBox.Focus();
        }

        private void ResetLocation()
        {
            Rect rect = this._textBox.GetRectFromCharacterIndex(this._textBox.CaretIndex);
            double left = rect.X >= 15 ? rect.X : 15;
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

        private void Run()
        {
            // don't change position if listbox is opened 
            if (!this.IsVisible)
            {
                this.ResetLocation();
            }

            // match input text
            this.Run(this._lastWords.ToString());
        }

        private void Run(string word)
        {
            // find matching words
            this._listBox.ItemsSource = this.Match(word);
            this._listBox.SelectedIndex = 0;

            // show listbox if it doesn't empty
            this.IsVisible = this._listBox.HasItems;
        }

        #endregion
    }
}