namespace QueryConsole.API.Infrastructure
{
    public static class QueryParser
    {
        #region Public Methods

        public static string Parse(string query)
        {
            return Split(query);
        }

        #endregion

        #region Private methods

        private static string Split(string query)
        {
            int endIndex = query.IndexOf(";", 0, System.StringComparison.Ordinal);

            if (endIndex == -1)
            {
                return query;
            }

            return query.Substring(0, endIndex);
        }

        #endregion
    }
}
