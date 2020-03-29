using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vialidad.Utils.Extensions;

namespace Vialidad.Utils.Export
{
    public class ExcelExport<T>
    {
        #region Atributos privados
        private string FileName { get; set; }
        private List<T> DataToExport { get; set; }
        private List<string> PropertiesToIgnore { get; set; }
        private List<PropertyInfo> FieldsInfo { get; set; }
        private Dictionary<TypeCode, ICellStyle> CellStyles { get; set; }
        private int RowIndex { get; set; }
        #endregion

        #region Constructores
        public ExcelExport(List<T> dataToExport, List<string> propertiesToIgnore, string fileName)
        {
            this.RowIndex = 0;
            this.DataToExport = dataToExport;
            this.PropertiesToIgnore = propertiesToIgnore ?? new List<string>();
            this.FileName = fileName;
            this.FieldsInfo = typeof(T).GetProperties().ToList();
            this.CellStyles = new Dictionary<TypeCode, ICellStyle>();
        }
        #endregion

        #region Overrides
        public void ExecuteResult()
        {
            var workbook = new HSSFWorkbook();
            var sheet = workbook.CreateSheet(this.FileName);

            //Inicializamos los estilos disponibles
            this.InitializeCellStyles(ref workbook);

            //Cargamos encabezado
            this.LoadHeader(ref sheet, ref workbook);
            this.RowIndex++;

            //Cargamos los registros
            if (this.DataToExport.Count > 0)
                this.LoadBody(ref sheet, ref workbook);

            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                StreamWriter writer = new StreamWriter(ms);
                ms.Seek(0, SeekOrigin.Begin);
                string fileName = @"C:\Tempo\Files\" + DateTime.Now.Ticks + "_Rutas.xls";
                using (FileStream fs = new FileStream(@fileName, FileMode.OpenOrCreate))
                {
                    ms.CopyTo(fs);
                    fs.Flush();
                }
            }
        }
        #endregion

        #region Helpers
        private void InitializeCellStyles(ref HSSFWorkbook workbook)
        {
            //Creamos estilo de celda de tipo string
            var cellDataStyle = workbook.CreateCellStyle();
            cellDataStyle = workbook.CreateCellStyle();
            cellDataStyle.Alignment = HorizontalAlignment.Left;

            //Creamos estilo de celda de tipo fecha
            var cellDateStyle = workbook.CreateCellStyle();
            cellDateStyle.DataFormat = workbook.CreateDataFormat().GetFormat("dd/MM/yyyy");

            //Creamos estilo de celda de tipo decimal
            var cellDecimalStyle = workbook.CreateCellStyle();
            cellDecimalStyle.Alignment = HorizontalAlignment.Right;
            cellDecimalStyle.DataFormat = workbook.CreateDataFormat().GetFormat("0.00");

            //Creamos estilo de celda de tipo boolean
            var cellBooleanStyle = workbook.CreateCellStyle();
            cellBooleanStyle.Alignment = HorizontalAlignment.Right;

            this.CellStyles.Add(TypeCode.String, cellDataStyle);
            this.CellStyles.Add(TypeCode.DateTime, cellDateStyle);
            this.CellStyles.Add(TypeCode.Decimal, cellDecimalStyle);
            this.CellStyles.Add(TypeCode.Double, cellDecimalStyle);
            this.CellStyles.Add(TypeCode.Boolean, cellBooleanStyle);
        }

        private ICellStyle FormatHeader(ref HSSFWorkbook workbook)
        {
            var styleCell = workbook.CreateCellStyle();
            var styleFont = workbook.CreateFont();
            styleFont.Boldweight = (short)FontBoldWeight.Bold;
            styleCell.BorderTop = BorderStyle.Double;
            styleCell.BorderBottom = BorderStyle.Double;
            styleCell.BorderLeft = BorderStyle.Thin;
            styleCell.BorderRight = BorderStyle.Thin;
            styleCell.Alignment = HorizontalAlignment.Left;
            styleCell.SetFont(styleFont);
            return styleCell;
        }

        private void LoadHeader(ref ISheet sheet, ref HSSFWorkbook workbook)
        {
            var row = sheet.CreateRow(this.RowIndex);
            for (int iPos = 0; iPos < this.FieldsInfo.Count; iPos++)
            {
                var fieldInfo = this.FieldsInfo[iPos];
                if (this.PropertiesToIgnore.Contains(fieldInfo.Name))
                    continue;

                var cell = row.CreateCell(iPos);
                cell.SetCellValue(fieldInfo.Name);
                cell.CellStyle = this.FormatHeader(ref workbook);
            }
        }

        private void LoadBody(ref ISheet sheet, ref HSSFWorkbook workbook)
        {
            foreach (T itemData in this.DataToExport)
            {
                var row = sheet.CreateRow(this.RowIndex);
                for (int iPos = 0; iPos < this.FieldsInfo.Count; iPos++)
                {
                    var fieldInfo = this.FieldsInfo[iPos];
                    if (this.PropertiesToIgnore.Contains(fieldInfo.Name))
                        continue;

                    var cell = row.CreateCell(iPos);
                    var value = fieldInfo.GetValue(itemData, null);
                    var strValue = (value != null) ? value.ToString() : string.Empty;
                    var typeCode = Type.GetTypeCode(value == null ? string.Empty.GetType() : value.GetType());

                    ICellStyle cellStyle = null;
                    if (!this.CellStyles.TryGetValue(typeCode, out cellStyle))
                        this.CellStyles.TryGetValue(TypeCode.String, out cellStyle);

                    cell.SetValue(ref workbook, ref cellStyle, typeCode, strValue);
                }
                this.RowIndex++;
            }
        }
        #endregion
    }
}
