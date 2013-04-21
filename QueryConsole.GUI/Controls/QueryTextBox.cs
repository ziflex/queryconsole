// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryTextBox.cs" company="">
//   
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace QueryConsole.Controls
{
    #region

    using System.Collections.Generic;
    using System.Windows.Controls;
    using System.Windows.Documents;

    using QueryConsole.Extensions;
    using QueryConsole.Extensions.Autocompletion;

    #endregion

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class QueryTextBox : TextBox
    {
        #region Fields

        private Autocomplete _autocomplete;

        #endregion

        #region Public Properties

        public IEnumerable<string> AutocompleteSource
        {
            get
            {
                if (this._autocomplete == null)
                {
                    return null;
                }

                return this._autocomplete.DataSource;
            }

            set
            {
                if (this._autocomplete != null)
                {
                    this._autocomplete.DataSource = value;
                }
            }
        }

        #endregion

        public override void EndInit()
        {
            base.EndInit();
            this._autocomplete = new Autocomplete(this);
        }
    }
}