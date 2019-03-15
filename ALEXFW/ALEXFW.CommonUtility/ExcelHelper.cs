using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ALEXFW.Entity.CommonDictionary;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace ALEXFW.CommonUtility
{
    /// <summary>
    ///  NPOI.Excel 工具
    /// </summary>
    public class ExcelHelper
    {
        //private string templateFile = null;
        //private string outputFile = null;
        //private HSSFWorkbook hssfWorkbook;
        //private ISheet sheet;

        public static DataTable GetDataTableByOleDb(string fileName, string tableName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("fileName");
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentException("tableName");

            OleDbConnection conn = null;
            try
            {
                var dataSet = new DataSet();
                var strCon = " Provider = Microsoft.Jet.OLEDB.4.0 ; Data Source = " + fileName +
                             ";Extended Properties='Excel 8.0;HDR=NO;IMEX=1';";
                conn = new OleDbConnection(strCon);
                var strCom = " SELECT *FROM [" + tableName + "$] ";
                conn.Open();

                var command = new OleDbDataAdapter(strCom, conn);
                command.Fill(dataSet, "[" + tableName + "$]");
                conn.Close();
                var dt = new DataTable();
                dt = dataSet.Tables[0];
                return dt;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }

        public static DataTable GetDataTable(string excel, int index, bool header)
        {
            var dt = new DataTable(Path.GetFileNameWithoutExtension(excel) + "_Sheet" + index);
            IWorkbook workbook;
            using (var file = new FileStream(excel, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(file);
            }
            var sheet = workbook.GetSheetAt(index);
            var rows = sheet.GetRowEnumerator();

            var headerRow = sheet.GetRow(0); //第一行为标题行
            var rowcount = sheet.LastRowNum;
            var cellcount = 10;

            rows.MoveNext();
            IRow row = (XSSFRow) rows.Current;

            for (var i = 0; i < cellcount; i++)
            {
                var cell = row.GetCell(i);
                var columnName = header ? cell.StringCellValue : i.ToString();
                dt.Columns.Add(columnName, typeof(string));
            }
            if (!header)
            {
                var first = dt.NewRow();
                for (var i = 0; i < cellcount; i++)
                {
                    var cell = row.GetCell(i);
                    first[i] = cell.StringCellValue;
                }
                dt.Rows.Add(first);
            }

            while (rows.MoveNext())
            {
                row = (XSSFRow) rows.Current;
                var dataRow = dt.NewRow();

                //if (row.Cells.Count() < row.LastCellNum )
                //    break;

                for (var i = 0; i < cellcount; i++)
                {
                    var cell = row.GetCell(i);
                    var cellValue = cell.ToString();

                    dataRow[i] = cellValue; // cell.StringCellValue;
                }
                dt.Rows.Add(dataRow);
            }

            return dt;
        }

        /// <summary>
        ///     DataTable导出到Excel文件
        /// </summary>
        /// <param name="dtSource"> 源DataTable </param>
        /// <param name="strHeaderText"> 表头文本 </param>
        public static MemoryStream ExportMemoryStream(DataTable dtSource, string strHeaderText)
        {
            var workbook = new HSSFWorkbook();
            var sheet = workbook.CreateSheet();

            #region 右击文件 属性信息

            {
                var dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                dsi.Company = "NPOI";
                workbook.DocumentSummaryInformation = dsi;

                var si = PropertySetFactory.CreateSummaryInformation();
                si.Author = "文件作者信息"; //填加xls文件作者信息
                si.ApplicationName = "创建程序信息"; //填加xls文件创建程序信息
                si.LastAuthor = "最后保存者信息"; //填加xls文件最后保存者信息
                si.Comments = "作者信息"; //填加xls文件作者信息
                si.Title = "标题信息"; //填加xls文件标题信息
                si.Subject = "主题信息"; //填加文件主题信息
                si.CreateDateTime = DateTime.Now;
                workbook.SummaryInformation = si;
            }

            #endregion 右击文件 属性信息

            var dateStyle = (HSSFCellStyle) workbook.CreateCellStyle();
            var format = (HSSFDataFormat) workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

            //取得列宽
            var arrColWidth = new int[dtSource.Columns.Count];
            foreach (DataColumn item in dtSource.Columns)
                arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName).Length;
            for (var i = 0; i < dtSource.Rows.Count; i++)
                for (var j = 0; j < dtSource.Columns.Count; j++)
                {
                    var intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
                    if (intTemp > arrColWidth[j])
                        arrColWidth[j] = intTemp;
                }
            var rowIndex = 0;
            foreach (DataRow row in dtSource.Rows)
            {
                #region 新建表，填充表头，填充列头，样式

                if ((rowIndex == 65535) || (rowIndex == 0))
                {
                    if (rowIndex != 0)
                        sheet = (HSSFSheet) workbook.CreateSheet();

                    #region 表头及样式

                    {
                        var headerRow = (HSSFRow) sheet.CreateRow(0);
                        headerRow.HeightInPoints = 25;
                        headerRow.CreateCell(0).SetCellValue(strHeaderText);

                        var headStyle = (HSSFCellStyle) workbook.CreateCellStyle();
                        //headStyle.Alignment = CellHorizontalAlignment.CENTER;
                        var font = (HSSFFont) workbook.CreateFont();
                        font.FontHeightInPoints = 20;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);
                        headerRow.GetCell(0).CellStyle = headStyle;
                        sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, dtSource.Columns.Count - 1));
                        //headerRow.Dispose();
                    }

                    #endregion 表头及样式

                    #region 列头及样式

                    {
                        var headerRow = (HSSFRow) sheet.CreateRow(1);
                        var headStyle = (HSSFCellStyle) workbook.CreateCellStyle();

                        var font = (HSSFFont) workbook.CreateFont();
                        font.FontHeightInPoints = 10;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);
                        foreach (DataColumn column in dtSource.Columns)
                        {
                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                            headerRow.GetCell(column.Ordinal).CellStyle = headStyle;

                            //设置列宽
                            sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 3)*256);
                        }
                        //headerRow.Dispose();
                    }

                    #endregion 列头及样式

                    rowIndex = 2;
                }

                #endregion 新建表，填充表头，填充列头，样式

                #region 填充内容

                var dataRow = sheet.CreateRow(rowIndex);
                foreach (DataColumn column in dtSource.Columns)
                {
                    var newCell = (HSSFCell) dataRow.CreateCell(column.Ordinal);

                    var drValue = row[column].ToString();

                    switch (column.DataType.ToString())
                    {
                        case "System.String": //字符串类型
                            newCell.SetCellValue(drValue);
                            break;

                        case "System.DateTime": //日期类型
                            DateTime dateV;
                            DateTime.TryParse(drValue, out dateV);
                            newCell.SetCellValue(dateV);

                            newCell.CellStyle = dateStyle; //格式化显示
                            break;

                        case "System.Boolean": //布尔型
                            var boolV = false;
                            bool.TryParse(drValue, out boolV);
                            newCell.SetCellValue(boolV);
                            break;

                        case "System.Int16": //整型
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            var intV = 0;
                            int.TryParse(drValue, out intV);
                            newCell.SetCellValue(intV);
                            break;

                        case "System.Decimal": //浮点型
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(drValue, out doubV);
                            newCell.SetCellValue(doubV);
                            break;

                        case "System.DBNull": //空值处理
                            newCell.SetCellValue("");
                            break;

                        default:
                            newCell.SetCellValue("");
                            break;
                    }
                }

                #endregion 填充内容

                rowIndex++;
            }
            var ms = new MemoryStream();
            //using (var ms = new MemoryStream())
            //{
            workbook.Write(ms);
            ms.Flush();
            ms.Position = 0;

            //sheet.Dispose();
            //workbook.Dispose();//一般只用写这一个就OK了，他会遍历并释放所有资源，但当前版本有问题所以只释放sheet
            return ms;
            //}
        }

        /// <summary>
        ///     根据 ViewModel 生成 DataTable
        /// </summary>
        /// <param name="boVMCollection"></param>
        /// <returns></returns>
        public static DataTable GetDataTable<T>(List<T> boVMCollection) where T : class
        {
            var dt = new DataTable();

            var boType = typeof(T);

            #region 表头数据规格

            var properties = boType.GetProperties();
            var listColumnHeaderItems = new List<ListColumnHeader>();
            foreach (var property in properties)
            {
                #region ViewModel定义的的列表项

                var propertyAttribute =
                    property.GetCustomAttribute(typeof(ListItemSpecification), false) as ListItemSpecification;
                if (propertyAttribute != null)
                {
                    var ct = new ListColumnHeader
                    {
                        Title = propertyAttribute.ListName,
                        Width = propertyAttribute.Width.ToString(),
                        OrderSort = propertyAttribute.OrderSort,
                        UseSortIndicator = propertyAttribute.UseSortIndicator,
                        PropertyName = property.Name
                    };
                    listColumnHeaderItems.Add(ct);
                }

                #endregion ViewModel定义的的列表项
            }

            #endregion 表头数据规格

            #region 表头数据

            foreach (var colItem in listColumnHeaderItems.OrderBy(n => n.OrderSort))
                dt.Columns.Add(colItem.Title, Type.GetType("System.String"));

            #endregion 表头数据

            #region 表体数据

            foreach (var boVMItem in boVMCollection)
            {
                var dr = dt.NewRow();
                foreach (var colItem in listColumnHeaderItems.OrderBy(n => n.OrderSort))
                {
                    var property = properties.FirstOrDefault(x => x.Name == colItem.PropertyName);
                    var propertyValue = property.GetValue(boVMItem);

                    dr[colItem.Title] = propertyValue;
                }
                dt.Rows.Add(dr);
            }

            #endregion 表体数据

            return dt;
        }
    }
}