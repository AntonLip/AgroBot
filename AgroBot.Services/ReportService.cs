using AgroBot.Models.Interfaces.IService;
using AgroBot.Models.ModelsDB;
using AgroBot.Models.ModelsDto;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;

namespace AgroBot.Services
{
    public class ReportService : IReportService
    {
        private readonly IUserService _userService;
        public ReportService(IUserService userService)
        {
            _userService = userService;
        }

        public void RemoveReportFile(string filename)
        {
            File.Delete(filename);
        }
        public async Task<string> GetReportByLogistAsync(ApplicationUser logist, IList<Route> route)
        {
            string  name = logist.ChatId + "report.xls";
            try
            {
                using (Stream memoryStream = File.Create(name))
                {
                    using (SpreadsheetDocument document =
                            SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook, true))
                    {
                        WorkbookPart workbookPart = document.AddWorkbookPart();
                        workbookPart.Workbook = new Workbook();
                        WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();

                        FileVersion fv = new FileVersion();
                        fv.ApplicationName = "Microsoft Office Excel";
                        worksheetPart.Worksheet = new Worksheet(new SheetData());
                        worksheetPart = CreateColumns(worksheetPart);
                        Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                        Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Отчет о Логисте" };
                        sheets.Append(sheet);
                        SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                        Row rowFirst = new Row { RowIndex = 1 };
                        sheetData.Append(rowFirst);
                        InsertCell(rowFirst, 4, "Отчет по работе", CellValues.String, 5);
                        InsertCell(rowFirst, 5, logist.FirstName + " " + logist.LastName, CellValues.String, 5);

                        Row rowSecond = new Row { RowIndex = 2 };
                        InsertCell(rowSecond, 1, " ", CellValues.String, 5);
                        sheetData.Append(rowSecond);

                        Row row3 = new Row { RowIndex = 3 };
                        InsertCell(row3, 7, "Статистика", CellValues.String, 5);
                        sheetData.Append(row3);

                        Row row4 = new Row { RowIndex = 4 };
                        InsertCell(row4, 1, "Количество выполненных маршрутов", CellValues.String, 5);
                        InsertCell(row4, 3, route.Where(p => p.IsDeleted == true).Count().ToString(), CellValues.String, 5);
                        sheetData.Append(row4);

                        Row row5 = new Row { RowIndex = 5 };
                        InsertCell(row5, 1, "Общее количество маршрутов", CellValues.String, 5);
                        InsertCell(row5, 3, route.Count.ToString(), CellValues.String, 5);
                        sheetData.Append(row5);

                        Row row6 = new Row { RowIndex = 6 };
                        InsertCell(row6, 1, "Количество маршрутов в работе", CellValues.String, 5);
                        InsertCell(row6, 3, route.Where(p => p.IsDeleted == false).Count().ToString(), CellValues.String, 5);
                        sheetData.Append(row6);

                        Row row10 = new Row { RowIndex = 8 };
                        InsertCell(row10, 5, "Список маршрутов", CellValues.String, 5);
                        sheetData.Append(row10);

                        Row row11 = new Row { RowIndex = 9 };
                        InsertCell(row11, 1, "№п/п", CellValues.String, 5);
                        InsertCell(row11, 2, "Название маршрута", CellValues.String, 5);
                        InsertCell(row11, 3, "Название груза", CellValues.String, 5);
                        InsertCell(row11, 4, "Вес груза", CellValues.String, 5);
                        InsertCell(row11, 5, "Дата регистрации", CellValues.String, 5);
                        InsertCell(row11, 6, "Дата назначения водителя", CellValues.String, 5);
                        InsertCell(row11, 7, "Дата выполнения", CellValues.String, 5);
                        InsertCell(row11, 8, "Количество контрольных точек", CellValues.String, 5);
                        InsertCell(row11, 9, "Имя водителя", CellValues.String, 5);
                        sheetData.Append(row11);

                        uint idx = 11;
                        int counter = 1;
                        foreach (var item in route)
                        {

                            Row row = new Row { RowIndex = idx };
                            sheetData.Append(row);
                            InsertCell(row, 1, counter.ToString(), CellValues.String, 5);
                            InsertCell(row, 2, item.Name, CellValues.String, 5);
                            InsertCell(row, 3, item.Goods, CellValues.String, 5);
                            InsertCell(row, 4, item.Kilo.ToString(), CellValues.String, 5);
                            InsertCell(row, 5, item.CreatedDate.ToShortDateString().ToString(), CellValues.String, 5);
                            InsertCell(row, 6, item.AppointDate.ToShortDateString().ToString(), CellValues.String, 5);
                            if(item.IsDeleted)
                            {
                                InsertCell(row, 7, item.FullffilDate.ToShortDateString().ToString(), CellValues.String, 5);
                            }
                            else
                            {
                                InsertCell(row, 7, "Не выполнен", CellValues.String, 5);
                            }
                            InsertCell(row, 8, item.Points.Count.ToString(), CellValues.String, 5);
                            var driver = await _userService.GetByChatIdAsync(item.DriverChatId);
                            InsertCell(row, 9, driver.FirstName + " " + driver.LastName, CellValues.String, 5);

                            counter++;
                            idx++;
                        }
                    }

                }
                return name;
            }
            catch(Exception ex)
            {
                return "Exception" + " " + ex.Message;
            }            
        }
        public async Task<string> GetReportByDriverAsync(ApplicationUser driver, IList<Route> route)
        {
            string name = driver.ChatId + "report.xls";
            try
            {
                using (Stream memoryStream = File.Create(name))
                {
                    using (SpreadsheetDocument document =
                            SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook, true))
                    {
                        WorkbookPart workbookPart = document.AddWorkbookPart();
                        workbookPart.Workbook = new Workbook();
                        WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();

                        FileVersion fv = new FileVersion();
                        fv.ApplicationName = "Microsoft Office Excel";
                        worksheetPart.Worksheet = new Worksheet(new SheetData());
                        worksheetPart = CreateColumns(worksheetPart);
                        Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                        Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Отчет о водителя" };
                        sheets.Append(sheet);
                        SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                        Row rowFirst = new Row { RowIndex = 1 };
                        sheetData.Append(rowFirst);
                        InsertCell(rowFirst, 4, "Отчет по работе", CellValues.String, 5);
                        InsertCell(rowFirst, 5, driver.FirstName + " " + driver.LastName, CellValues.String, 5);

                        Row rowSecond = new Row { RowIndex = 2 };
                        InsertCell(rowSecond, 1, " ", CellValues.String, 5);
                        sheetData.Append(rowSecond);

                        Row row3 = new Row { RowIndex = 3 };
                        InsertCell(row3, 7, "Статистика", CellValues.String, 5);
                        sheetData.Append(row3);

                        Row row4 = new Row { RowIndex = 4 };
                        InsertCell(row4, 1, "Количество выполненных маршрутов", CellValues.String, 5);
                        InsertCell(row4, 3, route.Where(p => p.IsDeleted == true).Count().ToString(), CellValues.String, 5);
                        sheetData.Append(row4);

                        Row row5 = new Row { RowIndex = 5 };
                        InsertCell(row5, 1, "Общее количество маршрутов", CellValues.String, 5);
                        InsertCell(row5, 3, route.Count.ToString(), CellValues.String, 5);
                        sheetData.Append(row5);

                        Row row6 = new Row { RowIndex = 6 };
                        InsertCell(row6, 1, "Количество маршрутов в работе", CellValues.String, 5);
                        InsertCell(row6, 3, route.Where(p => p.IsDeleted == false).Count().ToString(), CellValues.String, 5);
                        sheetData.Append(row6);

                        Row row10 = new Row { RowIndex = 8 };
                        InsertCell(row10, 5, "Список маршрутов", CellValues.String, 5);
                        sheetData.Append(row10);

                        Row row11 = new Row { RowIndex = 9 };
                        InsertCell(row11, 1, "№п/п", CellValues.String, 5);
                        InsertCell(row11, 2, "Название маршрута", CellValues.String, 5);
                        InsertCell(row11, 3, "Название груза", CellValues.String, 5);
                        InsertCell(row11, 4, "Вес груза", CellValues.String, 5);
                        InsertCell(row11, 5, "Дата регистрации", CellValues.String, 5);
                        InsertCell(row11, 6, "Дата назначения водителя", CellValues.String, 5);
                        InsertCell(row11, 7, "Дата выполнения", CellValues.String, 5);
                        InsertCell(row11, 8, "Количество контрольных точек", CellValues.String, 5);
                        InsertCell(row11, 9, "Количество пройденные точки", CellValues.String, 5);
                        InsertCell(row11, 10, "Имя логиста", CellValues.String, 5);
                        sheetData.Append(row11);

                        uint idx = 11;
                        int counter = 1;
                        foreach (var item in route)
                        {

                            Row row = new Row { RowIndex = idx };
                            sheetData.Append(row);
                            InsertCell(row, 1, counter.ToString(), CellValues.String, 5);
                            InsertCell(row, 2, item.Name, CellValues.String, 5);
                            InsertCell(row, 3, item.Goods, CellValues.String, 5);
                            InsertCell(row, 4, item.Kilo.ToString(), CellValues.String, 5);
                            InsertCell(row, 5, item.CreatedDate.ToShortDateString().ToString(), CellValues.String, 5);
                            InsertCell(row, 6, item.AppointDate.ToShortDateString().ToString(), CellValues.String, 5);
                            if (item.IsDeleted)
                            {
                                InsertCell(row, 7, item.FullffilDate.ToShortDateString().ToString(), CellValues.String, 5);
                            }
                            else
                            {
                                InsertCell(row, 7, "Не выполнен", CellValues.String, 5);
                            }
                            InsertCell(row, 8, item.Points.Count.ToString(), CellValues.String, 5);
                            var logist = await _userService.GetByChatIdAsync(item.LogicChatId);
                            InsertCell(row, 9, item.Points.Where(p => p.IsFullfil == true).Count().ToString(), CellValues.String, 5);
                            InsertCell(row, 10, logist.FirstName + " " + logist.LastName, CellValues.String, 5);

                            counter++;
                            idx++;
                        }
                    }

                }

                return name;
            }
            catch (Exception ex)
            {
                return "Exception" + " " + ex.Message;
            }
        }
        public async Task<string> GetReportAllRoutes(IList<Route> route)
        {
            string name = "AllRouteReport.xls";
            try
            {
                using (Stream memoryStream = File.Create(name))
                {
                    using (SpreadsheetDocument document =
                            SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook, true))
                    {
                        WorkbookPart workbookPart = document.AddWorkbookPart();
                        workbookPart.Workbook = new Workbook();
                        WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();

                        FileVersion fv = new FileVersion();
                        fv.ApplicationName = "Microsoft Office Excel";
                        worksheetPart.Worksheet = new Worksheet(new SheetData());
                        worksheetPart = CreateColumns(worksheetPart);
                        Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                        Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Отчет о маршрутах" };
                        sheets.Append(sheet);
                        SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                        var stat = GetStatistic(route);
                        Row rowFirst = new Row { RowIndex = 1 };
                        sheetData.Append(rowFirst);
                        InsertCell(rowFirst, 4, "Отчет по работе на", CellValues.String, 5);
                        InsertCell(rowFirst, 5, "Всех маршрутах", CellValues.String, 5);

                        Row rowSecond = new Row { RowIndex = 2 };
                        InsertCell(rowSecond, 1, " ", CellValues.String, 5);
                        sheetData.Append(rowSecond);

                        Row row3 = new Row { RowIndex = 3 };
                        InsertCell(row3, 7, "Статистика", CellValues.String, 5);
                        sheetData.Append(row3);

                        Row row4 = new Row { RowIndex = 4 };
                        InsertCell(row4, 1, "Количество выполненных маршрутов", CellValues.String, 5);
                        InsertCell(row4, 3, route.Where(p => p.IsDeleted == true).Count().ToString(), CellValues.String, 5);
                        sheetData.Append(row4);

                        Row row5 = new Row { RowIndex = 5 };
                        InsertCell(row5, 1, "Общее количество маршрутов", CellValues.String, 5);
                        InsertCell(row5, 3, route.Count.ToString(), CellValues.String, 5);
                        sheetData.Append(row5);

                        Row row6 = new Row { RowIndex = 6 };
                        InsertCell(row6, 1, "Количество маршрутов в работе", CellValues.String, 5);
                        InsertCell(row6, 3, route.Where(p => p.IsDeleted == false).Count().ToString(), CellValues.String, 5);
                        sheetData.Append(row6);

                        Row row7 = new Row { RowIndex = 7 };
                        InsertCell(row7, 1, "Минимальное время назначения", CellValues.String, 5);
                        InsertCell(row7, 3, stat.MinTimeAssign, CellValues.String, 5);
                        sheetData.Append(row7);


                        Row row8 = new Row { RowIndex = 8 };
                        InsertCell(row8, 1, "Максимальное время назначения", CellValues.String, 5);
                        InsertCell(row8, 3, stat.MaxTimeAssign, CellValues.String, 5);
                        sheetData.Append(row8);

                        Row row9 = new Row { RowIndex = 9 };
                        InsertCell(row9, 1, "Среднее время назначения", CellValues.String, 5);
                        InsertCell(row9, 3, stat.AvgTimeAssign, CellValues.String, 5);
                        sheetData.Append(row9);

                        Row row10 = new Row { RowIndex = 10 };
                        InsertCell(row10, 1, "Минимальное время выполнения", CellValues.String, 5);
                        InsertCell(row10, 3, stat.MinTimeFulfil, CellValues.String, 5);
                        sheetData.Append(row10);


                        Row row11 = new Row { RowIndex = 11 };
                        InsertCell(row11, 1, "Максимальное время выполнения", CellValues.String, 5);
                        InsertCell(row11, 3, stat.MaxTimeFulfil, CellValues.String, 5);
                        sheetData.Append(row11);

                        Row row12 = new Row { RowIndex = 12 };
                        InsertCell(row12, 1, "Среднее время выполнения", CellValues.String, 5);
                        InsertCell(row12, 3, stat.AvgTimeFulfil, CellValues.String, 5);
                        sheetData.Append(row12);

                        Row row14 = new Row { RowIndex = 14 };
                        InsertCell(row14, 5, "Список маршрутов", CellValues.String, 5);
                        sheetData.Append(row14);

                        Row row15 = new Row { RowIndex = 15 };
                        InsertCell(row15, 1, "№п/п", CellValues.String, 5);
                        InsertCell(row15, 2, "Название маршрута", CellValues.String, 5);
                        InsertCell(row15, 3, "Название груза", CellValues.String, 5);
                        InsertCell(row15, 4, "Вес груза", CellValues.String, 5);
                        InsertCell(row15, 5, "Дата регистрации", CellValues.String, 5);
                        InsertCell(row15, 6, "Дата назначения водителя", CellValues.String, 5);
                        InsertCell(row15, 7, "Дата выполнения", CellValues.String, 5);
                        InsertCell(row15, 8, "Контрольных точек", CellValues.String, 5);
                        InsertCell(row15, 9, "Пройдено точек", CellValues.String, 5);
                        InsertCell(row15, 10, "Имя логиста", CellValues.String, 5);
                        InsertCell(row15, 11, "Имя водителя", CellValues.String, 5);
                        sheetData.Append(row15);

                        uint idx = 16;
                        int counter = 1;
                        foreach (var item in route)
                        {

                            Row row = new Row { RowIndex = idx };
                            sheetData.Append(row);
                            InsertCell(row, 1, counter.ToString(), CellValues.String, 5);
                            InsertCell(row, 2, item.Name, CellValues.String, 5);
                            InsertCell(row, 3, item.Goods, CellValues.String, 5);
                            InsertCell(row, 4, item.Kilo.ToString(), CellValues.String, 5);
                            InsertCell(row, 5, item.CreatedDate.ToShortDateString().ToString(), CellValues.String, 5);
                            InsertCell(row, 6, item.AppointDate.ToShortDateString().ToString(), CellValues.String, 5);
                            if (item.IsDeleted)
                            {
                                InsertCell(row, 7, item.FullffilDate.ToShortDateString().ToString(), CellValues.String, 5);
                            }
                            else
                            {
                                InsertCell(row, 7, "Не выполнен", CellValues.String, 5);
                            }
                            InsertCell(row, 8, item.Points.Count.ToString(), CellValues.String, 5);
                            var logist = await _userService.GetByChatIdAsync(item.LogicChatId);
                            var driver = await _userService.GetByChatIdAsync(item.DriverChatId);
                            InsertCell(row, 9, item.Points.Where(p => p.IsFullfil == true).Count().ToString(), CellValues.String, 5);
                            InsertCell(row, 10, logist.FirstName + " " + logist.LastName, CellValues.String, 5);
                            InsertCell(row, 11, driver.FirstName + " " + driver.LastName, CellValues.String, 5);

                            counter++;
                            idx++;
                        }
                    }

                }

                return name;
            }
            catch (Exception ex)
            {
                return "Exception" + " " + ex.Message;
            }
        }
        private StatisticDto GetStatistic(IList<Route> route)
        {
            TimeSpan minValAss = TimeSpan.MaxValue;
            TimeSpan maxValAss = TimeSpan.MinValue;

            TimeSpan minValFul = TimeSpan.MaxValue;
            TimeSpan maxValFul = TimeSpan.MinValue;

            TimeSpan valAss = TimeSpan.Zero;
            TimeSpan valFul = TimeSpan.Zero;
            int cnt = 0;
            foreach (var item in route)
            {
                if (item.IsDeleted == true)
                {
                    cnt++;
                    var currValFul = item.FullffilDate - item.AppointDate;
                    var currValAss = item.AppointDate - item.CreatedDate;
                    valAss += currValFul;
                    valFul += currValAss;
                    if (currValFul > maxValFul)
                    {
                        maxValFul = currValFul;
                    }
                    if (currValFul < minValFul)
                    {
                        minValFul = currValFul;
                    }
                    if (currValAss > maxValAss)
                    {
                        maxValAss = currValAss;
                    }
                    if (currValAss < minValAss)
                    {
                        minValAss = currValAss;
                    }
                }

            }
            var ret = new StatisticDto();

            ret.AvgTimeAssign = String.Format("Hours: {0}, Min: {1}", (valAss / route.Count).Hours, (valAss / route.Count).Minutes);
            ret.MaxTimeAssign = String.Format("Hours: {0}, Min: {1}", maxValAss.Hours, maxValAss.Minutes);
            ret.MinTimeAssign = String.Format("Hours: {0}, Min: {1}", minValAss.Hours, minValAss.Minutes);

            ret.AvgTimeFulfil = String.Format("Hours: {0}, Min: {1}", (valFul / route.Count).Hours, (valFul / route.Count).Minutes);
            ret.MaxTimeFulfil = String.Format("Hours: {0}, Min: {1}", maxValFul.Hours, maxValFul.Minutes);
            ret.MinTimeFulfil = String.Format("Hours: {0}, Min: {1}", minValFul.Hours, minValFul.Minutes);
            return ret;
        }

        static void InsertCell(Row row, int cell_num, string val, CellValues type, uint styleIndex)
        {
            Cell refCell = null;
            Cell newCell = new Cell() { CellReference = cell_num.ToString() + ":" + row.RowIndex.ToString() };
            row.InsertBefore(newCell, refCell);

            // Устанавливает тип значения.
            newCell.CellValue = new CellValue(val);
            newCell.DataType = new EnumValue<CellValues>(type);

        }
        static string ReplaceHexadecimalSymbols(string txt)
        {
            string r = "[\x00-\x08\x0B\x0C\x0E-\x1F\x26]";
            return Regex.Replace(txt, r, "", RegexOptions.Compiled);
        }
        public static void UpdateCell(WorksheetPart worksheetPart, string text,
            uint rowIndex, string columnName)
        {

            if (worksheetPart != null)
            {
                Cell cell = GetCell(worksheetPart.Worksheet, columnName, rowIndex);
                cell.CellValue = new CellValue(text);
                worksheetPart.Worksheet.Save();
            }

        }

        private static WorksheetPart GetWorksheetPartByName(SpreadsheetDocument document, string sheetName)
        {
            IEnumerable<Sheet> sheets =
               document.WorkbookPart.Workbook.GetFirstChild<Sheets>().
               Elements<Sheet>().Where(s => s.Name == sheetName);
            if (sheets.Count() == 0)
            {
                return null;
            }
            string relationshipId = sheets.First().Id.Value;
            WorksheetPart worksheetPart = (WorksheetPart)
                 document.WorkbookPart.GetPartById(relationshipId);
            return worksheetPart;
        }

        private static Cell GetCell(Worksheet worksheet, string columnName, uint rowIndex)
        {
            Row row = GetRow(worksheet, rowIndex);
            if (row == null)
                return null;
            return row.Elements<Cell>().Where(c => string.Compare
                (c.CellReference.Value, columnName +
                rowIndex, true) == 0).First();

        }
        private static Row GetRow(Worksheet worksheet, uint rowIndex)
        {
            var r = worksheet.GetFirstChild<SheetData>().
             Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
            return r;
        }

        private WorksheetPart CreateColumns(WorksheetPart worksheetPart)
        {
            Columns lstColumns = worksheetPart.Worksheet.GetFirstChild<Columns>();
            Boolean needToInsertColumns = false;
            if (lstColumns == null)
            {
                lstColumns = new Columns();
                needToInsertColumns = true;
            }
            lstColumns.Append(new Column() { Min = 1, Max = 10, Width = 20, CustomWidth = true });
            lstColumns.Append(new Column() { Min = 2, Max = 10, Width = 20, CustomWidth = true });
            lstColumns.Append(new Column() { Min = 3, Max = 10, Width = 20, CustomWidth = true });
            lstColumns.Append(new Column() { Min = 4, Max = 10, Width = 20, CustomWidth = true });
            lstColumns.Append(new Column() { Min = 5, Max = 10, Width = 20, CustomWidth = true });
            lstColumns.Append(new Column() { Min = 6, Max = 10, Width = 20, CustomWidth = true });
            lstColumns.Append(new Column() { Min = 7, Max = 10, Width = 20, CustomWidth = true });
            lstColumns.Append(new Column() { Min = 7, Max = 10, Width = 20, CustomWidth = true });
            lstColumns.Append(new Column() { Min = 7, Max = 10, Width = 20, CustomWidth = true });
            lstColumns.Append(new Column() { Min = 7, Max = 10, Width = 20, CustomWidth = true });
            if (needToInsertColumns)
                worksheetPart.Worksheet.InsertAt(lstColumns, 0);


            return worksheetPart;
        }

        
    }
}
