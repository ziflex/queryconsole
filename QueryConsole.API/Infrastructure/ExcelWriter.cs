// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExcelWriter.cs" company="">
//   
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace QueryConsole.API.Infrastructure
{
    using System.Data;
    using System.IO;

    using OfficeOpenXml;

    public class ExcelWriter
    {
        #region Fields

        private readonly DataTable _dataSource;

        private readonly string _fileName;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelWriter"/> class. 
        /// </summary>
        /// <param name="source">
        /// data to export
        /// </param>
        public ExcelWriter(DataTable source)
            : this(source, string.Empty)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelWriter"/> class. 
        /// </summary>
        /// <param name="source">
        /// data to export
        /// </param>
        /// <param name="fileName">
        /// File name
        /// </param>
        public ExcelWriter(DataTable source, string fileName)
        {
            this._dataSource = source;
            this._fileName = fileName;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Export
        /// </summary>
        /// <returns>File info</returns>
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

        #region Methods

        private void ExecuteInternal(ExcelWorksheet ws)
        {
            this.FillHeaders(ws);
            this.FillContent(ws);
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

        private void FillHeaders(ExcelWorksheet ws)
        {
            const int RowsCounter = 1;
            int columnsCounter = 1;
            foreach (DataColumn column in this._dataSource.Columns)
            {
                ws.Cells[RowsCounter, columnsCounter].Value = column.ColumnName;
                columnsCounter++;
            }
        }

        #endregion
    }
}