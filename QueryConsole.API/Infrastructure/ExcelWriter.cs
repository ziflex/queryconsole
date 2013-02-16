// -----------------------------------------------------------------------
// <copyright file="ExcelWriter.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace QueryConsole.API.Infrastructure
{
    using System.Data;
    using System.IO;

    using OfficeOpenXml;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ExcelWriter
    {
        #region Properties

        private DataTable _dataSource;

        private string _fileName;

        #endregion

        #region Constructors

        public ExcelWriter(DataTable source) : this(source, string.Empty) {}

        public ExcelWriter(DataTable source, string fileName)
        {
            this._dataSource = source;
            this._fileName = fileName;
        }

        #endregion

        #region Public Methods

        public FileInfo Execute()
        {
            var newFile = new FileInfo(this._fileName);

            if (newFile.Exists)
            {
                newFile.Delete();
            }

            using (var pck = new ExcelPackage(newFile))
            {
                this.ExecuteInternal(pck.Workbook.Worksheets.Add("Content"));
                pck.Save();
            }

            newFile.Refresh();
            return newFile;
        }

        #endregion

        #region Private Methods

        private void ExecuteInternal(ExcelWorksheet ws)
        {
            this.FillHeaders(ws);
            this.FillContent(ws);
        }

        private void FillHeaders(ExcelWorksheet ws)
        {
            const int rowsCounter = 1;
            int columnsCounter = 1;
            foreach (DataColumn column in this._dataSource.Columns)
            {
                ws.Cells[rowsCounter, columnsCounter].Value = column.ColumnName;
                columnsCounter++;
            }
        }

        private void FillContent(ExcelWorksheet ws)
        {
            int rowsCounter = 2;
            foreach (DataRow row in this._dataSource.Rows)
            {
                int columnsCounter = 1;

                foreach (DataColumn column in this._dataSource.Columns)
                {
                    ws.Cells[rowsCounter, columnsCounter].Value = row[column];
                    columnsCounter++;
                }

                rowsCounter++;
            }
        }

        #endregion
    }
}
