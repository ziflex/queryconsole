// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObservableStringBuilder.cs" company="">
//   
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace QueryConsole.API.Infrastructure
{
    #region

    using System;
    using System.Text;

    #endregion

    /// <summary>
    /// StringBuilder wrapper with content changing event
    /// </summary>
    public class ObservableStringBuilder
    {
        #region Fields

        private readonly StringBuilder _context;

        #endregion

        #region Constructors and Destructors

        public ObservableStringBuilder()
        {
            this._context = new StringBuilder();
        }

        public ObservableStringBuilder(string str)
        {
            this._context = new StringBuilder(str);
        }

        public ObservableStringBuilder(StringBuilder stringBuilder)
        {
            this._context = stringBuilder;
        }

        #endregion

        #region Public Events

        public event EventHandler ContentChanged;

        #endregion

        #region Public Properties

        public int Length
        {
            get
            {
                return this._context.Length;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Append(string str)
        {
            this._context.Append(str);
            this.InvokeHandlers(this.ContentChanged);
        }

        public void Clear()
        {
            this._context.Clear();
            this.InvokeHandlers(this.ContentChanged);
        }

        public void Remove(int startIndex, int length)
        {
            this._context.Remove(startIndex, length);
            this.InvokeHandlers(this.ContentChanged);
        }

        public override string ToString()
        {
            return this._context.ToString();
        }

        #endregion

        #region Methods

        private void InvokeHandlers(EventHandler e)
        {
            if (e != null)
            {
                e.Invoke(this, null);
            }
        }

        #endregion
    }
}