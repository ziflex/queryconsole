// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryParser.cs" company="">
//   
// </copyright>
// <summary>
//   The query parser.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace QueryConsole.API.Infrastructure
{
    using System;

    public static class QueryParser
    {
        #region Public Methods and Operators

        public static string Parse(string query)
        {
            return Split(query);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Splits queries separated by ';' char
        /// </summary>
        /// <param name="query">text</param>
        /// <returns>First query</returns>
        private static string Split(string query)
        {
            int endIndex = query.IndexOf(";", 0, StringComparison.Ordinal);

            if (endIndex == -1)
            {
                return query;
            }

            return query.Substring(0, endIndex);
        }

        #endregion
    }
}