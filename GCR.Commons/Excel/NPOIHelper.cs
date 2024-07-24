using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NPOI;
using NPOI.HPSF;
using NPOI.HSSF;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.Util;
using NPOI.POIFS;
using NPOI.Util;
using NPOI.HSSF.Util;
using NPOI.XSSF.UserModel;
using System.IO;
using System.Web;
using System.Reflection;
using System.Collections;

namespace GCR.Commons
{
    /// <summary>
    /// Excel文件帮助类
    /// </summary>
    public static class NPOIHelper
    {
        #region 由DataSet、DataTable导出Excel

        /// <summary>
        /// 由DataSet导出Excel,被外界调用的方法，Web导出excel用
        /// </summary>
        /// <param name="sourceDs">要导出数据的DataSet</param>
        /// <param name="fileName">指定Excel工作表名称</param>
        /// <param name="sheetName">指定Excel工作表中sheet的名称,多个sheet用;号分隔</param>
        /// <param name="strType">strType=0:普通格式 1有格式化的形式</param>
        /// <returns>Excel工作表</returns>
        public static byte[] ExportDataSetToExcel(DataSet sourceDs, string fileName, string sheetName, string strType)
        {
            MemoryStream ms = null;
            if (strType == "0")
            {
                ms = ExportDataSetToBasicExcel(sourceDs, sheetName) as MemoryStream;
            }
            else
            {
                ms = ExportDataSetToFormatExcel(sourceDs, sheetName) as MemoryStream;
            }
            //HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, Encoding.UTF8) + ".xls");
            //HttpContext.Current.Response.BinaryWrite(ms.ToArray());
            //HttpContext.Current.Response.End();
            byte[] buffer = null;
            buffer = ms.GetBuffer();
            ms.Close();
            ms = null;
            return buffer;
        }

        /// <summary>
        /// 由DataSet导出Excel,被外界调用的方法，WinForm导出Excel用
        /// </summary>
        /// <param name="sourceDs">要导出数据的DataTable</param>
        /// <param name="savePath">导出的文件的存放地址</param>
        /// <param name="fileName">指定Excel工作表名称</param>
        /// <param name="sheetName">指定Excel工作表中sheet的名称,多个sheet用;号分隔</param>
        ///  <param name="strType">strType=0:普通格式 1有格式化的形式</param>
        /// <returns>Excel工作表</returns>
        public static void ExportDataSetToExcel(DataSet sourceDs, string savePath, string fileName, string sheetName, string strType)
        {
            MemoryStream ms = null;
            if (strType == "0")
            {
                ms = ExportDataSetToBasicExcel(sourceDs, sheetName) as MemoryStream;
            }
            else
            {
                ms = ExportDataSetToFormatExcel(sourceDs, sheetName) as MemoryStream;
            }

            using (FileStream fs = new FileStream(savePath, FileMode.Create, FileAccess.Write))
            {
                byte[] data = ms.ToArray();
                fs.Write(data, 0, data.Length);
                fs.Flush();
            }
        }

        /// <summary>
        /// 由DataSet导出Excel(基本形式)
        /// </summary>
        /// <param name="sourceTable">要导出数据的DataTable</param>
        /// <param name="sheetName">工作表名称</param>
        /// <returns>Excel工作表</returns>
        private static Stream ExportDataSetToBasicExcel(DataSet sourceDs, string sheetName)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            MemoryStream ms = new MemoryStream();
            string[] sheetNames = sheetName.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries); //分割符
            for (int i = 0; i < sheetNames.Length; i++)
            {
                HSSFSheet sheet = (HSSFSheet)workbook.CreateSheet(sheetNames[i]);

                // handling value.
                int rowIndex = 0;
                int sheetnum = 1;
                foreach (DataRow row in sourceDs.Tables[i].Rows)
                {
                    #region 创建表头

                    if (rowIndex == 65535 || rowIndex == 0)
                    {
                        if (rowIndex != 0)
                        {
                            sheetnum++;
                            sheet = (HSSFSheet)workbook.CreateSheet(sheetNames[i] + "-" + sheetnum.ToString());
                        }
                        var headerRow = sheet.CreateRow(0);
                        var headStyle = workbook.CreateCellStyle();
                        headStyle.Alignment = HorizontalAlignment.Center;
                        var font = workbook.CreateFont();
                        font.FontHeightInPoints = 10;
                        font.IsBold = true;
                        headStyle.SetFont(font);
                        foreach (DataColumn column in sourceDs.Tables[i].Columns)
                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                        rowIndex = 1;
                    }

                    #endregion 创建表头

                    #region 创建内容

                    HSSFRow dataRow = (HSSFRow)sheet.CreateRow(rowIndex);
                    foreach (DataColumn column in sourceDs.Tables[i].Columns)
                    {
                        dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
                    }
                    rowIndex++;

                    #endregion 创建内容
                }
            }
            workbook.Write(ms);
            ms.Flush();
            ms.Position = 0;
            workbook = null;
            return ms;
        }

        /// <summary>
        /// 由DataSet导出Excel(带有格式)
        /// </summary>
        /// <param name="sourceTable">要导出数据的DataTable</param>
        /// <param name="sheetName">工作表名称</param>
        /// <returns>Excel工作表</returns>
        private static Stream ExportDataSetToFormatExcel(DataSet sourceDs, string sheetName)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            MemoryStream ms = new MemoryStream();
            string[] sheetNames = sheetName.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries); //分割符
            for (int i = 0; i < sheetNames.Length; i++)
            {
                HSSFSheet sheet = (HSSFSheet)workbook.CreateSheet(sheetNames[i]);

                var dateStyle = workbook.CreateCellStyle();
                var format = workbook.CreateDataFormat();
                dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

                //取得列宽
                int[] arrColWidth = new int[sourceDs.Tables[i].Columns.Count];
                foreach (DataColumn item in sourceDs.Tables[i].Columns)
                {
                    arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
                }
                for (int k = 0; k < sourceDs.Tables[i].Rows.Count; k++)
                {
                    for (int j = 0; j < sourceDs.Tables[i].Columns.Count; j++)
                    {
                        int intTemp = Encoding.GetEncoding(936).GetBytes(sourceDs.Tables[i].Rows[k][j].ToString()).Length;
                        if (intTemp > arrColWidth[j])
                        {
                            arrColWidth[j] = intTemp;
                        }
                    }
                }

                int rowIndex = 0;
                int sheetnum = 1;
                foreach (DataRow row in sourceDs.Tables[i].Rows)
                {
                    #region 创建表头

                    if (rowIndex == 65535 || rowIndex == 0)
                    {
                        if (rowIndex != 0)
                        {
                            sheetnum++;
                            sheet = (HSSFSheet)workbook.CreateSheet(sheetNames[i] + "-" + sheetnum.ToString());
                        }
                        var headerRow = sheet.CreateRow(0);
                        var headStyle = workbook.CreateCellStyle();
                        headStyle.Alignment = HorizontalAlignment.Center;
                        var font = workbook.CreateFont();
                        font.FontHeightInPoints = 10;
                        font.IsBold = true;
                        headStyle.SetFont(font);

                        //设置边框
                        headStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                        headStyle.BottomBorderColor = HSSFColor.Black.Index;
                        headStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                        headStyle.LeftBorderColor = HSSFColor.Black.Index;
                        headStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                        headStyle.RightBorderColor = HSSFColor.Black.Index;
                        headStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                        headStyle.TopBorderColor = HSSFColor.Black.Index;
                        //设置背景色
                        //headStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LIME.index;
                        //headStyle.FillPattern = NPOI.SS.UserModel.FillPatternType.BIG_SPOTS;
                        //headStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.LIGHT_GREEN.index;

                        foreach (DataColumn column in sourceDs.Tables[i].Columns)
                        {
                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                            headerRow.GetCell(column.Ordinal).CellStyle = headStyle;
                            //设置列宽
                            //sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
                            int colWidth = (arrColWidth[column.Ordinal] + 1) * 256;
                            if (colWidth < 255 * 256)
                            {
                                sheet.SetColumnWidth(column.Ordinal, colWidth < 3000 ? 3000 : colWidth);
                            }
                            else
                            {
                                sheet.SetColumnWidth(column.Ordinal, 6000);
                            }
                        }
                        rowIndex = 1;
                    }

                    #endregion 创建表头

                    #region 创建内容

                    var dataRow = sheet.CreateRow(rowIndex);
                    foreach (DataColumn column in sourceDs.Tables[i].Columns)
                    {
                        var newCell = dataRow.CreateCell(column.Ordinal);

                        string drValue = row[column].ToString();

                        switch (column.DataType.ToString())
                        {
                            case "System.String"://字符串类型
                                newCell.SetCellValue(drValue);
                                break;

                            case "System.DateTime"://日期类型
                            case "MySql.Data.Types.MySqlDateTime": //MySql类型
                                if (drValue == "0000/0/0 0:00:00" || String.IsNullOrEmpty(drValue))
                                {
                                    //当时间为空，防止生成的execl 中是一串“#######”号，所有赋值为空字符串
                                    newCell.SetCellValue("");
                                }
                                else
                                {
                                    DateTime dateV;
                                    DateTime.TryParse(drValue, out dateV);
                                    newCell.SetCellValue(dateV);
                                    newCell.CellStyle = dateStyle;//格式化显示
                                }
                                break;

                            case "System.Boolean"://布尔型
                                bool boolV = false;
                                bool.TryParse(drValue, out boolV);
                                newCell.SetCellValue(boolV);
                                break;

                            case "System.Int16"://整型
                            case "System.Int32":
                            case "System.Int64":
                            case "System.Byte":
                                int intV = 0;
                                int.TryParse(drValue, out intV);
                                newCell.SetCellValue(intV);
                                break;

                            case "System.Decimal"://浮点型
                            case "System.Double":
                                double doubV = 0;
                                double.TryParse(drValue, out doubV);
                                newCell.SetCellValue(doubV);
                                break;

                            case "System.DBNull"://空值处理
                                newCell.SetCellValue("");
                                break;

                            default:
                                newCell.SetCellValue("");
                                break;
                        }
                    }

                    #endregion 创建内容

                    rowIndex++;
                }
                //设置首行首列冻结
                //第一个参数表示要冻结的列数
                //第二个参数表示要冻结的行数
                //第三个参数表示右边区域可见的首列序号，从1开始计算
                //第四个参数表示下边区域可见的首行序号，也是从1开始计算
                //sheet.CreateFreezePane(1, 1, 0,0);
            }
            workbook.Write(ms);
            ms.Flush();
            ms.Position = 0;
            workbook = null;
            return ms;
        }

        /// <summary>
        /// 由DataTable导出Excel,基本方法
        /// </summary>
        /// <param name="sourceTable">要导出数据的DataTable</param>
        /// <param name="sheetName">要导出数据的DataTable</param>
        /// <returns>Excel工作表</returns>
        private static Stream ExportDataTableToBasicExcel(DataTable sourceTable, string sheetName)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            MemoryStream ms = new MemoryStream();
            var sheet = workbook.CreateSheet(sheetName);

            int rowIndex = 0;
            int sheetnum = 1;
            foreach (DataRow row in sourceTable.Rows)
            {
                #region 创建表头

                if (rowIndex == 65535 || rowIndex == 0)
                {
                    if (rowIndex != 0)
                    {
                        sheetnum++;
                        sheet = workbook.CreateSheet(sheetName + "-" + sheetnum.ToString());
                    }
                    var headerRow = sheet.CreateRow(0);
                    var headStyle = workbook.CreateCellStyle();
                    headStyle.Alignment = HorizontalAlignment.Center;
                    var font = workbook.CreateFont();
                    font.FontHeightInPoints = 10;
                    font.IsBold = true;
                    headStyle.SetFont(font);
                    foreach (DataColumn column in sourceTable.Columns)
                        headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                    rowIndex = 1;
                }

                #endregion 创建表头

                HSSFRow dataRow = (HSSFRow)sheet.CreateRow(rowIndex);
                foreach (DataColumn column in sourceTable.Columns)
                {
                    dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
                }
                rowIndex++;
            }
            workbook.Write(ms);
            ms.Flush();
            ms.Position = 0;
            sheet = null;
            workbook = null;
            return ms;
        }

        /// <summary>
        /// 带格式化的
        /// </summary>
        /// <param name="dtSource">数据源</param>
        /// <param name="sheetName">创建的Sheet名称</param>
        /// <returns></returns>
        private static Stream ExportDataTableToFormatExcel(DataTable dtSource, string sheetName)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            var sheet = workbook.CreateSheet(sheetName);

            var dateStyle = workbook.CreateCellStyle();
            var format = workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

            //取得列宽
            int[] arrColWidth = new int[dtSource.Columns.Count];
            foreach (DataColumn item in dtSource.Columns)
            {
                arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
            }
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                for (int j = 0; j < dtSource.Columns.Count; j++)
                {
                    int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
                    if (intTemp > arrColWidth[j])
                    {
                        arrColWidth[j] = intTemp;
                    }
                }
            }

            int rowIndex = 0;

            int sheetnum = 1;

            foreach (DataRow row in dtSource.Rows)
            {
                #region 新建表，填充表头，填充列头，样式

                if (rowIndex == 65535 || rowIndex == 0)
                {
                    if (rowIndex != 0)
                    {
                        sheetnum++;
                        sheet = workbook.CreateSheet(sheetName + "-" + sheetnum.ToString());
                    }

                    #region 列头及样式

                    {
                        var headerRow = sheet.CreateRow(0);
                        var headStyle = workbook.CreateCellStyle();
                        headStyle.Alignment = HorizontalAlignment.Center;
                        var font = workbook.CreateFont();
                        font.FontHeightInPoints = 10;
                        font.IsBold = true;
                        headStyle.SetFont(font);
                        //设置背景色
                        headStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.White.Index;
                        headStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.Red.Index;

                        foreach (DataColumn column in dtSource.Columns)
                        {
                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                            headerRow.GetCell(column.Ordinal).CellStyle = headStyle;
                            //设置列宽
                            //sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
                            int colWidth = (arrColWidth[column.Ordinal] + 1) * 256;
                            if (colWidth < 255 * 256)
                            {
                                sheet.SetColumnWidth(column.Ordinal, colWidth < 3000 ? 3000 : colWidth);
                            }
                            else
                            {
                                sheet.SetColumnWidth(column.Ordinal, 6000);
                            }
                        }
                    }

                    #endregion 列头及样式

                    rowIndex = 1;
                }

                #endregion 新建表，填充表头，填充列头，样式

                #region 填充内容

                var dataRow = sheet.CreateRow(rowIndex);
                foreach (DataColumn column in dtSource.Columns)
                {
                    var newCell = dataRow.CreateCell(column.Ordinal);

                    string drValue = row[column].ToString();

                    switch (column.DataType.ToString())
                    {
                        case "System.String"://字符串类型
                            newCell.SetCellValue(drValue);
                            break;

                        case "System.DateTime"://日期类型
                        case "MySql.Data.Types.MySqlDateTime": //MySql类型
                            if (drValue == "0000/0/0 0:00:00" || String.IsNullOrEmpty(drValue))
                            {
                                //当时间为空，防止生成的execl 中是一串“#######”号，所有赋值为空字符串
                                newCell.SetCellValue("");
                            }
                            else
                            {
                                DateTime dateV;
                                DateTime.TryParse(drValue, out dateV);
                                newCell.SetCellValue(dateV);

                                newCell.CellStyle = dateStyle;//格式化显示
                            }
                            break;

                        case "System.Boolean"://布尔型
                            bool boolV = false;
                            bool.TryParse(drValue, out boolV);
                            newCell.SetCellValue(boolV);
                            break;

                        case "System.Int16"://整型
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            int intV = 0;
                            int.TryParse(drValue, out intV);
                            newCell.SetCellValue(intV);
                            break;

                        case "System.Decimal"://浮点型
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(drValue, out doubV);
                            newCell.SetCellValue(doubV);
                            break;

                        case "System.DBNull"://空值处理
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
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
                return ms;
            }
        }

        /// <summary>
        /// 由DataTable导出Excel,Web导出用
        /// </summary>
        /// <param name="sourceTable">要导出数据的DataTable</param>
        /// <param name="fileName">指定Excel工作表名称</param>
        /// <param name="sheetName">指定Sheet名称</param>
        /// <param name="strType">strType=0:基本的，1：带有格式的</param>
        /// <returns>Excel工作表</returns>
        public static byte[] ExportDataTableToExcel(DataTable sourceTable, string fileName, string sheetName, string strType)
        {
            MemoryStream ms = null;
            if (strType == "0")
            {
                ms = ExportDataTableToBasicExcel(sourceTable, sheetName) as MemoryStream;
            }
            else
            {
                ms = ExportDataTableToFormatExcel(sourceTable, sheetName) as MemoryStream;
            }

            //HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, Encoding.UTF8) + ".xls");
            //HttpContext.Current.Response.BinaryWrite(ms.ToArray());
            //HttpContext.Current.Response.End();
            byte[] buffer = null;
            buffer = ms.GetBuffer();
            ms.Close();
            ms = null;
            return buffer;
        }

        /// <summary>
        /// 由DataTable导出Excel,WinForm导出用
        /// </summary>
        /// <param name="sourceTable">要导出数据的DataTable</param>
        /// <param name="savePath">导出Excel的存放路径</param>
        /// <param name="fileName">指定Excel工作表名称</param>
        /// <param name="sheetName">指定Sheet名称</param>
        /// <param name="strType">strType=0:基本的，1：带有格式的</param>
        /// <returns>Excel工作表</returns>
        public static void ExportDataTableToExcel(DataTable sourceTable, string savePath, string fileName, string sheetName, string strType)
        {
            MemoryStream ms = null;
            if (strType == "0")
            {
                ms = ExportDataTableToBasicExcel(sourceTable, sheetName) as MemoryStream;
            }
            else
            {
                ms = ExportDataTableToFormatExcel(sourceTable, sheetName) as MemoryStream;
            }

            using (FileStream fs = new FileStream(savePath, FileMode.Create, FileAccess.Write))
            {
                byte[] data = ms.ToArray();
                fs.Write(data, 0, data.Length);
                fs.Flush();
            }
        }

        /// <summary>
        /// 由DataTable导出Excel(适应于基本的模版导出，且不超过65535条)
        /// </summary>
        /// <param name="sourceTable">要导出数据的DataTable</param>
        /// <param name="modelpath">模版文件实体路径</param>
        /// <param name="modelName">模版文件名称</param>
        /// <param name="fileName">指定Excel工作表名称</param>
        /// <param name="sheetName">作为模型的Excel</param>
        /// <param name="rowindex">从第几行开始写入数据(此为行索引，若为1则从第2行开始写入数据)</param>
        /// <returns>Excel工作表</returns>
        public static byte[] ExportDataTableToExcelModel(DataTable sourceTable, string modelpath, string modelName, string fileName, string sheetName, int rowIndex)
        {
            int colIndex = 0;
            FileStream file = new FileStream(modelpath + "/" + modelName, FileMode.Open, FileAccess.Read);//读入excel模板
            HSSFWorkbook hssfworkbook = new HSSFWorkbook(file);
            HSSFSheet sheet1 = (HSSFSheet)hssfworkbook.GetSheet(sheetName);
            if (sourceTable.Rows.Count + rowIndex > 65535)
            {
                throw new ArgumentException("数据太多，系统尚不支持，请缩小查询范围!");
            }

            foreach (DataRow row in sourceTable.Rows)
            {   //双循环写入sourceTable中的数据
                colIndex = 0;
                HSSFRow xlsrow = (HSSFRow)sheet1.CreateRow(rowIndex);
                foreach (DataColumn col in sourceTable.Columns)
                {
                    xlsrow.CreateCell(colIndex).SetCellValue(row[col.ColumnName].ToString());
                    colIndex++;
                }
                rowIndex++;
            }
            sheet1.ForceFormulaRecalculation = true;

            //CS项目适用胡方法
            //FileStream fileS = new FileStream(modelpath + fileName + ".xls", FileMode.Create);//保存
            //hssfworkbook.Write(fileS);
            //fileS.Close();
            MemoryStream ms = new MemoryStream();
            hssfworkbook.Write(ms);

            //HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, Encoding.UTF8) + ".xls");
            //HttpContext.Current.Response.BinaryWrite(ms.ToArray());
            //HttpContext.Current.Response.End();
            byte[] buffer = null;
            buffer = ms.GetBuffer();
            ms.Close();
            ms = null;
            return buffer;
        }

        #endregion 由DataSet、DataTable导出Excel

        #region 从Excel中读数据到DataTable

        /// <summary>
        /// 从Excel中获取数据到DataTable
        /// </summary>
        /// <param name="strFileName">Excel文件全路径(服务器路径)</param>
        /// <param name="extension">Excel文件的扩展名</param>
        /// <param name="SheetName">要获取数据的工作表名称</param>
        /// <param name="HeaderRowIndex">工作表标题行所在行号(从0开始)</param>
        /// <returns></returns>
        public static DataTable RenderDataTableFromExcel(string strFileName, string extension, string SheetName, int HeaderRowIndex)
        {
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = null;
                if (extension.Equals(".xls") || extension.Equals(".XLS"))
                {
                    workbook = new HSSFWorkbook(file);
                }
                else
                {
                    workbook = new XSSFWorkbook(file);
                }
                return RenderDataTableFromExcel(workbook, SheetName, HeaderRowIndex);
            }
        }

        /// <summary>
        /// 从Excel中获取数据到DataTable
        /// </summary>
        /// <param name="strFileName">Excel文件全路径(服务器路径)</param>
        /// <param name="extension">Excel文件的扩展名</param>
        /// <param name="SheetIndex">要获取数据的工作表序号(从0开始)</param>
        /// <param name="HeaderRowIndex">工作表标题行所在行号(从0开始)</param>
        /// <returns></returns>
        public static DataTable RenderDataTableFromExcel(string strFileName, string extension, int SheetIndex, int HeaderRowIndex)
        {
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = null;
                if (extension.Equals(".xls") || extension.Equals(".XLS"))
                {
                    workbook = new HSSFWorkbook(file);
                }
                else
                {
                    workbook = new XSSFWorkbook(file);
                }

                string SheetName = workbook.GetSheetName(SheetIndex);
                return RenderDataTableFromExcel(workbook, SheetName, HeaderRowIndex);
            }
        }

        /// <summary>
        /// 从Excel中获取数据到DataTable
        /// </summary>
        /// <param name="ExcelFileStream">Excel文件流</param>
        /// <param name="SheetName">要获取数据的工作表名称</param>
        /// <param name="HeaderRowIndex">工作表标题行所在行号(从0开始)</param>
        /// <returns></returns>
        public static DataTable RenderDataTableFromExcel(Stream ExcelFileStream, string SheetName, int HeaderRowIndex)
        {
            IWorkbook workbook = new HSSFWorkbook(ExcelFileStream);
            ExcelFileStream.Close();
            return RenderDataTableFromExcel(workbook, SheetName, HeaderRowIndex);
        }

        /// <summary>
        /// 从Excel中获取数据到DataTable
        /// </summary>
        /// <param name="ExcelFileStream">Excel文件流</param>
        /// <param name="SheetIndex">要获取数据的工作表序号(从0开始)</param>
        /// <param name="HeaderRowIndex">工作表标题行所在行号(从0开始)</param>
        /// <returns></returns>
        public static DataTable RenderDataTableFromExcel(Stream ExcelFileStream, int SheetIndex, int HeaderRowIndex)
        {
            IWorkbook workbook = new HSSFWorkbook(ExcelFileStream);
            ExcelFileStream.Close();
            string SheetName = workbook.GetSheetName(SheetIndex);
            return RenderDataTableFromExcel(workbook, SheetName, HeaderRowIndex);
        }

        /// <summary>
        /// 从Excel中获取数据到DataTable
        /// </summary>
        /// <param name="workbook">要处理的工作薄</param>
        /// <param name="SheetName">要获取数据的工作表名称</param>
        /// <param name="HeaderRowIndex">工作表标题行所在行号(从0开始)</param>
        /// <returns></returns>
        public static DataTable RenderDataTableFromExcel(IWorkbook workbook, string SheetName, int HeaderRowIndex)
        {
            ISheet sheet = workbook.GetSheet(SheetName);
            DataTable table = new DataTable();
            table.TableName = SheetName;
            try
            {
                IRow headerRow = sheet.GetRow(HeaderRowIndex);
                int cellCount = headerRow.LastCellNum;

                bool isHeadNum = false;
                List<string> listcolumn = new List<string>();
                for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                {
                    string name = headerRow.GetCell(i)?.StringCellValue ?? "";
                    if (listcolumn.Contains(name)) { isHeadNum = true; break; }
                    listcolumn.Add(name);
                }

                for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                {
                    DataColumn column = null;

                    try
                    {
                        column = new DataColumn(isHeadNum ? headerRow.GetCell(i)?.StringCellValue + "_" + i.ToString() : headerRow.GetCell(i)?.StringCellValue);
                    }
                    catch
                    {
                        column = new DataColumn();
                    }

                    table.Columns.Add(column);
                }

                int rowCount = sheet.LastRowNum;

                #region 循环各行各列,写入数据到DataTable

                for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                {
                    try
                    {
                        IRow row = sheet.GetRow(i);
                        DataRow dataRow = table.NewRow();
                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {
                            ICell cell = row.GetCell(j);
                            if (cell == null)
                            {
                                dataRow[j] = null;
                            }
                            else
                            {
                                switch (cell.CellType)
                                {
                                    case CellType.Blank:
                                        dataRow[j] = null;
                                        break;

                                    case CellType.Boolean:
                                        dataRow[j] = cell.BooleanCellValue;
                                        break;

                                    case CellType.Numeric:
                                        dataRow[j] = cell.ToString();
                                        break;

                                    case CellType.String:
                                        dataRow[j] = cell.StringCellValue.Replace(",", "，").Replace("'", "‘");
                                        break;

                                    case CellType.Error:
                                        dataRow[j] = cell.ErrorCellValue;
                                        break;

                                    case CellType.Formula:
                                    default:
                                        dataRow[j] = "=" + cell.CellFormula;
                                        break;
                                }
                            }
                        }

                        table.Rows.Add(dataRow);
                    }
                    catch { }
                    //dataRow[j] = row.GetCell(j).ToString();
                }

                #endregion 循环各行各列,写入数据到DataTable
            }
            catch (Exception ex)
            {
                table.Clear();
                table.Columns.Clear();
                table.Columns.Add("出错了");
                DataRow dr = table.NewRow();
                dr[0] = ex.Message;
                table.Rows.Add(dr);
                return table;
            }
            finally
            {
                //sheet.Dispose();
                workbook = null;
                sheet = null;
            }

            #region 清除最后的空行

            for (int i = table.Rows.Count - 1; i > 0; i--)
            {
                bool isnull = true;
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    if (table.Rows[i][j] != null)
                    {
                        if (table.Rows[i][j].ToString() != "")
                        {
                            isnull = false;
                            break;
                        }
                    }
                }
                if (isnull)
                {
                    table.Rows[i].Delete();
                }
            }

            #endregion 清除最后的空行

            return table;
        }

        #endregion 从Excel中读数据到DataTable

        /// <summary>
        /// 通用方法替换规律DataTable表头
        /// </summary>
        /// <param name="dt">原始Table</param>
        /// <param name="newclo">列对照{old:new}</param>
        /// <returns></returns>
        public static DataTable ReplaceColumns(DataTable dt, Object newclo)
        {
            DataTable newdt = new DataTable();
            newdt = dt.DefaultView.ToTable(false,
                newclo.GetType().GetProperties().Select(item => item.Name).ToArray()
            );
            foreach (PropertyInfo p in newclo.GetType().GetProperties())
            {
                //p.Name, p.GetValue(newclo)
                newdt.Columns[p.Name].ColumnName = p.GetValue(newclo).ToString();
            }
            return newdt;
        }

        /// <summary>
        /// list转datable
        /// </summary>
        /// <typeparam name="T">当前类型</typeparam>
        /// <param name="source">数据</param>
        /// <param name="newclo">列</param>
        /// <returns></returns>
        public static DataTable ReplaceColumns<T>(IEnumerable<T> source, Object newclo)
        {
            var dt = ToDataTable<T>(source, newclo);
            return ReplaceColumns(dt, newclo);
        }

        /// <summary>
        /// 实体转Table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(IEnumerable<T> collection, Object newclo)
        {
            var props = typeof(T).GetProperties().Where(m => newclo.GetType().GetProperties().Any(c => c.Name == m.Name)).ToList().Distinct();
            var dt = new DataTable();
            dt.Columns.AddRange(props.Select(p => new DataColumn(p.Name)).ToArray());
            if (collection.Count() > 0)
            {
                for (int i = 0; i < collection.Count(); i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in props)
                    {
                        object obj = pi.GetValue(collection.ElementAt(i), null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    dt.LoadDataRow(array, true);
                }
            }
            return dt;
        }
    }
}
