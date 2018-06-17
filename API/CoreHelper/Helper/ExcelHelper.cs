using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;

namespace CoreHelper
{
    /// <summary>
    /// Excel帮助类，导出Excel，导入Excel
    /// 使用前请先安装Install-Package EPPlus
    /// </summary>
    public class ExcelHelper
    {
        #region 使用EPPlus导出Excel(xlsx格式)
        /// <summary>
        /// 使用EPPlus导出Excel(xlsx格式)
        /// </summary>
        /// <typeparam name="T">泛型对象</typeparam>
        /// <param name="list">泛型List对象</param>
        /// <param name="strFileName">xlsx文件名(不含后缀名)</param>
        /// <returns></returns>
        public static bool SaveToExcel<T>(List<T> list, string strFileName)
        {
            System.Data.DataTable dt = ListTableHelper.ToDataTable<T>(list);
            return SaveToExcel(dt, strFileName);
        }
        #endregion

        #region 使用EPPlus导出Excel(xlsx格式)
        /// <summary>
        /// 使用EPPlus导出Excel(xlsx格式)
        /// </summary>
        /// <param name="sourceTable">数据源</param>
        /// <param name="strFileName">xlsx文件名(不含后缀名)</param>
        /// <returns></returns>
        public static bool SaveToExcel(System.Data.DataTable sourceTable, string strFileName)
        {
            bool succeed = false;
            using (ExcelPackage pck = new ExcelPackage())
            {
                //创建工作表
                string sheetName = string.IsNullOrEmpty(sourceTable.TableName) ? "Sheet1" : sourceTable.TableName;
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add(sheetName);

                //从单元格A1开始将数据表加载到工作表中。 打印行1上的列名称
                ws.Cells["A1"].LoadFromDataTable(sourceTable, true);

                //格式化行
                ExcelBorderStyle borderStyle = ExcelBorderStyle.Thin;
                Color borderColor = Color.FromArgb(155, 155, 155);

                using (ExcelRange rng = ws.Cells[1, 1, sourceTable.Rows.Count + 1, sourceTable.Columns.Count])
                {
                    rng.Style.Font.Name = "宋体";
                    rng.Style.Font.Size = 10;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;//将背景图案设置为实心
                    rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));

                    rng.Style.Border.Top.Style = borderStyle;
                    rng.Style.Border.Top.Color.SetColor(borderColor);

                    rng.Style.Border.Bottom.Style = borderStyle;
                    rng.Style.Border.Bottom.Color.SetColor(borderColor);

                    rng.Style.Border.Right.Style = borderStyle;
                    rng.Style.Border.Right.Color.SetColor(borderColor);
                }

                //格式化标题行
                using (ExcelRange rng = ws.Cells[1, 1, 1, sourceTable.Columns.Count])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(234, 241, 246));  //将颜色设置为深蓝色
                    rng.Style.Font.Color.SetColor(Color.FromArgb(51, 51, 51));
                }

                //把数据写回客户端
                System.Web.HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                System.Web.HttpContext.Current.Response.AddHeader("content-disposition", string.Format("attachment;  filename={0}.xlsx", HttpUtility.UrlEncode(strFileName, Encoding.UTF8)));
                System.Web.HttpContext.Current.Response.ContentEncoding = Encoding.UTF8;

                System.Web.HttpContext.Current.Response.BinaryWrite(pck.GetAsByteArray());
                System.Web.HttpContext.Current.Response.End();
                succeed = true;
            }
            return succeed;
        }
        #endregion

        #region 从Excel中加载数据（泛型）+IEnumerable<T> LoadFromExcel<T>(string FileName) where T : new()
        /// <summary>
        /// 从Excel中加载数据（泛型）
        /// </summary>
        /// <typeparam name="T">每行数据的类型</typeparam>
        /// <param name="FileName">Excel文件名</param>
        /// <returns>泛型列表</returns>
        public static IEnumerable<T> LoadFromExcel<T>(string FileName) where T : new()
        {
            FileInfo existingFile = new FileInfo(FileName);
            List<T> resultList = new List<T>();
            Dictionary<string, int> dictHeader = new Dictionary<string, int>();

            using (ExcelPackage package = new ExcelPackage(existingFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                int colStart = worksheet.Dimension.Start.Column;  //工作区开始列
                int colEnd = worksheet.Dimension.End.Column;       //工作区结束列
                int rowStart = worksheet.Dimension.Start.Row;       //工作区开始行号
                int rowEnd = worksheet.Dimension.End.Row;       //工作区结束行号

                //将每列标题添加到字典中
                for (int i = colStart; i <= colEnd; i++)
                {
                    dictHeader[worksheet.Cells[rowStart, i].Value.ToString()] = i;
                }

                List<PropertyInfo> propertyInfoList = new List<PropertyInfo>(typeof(T).GetProperties());

                for (int row = rowStart + 1; row < rowEnd; row++)
                {
                    T result = new T();

                    //为对象T的各属性赋值
                    foreach (PropertyInfo p in propertyInfoList)
                    {
                        try
                        {
                            ExcelRange cell = worksheet.Cells[row, dictHeader[p.Name]]; //与属性名对应的单元格

                            if (cell.Value == null)
                                continue;
                            switch (p.PropertyType.Name.ToLower())
                            {
                                case "string":
                                    p.SetValue(result, cell.GetValue<String>());
                                    break;
                                case "int16":
                                    p.SetValue(result, cell.GetValue<Int16>());
                                    break;
                                case "int32":
                                    p.SetValue(result, cell.GetValue<Int32>());
                                    break;
                                case "int64":
                                    p.SetValue(result, cell.GetValue<Int64>());
                                    break;
                                case "decimal":
                                    p.SetValue(result, cell.GetValue<Decimal>());
                                    break;
                                case "double":
                                    p.SetValue(result, cell.GetValue<Double>());
                                    break;
                                case "datetime":
                                    p.SetValue(result, cell.GetValue<DateTime>());
                                    break;
                                case "boolean":
                                    p.SetValue(result, cell.GetValue<Boolean>());
                                    break;
                                case "byte":
                                    p.SetValue(result, cell.GetValue<Byte>());
                                    break;
                                case "char":
                                    p.SetValue(result, cell.GetValue<Char>());
                                    break;
                                case "single":
                                    p.SetValue(result, cell.GetValue<Single>());
                                    break;
                                default:
                                    break;
                            }
                        }
                        catch (KeyNotFoundException ex)
                        { }
                    }
                    resultList.Add(result);
                }
            }
            return resultList;
        }
        #endregion
    }
}
